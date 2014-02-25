using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Tools;
using Newtonsoft.Json.Linq;

namespace CrouMetro.Core.Managers
{
    public class StatusManager
    {
        public static async Task<bool> UpdateStatus(String status, long? inReply, bool? inQuote, bool? trim,
            UserAccountEntity userAccountEntity)
        {
            var param = new Dictionary<String, String> {{"status", status}};
            if (inReply != null) param.Add("in_reply_to_status_id", inReply.ToString());
            if (inQuote == true) param.Add("in_reply_with_quote", true.ToString());
            if (trim == true) param.Add("trim_user", true.ToString());

            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.STATUS_UPDATE);

            string accessToken = userAccountEntity.GetAccessToken();
            if (accessToken.Equals("refresh"))
            {
                await AuthenticationManager.RefreshAccessToken(userAccountEntity);
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


        public static async Task<HttpStatusCode> DestroyStatus(long statusId, UserAccountEntity userAccountEntity)
        {
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, string.Format(EndPoints.STATUS_DESTROY, statusId));

            string accessToken = userAccountEntity.GetAccessToken();
            if (accessToken.Equals("refresh"))
            {
                await AuthenticationManager.RefreshAccessToken(userAccountEntity);
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

        public static async Task<PostEntity> GetPost(bool? trim, bool? entities, long postId,
            UserAccountEntity userAccountEntity)
        {
            string url = string.Format(EndPoints.STATUS_SHOW, postId);
            if (trim != null) url += "&trim_user=" + trim;
            if (entities != null) url += "&include_entities=" + entities;
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            HttpResponseMessage response = await theAuthClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            JObject post = JObject.Parse(responseContent);
            return PostEntity.ParsePost(post, userAccountEntity);
        }

        public static async Task<bool> UpdateStatusWithMedia(String status, String path, byte[] fileStream,
            long? inReply, bool? isQuote, bool? trim, UserAccountEntity account)
        {
            try
            {
                Debug.WriteLine(EndPoints.STATUS_UPDATE_WITH_MEDIA);

                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.STATUS_UPDATE_WITH_MEDIA);
                var form = new MultipartFormDataContent();
                if (account != null)
                {
                    if (account.GetAccessToken().Equals("refresh"))
                    {
                        await AuthenticationManager.RefreshAccessToken(account);
                    }
                }
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", account.GetAccessToken());
                form.Add(new StringContent(status), @"""status""");
                Stream stream = new MemoryStream(fileStream);
                var t = new StreamContent(stream);
                const string fileName = "test.jpg";
                string extension = Path.GetExtension(path);
                if (extension != null && extension.Equals(".png"))
                {
                    t.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                }
                else
                {
                    string s = Path.GetExtension(path);
                    if (s != null && (s.Equals(".jpg") || s.Equals(".jpeg")))
                    {
                        t.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                    }
                    else
                    {
                        t.Headers.ContentType = new MediaTypeHeaderValue("image/gif");
                    }
                }
                t.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                form.Add(t, @"""media""", fileName);
                request.Content = form;
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (WebException e)
            {
                Debug.WriteLine(((HttpWebResponse) e.Response).StatusDescription);
                Debug.WriteLine(new StreamReader(e.Response.GetResponseStream()).ReadToEnd());
                return false;
            }
        }
    }
}