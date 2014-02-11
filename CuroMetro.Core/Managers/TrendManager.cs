using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Tools;

namespace CrouMetro.Core.Managers
{
    public class TrendManager
    {
        public static async Task<TrendPlaceEntity> GetTrends(int woeId,
            UserAccountEntity userAccountEntity)
        {
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await Auth.RefreshAccessToken(userAccountEntity);
            }
            var theAuthClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, string.Format(EndPoints.TrendsPlace, woeId));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccountEntity.GetAccessToken());
            try
            {
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                return TrendPlaceEntity.Parse(responseContent);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
