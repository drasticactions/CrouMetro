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
    public class SearchManager
    {
        public static async Task<List<UserEntity>> SearchUserList(string query, int? count, int? page, bool trim,
            UserAccountEntity userAccountEntity)
        {
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
            }
            var paramer = "?q=" + query;
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
                paramer += "&trim_user=" + true;
            }
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, EndPoints.SEARCH_USERS + paramer);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            try
            {
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                return UserEntity.ParseUserList(responseContent, userAccountEntity);
            }
            catch (Exception)
            {
                return null;
            }
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
                paramer += "&trim_user=" + true;
            }
            if (entities)
            {
                paramer += "&include_entities=" + true;
            }
            var param = new Dictionary<String, String>();
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, EndPoints.SEARCH_VOICES + paramer);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            try
            {
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                responseContent = "[" + responseContent + "]";
                JArray a = JArray.Parse(responseContent);
                var b = (JObject)a[0];
                if (b["statuses"] == null || b["search_metadata"] == null) return null;
                return SearchEntity.ParseStatuses(b["statuses"].ToString(), b["search_metadata"].ToString(),
                    userAccountEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}