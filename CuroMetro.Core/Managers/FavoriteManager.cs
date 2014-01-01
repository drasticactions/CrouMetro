using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Tools;
using HtmlAgilityPack;

namespace CrouMetro.Core.Managers
{
    public class FavoriteManager
    {
        public static async Task<HttpStatusCode> CreateFavorite(long id, bool? trim, UserAccountEntity userAccountEntity)
        {
            var param = new Dictionary<String, String>();
            if (trim == true) param.Add("trim_user", trim.ToString());
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, (String.Format(EndPoints.FAVORITE_CREATE, id)));

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
            return response.StatusCode;
            //return Http.HttpPost(String.Format(EndPoints.SPREAD_CREATE, id.ToString()), param, ref account, out content);
        }

        public static async Task<HttpStatusCode> DestoryFavorite(long id, UserAccountEntity userAccountEntity)
        {
            var param = new Dictionary<String, String>();
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, (String.Format(EndPoints.FAVORITE_DESTROY, id)));

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
            return response.StatusCode;
            //return Http.HttpPost(String.Format(EndPoints.SPREAD_CREATE, id.ToString()), param, ref account, out content);
        }

        public static async Task<List<PostEntity>> getFavorite(bool? trim, UserAccountEntity userAccountEntity)
        {
            var param = new Dictionary<String, String>();
            if (trim == true) param.Add("trim_user", trim.ToString());
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.FAVORITES);

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
            string responseContent = await response.Content.ReadAsStringAsync();
            return PostEntity.Parse(responseContent, userAccountEntity);
            //return Http.HttpPost(String.Format(EndPoints.SPREAD_CREATE, id.ToString()), param, ref account, out content);
        }

        public static async Task<List<PostEntity>> getFavoriteHtmlParse(int offSet, String screenName, bool? trim,
            UserAccountEntity userAccountEntity)
        {
            string accessToken = userAccountEntity.GetAccessToken();
            if (accessToken.Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
                accessToken = userAccountEntity.GetAccessToken();
            }
            var statusIdList = new List<long>();
            var theAuthClient = new HttpClient();
            var requestMsg = new HttpRequestMessage(new HttpMethod("GET"),
                "https://croudia.com/favorites/favodia/" + screenName + "?offset=" + offSet);
            HttpResponseMessage response = await theAuthClient.SendAsync(requestMsg);
            string responseContent = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(responseContent);
            foreach (
                HtmlNode link in
                    doc.DocumentNode.Descendants()
                        .Where(node => node.GetAttributeValue("class", "").Contains("test_a_margin")))
            {
                statusIdList.Add(ParseHtmlString(link.GetAttributeValue("href", "")));
            }
            var postList = new List<PostEntity>();
            for (int i = 0; i < 5; i++)
            {
                PostEntity newPost = await StatusManager.GetPost(false, true, statusIdList[i], userAccountEntity);
                postList.Add(newPost);
            }
            return postList;
        }

        private static long ParseHtmlString(string txt)
        {
            string re1 = ".*?"; // Non-greedy match on filler
            string re2 = "(\\d+)"; // Integer Number 1
            String int1 = string.Empty;
            var r = new Regex(re1 + re2, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(txt);
            if (m.Success)
            {
                int1 = m.Groups[1].ToString();
            }
            return (long) Convert.ToDouble(int1);
        }
    }
}