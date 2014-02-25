using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Tools;

namespace CrouMetro.Core.Managers
{
    public class TimelineManager
    {
        public static async Task<List<PostEntity>> GetPublicTimeline(bool? trim, long? sinceId, long? maxId, int? count,
            UserAccountEntity userAccountEntity)
        {
            string accessToken = userAccountEntity.GetAccessToken();
            if (accessToken.Equals("refresh"))
            {
                await AuthenticationManager.RefreshAccessToken(userAccountEntity);
            }
            string url = EndPoints.PUBLIC_TIMELINE + "?";
            if (sinceId != null) url += "&since_id=" + sinceId;
            if (maxId != null) url += "&max_id=" + maxId;
            if (count != null) url += "&count=" + count;
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            try
            {
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                return PostEntity.Parse(responseContent, userAccountEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<List<PostEntity>> GetHomeTimeline(bool? trim, long? sinceId, long? maxId, int? count,
            UserAccountEntity userAccountEntity)
        {
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await AuthenticationManager.RefreshAccessToken(userAccountEntity);
            }
            string url = EndPoints.HOME_TIMELINE + "?";
            if (sinceId != null) url += "&since_id=" + sinceId;
            if (maxId != null) url += "&max_id=" + maxId;
            if (count != null) url += "&count=" + count;
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            try
            {
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                return PostEntity.Parse(responseContent, userAccountEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<List<PostEntity>> GetMentions(bool? trim, long? sinceId, long? maxId, int? count,
            UserAccountEntity userAccountEntity)
        {
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await AuthenticationManager.RefreshAccessToken(userAccountEntity);
            }

            string url = EndPoints.MENTIONS_TIMELINE + "?";
            //if (trim != null) url += "&trim_user=" + trim.ToString();
            if (sinceId != null) url += "&since_id=" + sinceId;
            if (maxId != null) url += "&max_id=" + maxId;
            if (count != null) url += "&count=" + count;

            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            try
            {
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                return PostEntity.Parse(responseContent, userAccountEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<List<PostEntity>> GetUserTimeline(String screenname, long? userId, bool? trim,
            long? sinceId, long? maxId, int? count, UserAccountEntity userAccountEntity)
        {
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await AuthenticationManager.RefreshAccessToken(userAccountEntity);
            }
            string url = EndPoints.USER_TIMELINE + "?";
            if (screenname != null) url += "&screen_name=" + screenname;
            if (userId != null) url += "&user_id=" + userId;
            if (trim != null) url += "&trim_user=" + trim;
            if (sinceId != null) url += "&since_id=" + sinceId;
            if (maxId != null) url += "&max_id=" + maxId;
            if (count != null) url += "&count=" + count;

            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            try
            {
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                return PostEntity.Parse(responseContent, userAccountEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<List<PostEntity>> GetConversation(PostEntity startingPost,
            UserAccountEntity userAccountEntity)
        {
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await AuthenticationManager.RefreshAccessToken(userAccountEntity);
            }
            var conversationList = new List<PostEntity>();
            conversationList.Add(startingPost);
            if (startingPost.InReplyToStatusID == 0) return conversationList;
            PostEntity nextEntry =
                await StatusManager.GetPost(false, false, startingPost.InReplyToStatusID, userAccountEntity);
            conversationList.Add(nextEntry);
            while (nextEntry.InReplyToStatusID != 0)
            {
                nextEntry =
                    await StatusManager.GetPost(false, false, nextEntry.InReplyToStatusID, userAccountEntity);
                conversationList.Add(nextEntry);
            }

            return conversationList;
        }
    }
}