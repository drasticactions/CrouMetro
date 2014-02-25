using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Tools;

namespace CrouMetro.Core.Managers
{
    public class SpreadManager
    {
        public static async Task<HttpStatusCode> CreateSpread(long id, bool? trim, UserAccountEntity userAccountEntity)
        {
            var param = new Dictionary<String, String>();
            if (trim == true) param.Add("trim_user", trim.ToString());
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, (String.Format(EndPoints.SPREAD_CREATE, id)));

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
                return response.StatusCode;
            }
            catch (Exception)
            {
                return HttpStatusCode.BadRequest;
            }
        }
    }
}