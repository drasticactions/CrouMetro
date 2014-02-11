using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Tools;

namespace CrouMetro.Core.Managers
{
    public class FriendshipManager
    {
        public static async Task<bool> DestroyFriendship(long? id, UserAccountEntity userAccountEntity)
        {
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.FRIEND_DESTROY + "?user_id=" + id);

            string accessToken = userAccountEntity.GetAccessToken();
            if (accessToken.Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
                accessToken = userAccountEntity.GetAccessToken();
            }
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response;
            try
            {
                response = await theAuthClient.SendAsync(request);
            }
            catch (WebException)
            {
                return false;
            }
            return response.IsSuccessStatusCode;
        }

        public static async Task<bool> CreateFriendship(long? id, UserAccountEntity userAccountEntity)
        {
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, EndPoints.FRIEND_CREATE + "?user_id=" + id);

            string accessToken = userAccountEntity.GetAccessToken();
            if (accessToken.Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
                accessToken = userAccountEntity.GetAccessToken();
            }
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response;
            try
            {
                response = await theAuthClient.SendAsync(request);
            }
            catch (WebException)
            {
                return false;
            }
            return response.IsSuccessStatusCode;
        }
    }
}