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
            var values = new FormUrlEncodedContent(param);
            HttpResponseMessage response = await theAuthClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            return UserEntity.Parse(responseContent, userAccountEntity);
        }

        public static async Task<bool> ChangeUserProfile(String Name, String URL, String Description, String Location,
            UserAccountEntity userAccountEntity)
        {
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
            }
            var param = new Dictionary<String, String>();
            if (!string.IsNullOrEmpty(Name)) param.Add("name", Name);
            if (!string.IsNullOrEmpty(URL)) param.Add("url", URL);
            if (!string.IsNullOrEmpty(Location)) param.Add("location", Location);
            if (!string.IsNullOrEmpty(Description)) param.Add("description", Description);

            var theAuthClient = new HttpClient();
            HttpContent header = new FormUrlEncodedContent(param);
            var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.ACCOUNT_UPDATE);
            request.Content = header;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            HttpResponseMessage response = await theAuthClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public static async Task<bool> ChangeUserProfileImage(String filePath, byte[] fileStream,
            UserAccountEntity userAccountEntity)
        {
            try
            {
                String boundary = "CroudiaFormBoundary";

                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.ACCOUNT_UPDATE_IMAGE);
                var form = new MultipartFormDataContent();
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(EndPoints.STATUS_UPDATE_WITH_MEDIA);
                //request.Method = "POST";
                //request. = "multipart/form-data;boundary=" + boundary;
                if (userAccountEntity != null)
                {
                    if (userAccountEntity.GetAccessToken().Equals("refresh"))
                    {
                        await Auth.RefreshAccessToken(userAccountEntity);
                    }
                }
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",
                    userAccountEntity.GetAccessToken());
                Stream stream = new MemoryStream(fileStream);
                var t = new StreamContent(stream);
                string FilePath = "testtesttesttest.jpg";
                if (Path.GetExtension(FilePath).Equals(".png"))
                {
                    t.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                }
                else if (Path.GetExtension(FilePath).Equals(".jpg") || Path.GetExtension(FilePath).Equals(".jpeg"))
                {
                    t.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                }
                else
                {
                    t.Headers.ContentType = new MediaTypeHeaderValue("image/gif");
                }
                //t.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpg");
                form.Add(t, @"""image""", FilePath);
                request.Content = form;
                string requestContent = await request.Content.ReadAsStringAsync();
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent2 = await response.Content.ReadAsStringAsync();
                return response.IsSuccessStatusCode;
            }
            catch (WebException e)
            {
                return false;
            }
        }

        public static async Task<List<UserEntity>> LookupUsers(String[] screennames, long?[] userIds,
            UserAccountEntity userAccountEntity)
        {
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
            }
            var param = new Dictionary<String, String>();
            String url = "?screen_name=";
            if (screennames.Length != 0)
            {
                String prms = "";
                foreach (String sn in screennames)
                {
                    url += sn + ",";
                }
                // param.Add("screen_name", prms.Substring(0, prms.Length - 1));
            }
            if (userIds != null && userIds.Length != 0)
            {
                String prms = "";
                foreach (long id in userIds)
                {
                    prms += id + ",";
                }
                param.Add("user_id", prms.Substring(0, prms.Length - 1));
            }
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, EndPoints.USERS_LOOKUP + url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            var values = new FormUrlEncodedContent(param);
            HttpResponseMessage response = await theAuthClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            return UserEntity.ParseUserList(responseContent, userAccountEntity);
        }

        private static string ParseHtmlString(string txt)
        {
            string re1 = ".*?"; // Non-greedy match on filler
            string re2 = "((?:[a-z][a-z]+))"; // Word 1
            String word1 = string.Empty;
            var r = new Regex(re1 + re2, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(txt);
            if (m.Success)
            {
                word1 = m.Groups[1].ToString();
            }
            return word1;
        }

        public static async Task<List<UserEntity>> LookupFollowingUsers(int cursor, long? userId,
            UserAccountEntity userAccountEntity)
        {
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
            }
            string paramer = "?cursor=" + cursor + "&user_id=" + userId;
            var param = new Dictionary<String, String>();
            //param.Add("cursor", "-1");
            //if (userId != null) param.Add("user_id", userId.ToString());
            var theAuthClient = new HttpClient();
            //if (userId != null) param.Add("user_id", userId.ToString());
            var request = new HttpRequestMessage(HttpMethod.Get, EndPoints.FRIEND_LIST + paramer);
            //var values = new FormUrlEncodedContent(param);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            HttpResponseMessage response = await theAuthClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            responseContent = "[" + responseContent + "]";
            JArray a = JArray.Parse(responseContent);
            var b = (JObject) a[0];
            return UserEntity.ParseUserList(b["users"].ToString(), userAccountEntity);
        }

        public static async Task<List<UserEntity>> LookupFollowerUsers(int cursor, long? userId,
            UserAccountEntity userAccountEntity)
        {
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
            }
            string paramer = "?cursor=" + cursor + "&user_id=" + userId;
            //Dictionary<String, String> param = new Dictionary<String, String>();
            //if (userId != null) param.Add("user_id", userId.ToString());
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, EndPoints.FOLLOWERS_LIST + paramer);
            //var values = new FormUrlEncodedContent(param);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            HttpResponseMessage response = await theAuthClient.SendAsync(request);

            string responseContent = await response.Content.ReadAsStringAsync();

            responseContent = "[" + responseContent + "]";
            JArray a = JArray.Parse(responseContent);
            var b = (JObject) a[0];
            return UserEntity.ParseUserList(b["users"].ToString(), userAccountEntity);
        }
    }
}