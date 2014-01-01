using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Tools;
using Newtonsoft.Json.Linq;

namespace CrouMetro.Core.Managers
{
    public class SearchManager
    {
        public static async Task<List<UserEntity>> SearchUserList(string query, int? count, int? page, bool trim,
            UserAccountEntity userAccountEntity)
        {
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
            }
            string paramer = "?q=" + query;
            if (count.HasValue)
            {
                paramer += "&count=" + count.Value;
            }
            if (page.HasValue)
            {
                paramer += "&page=" + page.Value;
            }
            if (trim)
            {
                paramer += "&trim_user=" + trim;
            }
            var theAuthClient = new HttpClient();
            //if (userId != null) param.Add("user_id", userId.ToString());
            var request = new HttpRequestMessage(HttpMethod.Get, EndPoints.SEARCH_USERS + paramer);
            //var values = new FormUrlEncodedContent(param);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            HttpResponseMessage response = await theAuthClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            return UserEntity.ParseUserList(responseContent, userAccountEntity);
        }

        public static async Task<SearchEntity> SearchStatusList(string query, long? MaxId, long? SinceId, int? count,
            bool trim, bool entities, UserAccountEntity userAccountEntity)
        {
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
            }
            string paramer = "?q=" + query;
            if (MaxId.HasValue)
            {
                paramer += "&max_id=" + MaxId.Value;
            }
            if (SinceId.HasValue)
            {
                paramer += "&since_id=" + SinceId.Value;
            }
            if (count.HasValue)
            {
                paramer += "&count=" + count.Value;
            }
            if (trim)
            {
                paramer += "&trim_user=" + trim;
            }
            if (entities)
            {
                paramer += "&include_entities=" + entities;
            }
            var param = new Dictionary<String, String>();
            //param.Add("cursor", "-1");
            //if (userId != null) param.Add("user_id", userId.ToString());
            var theAuthClient = new HttpClient();
            //if (userId != null) param.Add("user_id", userId.ToString());
            var request = new HttpRequestMessage(HttpMethod.Get, EndPoints.SEARCH_VOICES + paramer);
            //var values = new FormUrlEncodedContent(param);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            HttpResponseMessage response = await theAuthClient.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();
            responseContent = "[" + responseContent + "]";
            JArray a = JArray.Parse(responseContent);
            var b = (JObject) a[0];
            return SearchEntity.ParseStatuses(b["statuses"].ToString(), b["serach_metadata"].ToString(),
                userAccountEntity);
        }
    }
}