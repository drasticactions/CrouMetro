using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Tools;
using Newtonsoft.Json.Linq;

namespace CrouMetro.Core.Managers
{
    public class UserManager
    {
        public static async Task<UserEntity> ShowUser(String screenname, long? userId,
            UserAccountEntity userAccountEntity)
        {
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
            }
            var param = new Dictionary<String, String>();
            if (screenname != null) param.Add("screen_name", screenname);
            if (userId != null) param.Add("user_id", userId.ToString());
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, EndPoints.USERS_SHOW);
            try
            {
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                return UserEntity.Parse(responseContent, userAccountEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<bool> ChangeUserProfile(String name, String url, String description, String location,
            UserAccountEntity userAccountEntity)
        {
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
            }
            var param = new Dictionary<String, String>();
            if (!string.IsNullOrEmpty(name)) param.Add("name", name);
            if (!string.IsNullOrEmpty(url)) param.Add("url", url);
            if (!string.IsNullOrEmpty(location)) param.Add("location", location);
            if (!string.IsNullOrEmpty(description)) param.Add("description", description);

            var theAuthClient = new HttpClient();
            HttpContent header = new FormUrlEncodedContent(param);
            var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.ACCOUNT_UPDATE) {Content = header};
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            try
            {
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<bool> ChangeUserProfileImage(String filePath, byte[] fileStream,
            UserAccountEntity userAccountEntity)
        {
            try
            {
                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.ACCOUNT_UPDATE_IMAGE);
                var form = new MultipartFormDataContent();
                if (userAccountEntity != null)
                {
                    if (userAccountEntity.GetAccessToken().Equals("refresh"))
                    {
                        await Auth.RefreshAccessToken(userAccountEntity);
                    }
                }
                if (userAccountEntity != null)
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",
                        userAccountEntity.GetAccessToken());
                Stream stream = new MemoryStream(fileStream);
                var t = new StreamContent(stream);
                const string fileName = "testtesttesttest.jpg";
                if (Path.GetExtension(fileName).Equals(".png"))
                {
                    t.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                }
                else if (Path.GetExtension(fileName).Equals(".jpg") || Path.GetExtension(fileName).Equals(".jpeg"))
                {
                    t.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                }
                else
                {
                    t.Headers.ContentType = new MediaTypeHeaderValue("image/gif");
                }
                form.Add(t, @"""image""", fileName);
                request.Content = form;
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (WebException e)
            {
                return false;
            }
        }

        public static async Task<List<UserEntity>> LookupFollowingUsers(int cursor, long? userId,
            UserAccountEntity userAccountEntity)
        {
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
            }
            string paramer = "?cursor=" + cursor + "&user_id=" + userId;
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, EndPoints.FRIEND_LIST + paramer);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            try
            {
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                responseContent = "[" + responseContent + "]";
                JArray a = JArray.Parse(responseContent);
                var b = (JObject)a[0];
                return UserEntity.ParseUserList(b["users"].ToString(), userAccountEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<List<UserEntity>> LookupFollowerUsers(int cursor, long? userId,
            UserAccountEntity userAccountEntity)
        {
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
            }
            string paramer = "?cursor=" + cursor + "&user_id=" + userId;
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, EndPoints.FOLLOWERS_LIST + paramer);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            try
            {
                HttpResponseMessage response = await theAuthClient.SendAsync(request);

                string responseContent = await response.Content.ReadAsStringAsync();

                responseContent = "[" + responseContent + "]";
                JArray a = JArray.Parse(responseContent);
                var b = (JObject)a[0];
                return UserEntity.ParseUserList(b["users"].ToString(), userAccountEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}