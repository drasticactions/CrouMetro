using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Controls;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Tools;
using Newtonsoft.Json.Linq;

namespace CrouMetro.Core.Managers
{
    public class StatusManager
    {
        /// <summary>
        ///     ツイートを投稿します。
        /// </summary>
        /// <param name="status">ツイート本文</param>
        /// <param name="inReply">Mentionsの送信先</param>
        /// <param name="inQuote">引用ツイートか否か</param>
        /// <param name="trim">TrimUserを利用するか否か</param>
        /// <param name="account">投稿するアカウント</param>
        /// <param name="content">レスポンス</param>
        /// <returns></returns>
        public static async Task<bool> UpdateStatus(String status, long? inReply, bool? inQuote, bool? trim,
            UserAccountEntity userAccountEntity)
        {
            var param = new Dictionary<String, String>();
            param.Add("status", status);
            if (inReply != null) param.Add("in_reply_to_status_id", inReply.ToString());
            if (inQuote == true) param.Add("in_reply_with_quote", inQuote.ToString());
            if (trim == true) param.Add("trim_user", trim.ToString());

            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.STATUS_UPDATE);

            string accessToken = userAccountEntity.GetAccessToken();
            if (accessToken.Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
                accessToken = userAccountEntity.GetAccessToken();
            }

            HttpContent header = new FormUrlEncodedContent(param);
            request.Content = header;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await theAuthClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }


        public static async Task<HttpStatusCode> DestroyStatus(long statusId, UserAccountEntity userAccountEntity)
        {
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, string.Format(EndPoints.STATUS_DESTROY, statusId));

            string accessToken = userAccountEntity.GetAccessToken();
            if (accessToken.Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
                accessToken = userAccountEntity.GetAccessToken();
            }
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await theAuthClient.SendAsync(request);
            return response.StatusCode;
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

        public static List<HyperlinkButton> CreateLinkButtons(IEnumerable<String> Links)
        {
            var hyperlinkButtonList = new List<HyperlinkButton>();
            foreach (string link in Links)
            {
                var button = new HyperlinkButton();
                button.NavigateUri = new Uri(link);
                button.Content = link;
                button.FontSize = 25;
                hyperlinkButtonList.Add(button);
            }
            return hyperlinkButtonList;
        }

        /// <summary>
        ///     ツイートを画像と一緒に投稿します。
        /// </summary>
        /// <param name="status">ツイート本文</param>
        /// <param name="path">投稿する画像の本文</param>
        /// <param name="inReply">Mentionsの送信先</param>
        /// <param name="isQuote">引用ツイートか否か</param>
        /// <param name="trim">TrimUserを利用するか否か</param>
        /// <param name="account">投稿するアカウント</param>
        /// <param name="content">レスポンス</param>
        /// <returns></returns>
        public static async Task<bool> UpdateStatusWithMedia(String status, String path, byte[] fileStream,
            long? inReply, bool? isQuote, bool? trim, UserAccountEntity account)
        {
            try
            {
                Debug.WriteLine(EndPoints.STATUS_UPDATE_WITH_MEDIA);

                String boundary = "CroudiaFormBoundary";

                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.STATUS_UPDATE_WITH_MEDIA);
                var form = new MultipartFormDataContent();
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(EndPoints.STATUS_UPDATE_WITH_MEDIA);
                //request.Method = "POST";
                //request. = "multipart/form-data;boundary=" + boundary;
                if (account != null)
                {
                    if (account.GetAccessToken().Equals("refresh"))
                    {
                        await Auth.RefreshAccessToken(account);
                    }
                }
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", account.GetAccessToken());
                form.Add(new StringContent(status), @"""status""");
                Stream stream = new MemoryStream(fileStream);
                var t = new StreamContent(stream);
                string fileName = "testtesttesttest.jpg";
                if (Path.GetExtension(path).Equals(".png"))
                {
                    t.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                }
                else if (Path.GetExtension(path).Equals(".jpg") || Path.GetExtension(path).Equals(".jpeg"))
                {
                    t.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                }
                else
                {
                    t.Headers.ContentType = new MediaTypeHeaderValue("image/gif");
                }
                t.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                form.Add(t, @"""media""", fileName);
                request.Content = form;
                string requestContent = await request.Content.ReadAsStringAsync();
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent2 = await response.Content.ReadAsStringAsync();
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