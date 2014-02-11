using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Tools;
using Newtonsoft.Json.Linq;

namespace CrouMetro.Core.Managers
{
    public class SecretMailManager
    {
        public static async Task<bool> CreateMail(String text, long? userId, String screenName,
            UserAccountEntity userAccountEntity)
        {
            var param = new Dictionary<String, String> {{"text", text}, {"user_id", userId.ToString()}};
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.MESSAGE_NEW);

            string accessToken = userAccountEntity.GetAccessToken();
            if (accessToken.Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
                accessToken = userAccountEntity.GetAccessToken();
            }

            HttpContent header = new FormUrlEncodedContent(param);
            request.Content = header;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
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

        public static async Task<List<SecretMailEntity>> GetSecretMails(long? sinceId, long? maxId, int? count,
            UserAccountEntity userAccountEntity)
        {
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
            }
            string url = EndPoints.MESSAGE_MAILS + "?";
            if (sinceId != null) url += "&since_id=" + sinceId;
            if (maxId != null) url += "&since_id=" + maxId;
            if (count != null) url += "&count=" + count;
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            try
            {
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                return SecretMailEntity.Parse(responseContent, userAccountEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<List<SecretMailEntity>> GetSentSecretMails(long? sinceId, long? maxId, int? count,
            UserAccountEntity userAccountEntity)
        {
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
            }
            string url = EndPoints.MESSAGE_SENT + "?";
            if (sinceId != null) url += "&since_id=" + sinceId;
            if (maxId != null) url += "&since_id=" + maxId;
            if (count != null) url += "&count=" + count;
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            try
            {
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                return SecretMailEntity.Parse(responseContent, userAccountEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<HttpStatusCode> DestroyMail(long Id, UserAccountEntity userAccountEntity)
        {
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, string.Format(EndPoints.MESSAGE_DESTROY, Id));

            string accessToken = userAccountEntity.GetAccessToken();
            if (accessToken.Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
                accessToken = userAccountEntity.GetAccessToken();
            }
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            try
            {
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                return response.StatusCode;
            }
            catch (Exception)
            {
                return HttpStatusCode.BadRequest;
            }
        }

        public static async Task<SecretMailEntity> GetMail(long postId, UserAccountEntity userAccountEntity)
        {
            string url = string.Format(EndPoints.STATUS_SHOW, postId);

            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            try
            {
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                JObject post = JObject.Parse(responseContent);
                return SecretMailEntity.ParseMail(post, userAccountEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}