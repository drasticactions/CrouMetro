using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Tools;
using Newtonsoft.Json.Linq;

namespace CrouMetro.Core.Managers
{
    public class Auth
    {
        private static readonly IsolatedStorageSettings AppSettings = IsolatedStorageSettings.ApplicationSettings;

        /// <summary>
        ///     新しくアクセストークンを取得します。
        ///     また、Accountオブジェクトを新規に作成します。
        /// </summary>
        /// <param name="code">認可コード</param>
        public static async Task<bool> RequestAccessToken(String code)
        {
            var dic = new Dictionary<String, String>();
            dic["grant_type"] = "authorization_code";
            dic["client_id"] = Constants.CONSUMER_KEY;
            dic["client_secret"] = Constants.CONSUMER_SECRET;
            dic["code"] = code;

            var theAuthClient = new HttpClient();
            HttpContent header = new FormUrlEncodedContent(dic);
            HttpResponseMessage response = await theAuthClient.PostAsync(EndPoints.OAUTH_TOKEN, header);
            string responseContent = await response.Content.ReadAsStringAsync();

            CroudiaAuthEntity authEntity = CroudiaAuthEntity.Parse(responseContent);


            if (!AppSettings.Any())
            {
                AppSettings.Add("accessToken", authEntity.AccessToken);
            }
            else
            {
                AppSettings["accessToken"] = authEntity.AccessToken;
            }

            if (!AppSettings.Any())
            {
                AppSettings.Add("refreshToken", authEntity.RefreshToken);
            }
            else
            {
                AppSettings["refreshToken"] = authEntity.RefreshToken;
            }

            AppSettings.Save();
            return true;
        }

        /// <summary>
        ///     アクセストークンを更新します。
        /// </summary>
        /// <param name="account">対象のアカウント</param>
        public static async Task<bool> RefreshAccessToken(UserAccountEntity account)
        {
            var dic = new Dictionary<String, String>();
            dic["grant_type"] = "refresh_token";
            dic["client_id"] = Constants.CONSUMER_KEY;
            dic["client_secret"] = Constants.CONSUMER_SECRET;
            dic["refresh_token"] = account.GetRefreshToken();

            account.SetAccessToken("updating", null);
            account.SetRefreshTime(1000);
            var theAuthClient = new HttpClient();
            HttpContent header = new FormUrlEncodedContent(dic);
            HttpResponseMessage response;
            try
            {
                response = await theAuthClient.PostAsync(EndPoints.OAUTH_TOKEN, header);
            }
            catch (WebException)
            {
                return false;
            }
            if (response.StatusCode != HttpStatusCode.OK) return false;
            string responseContent = await response.Content.ReadAsStringAsync();
            JObject o = JObject.Parse(responseContent);
            account.SetAccessToken((String) o["access_token"], (String) o["refresh_token"]);
            account.SetRefreshTime(long.Parse((String) o["expires_in"]));

            CroudiaAuthEntity authEntity = CroudiaAuthEntity.Parse(responseContent);
            AppSettings["refreshToken"] = authEntity.RefreshToken;
            AppSettings["accessToken"] = authEntity.AccessToken;
            AppSettings.Save();
            return true;
        }

        public static async Task<bool> VerifyAccount(UserAccountEntity userAccountEntity)
        {
            var theAuthClient = new HttpClient();
            var requestMsg = new HttpRequestMessage(new HttpMethod("GET"), EndPoints.ACCOUNT_VERIFY);
            requestMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer",
                userAccountEntity.GetAccessToken());
            HttpResponseMessage response;
            try
            {
                response = await theAuthClient.SendAsync(requestMsg);
            }
            catch (WebException)
            {
                return false;
            }
            if (response.StatusCode != HttpStatusCode.OK) return false;
            string responseContent = await response.Content.ReadAsStringAsync();
            userAccountEntity.SetUserEntity(UserEntity.Parse(responseContent, userAccountEntity));
            SaveUserCredentials(userAccountEntity);
            return true;
        }

        public static void SaveUserCredentials(UserAccountEntity userAccountEntity)
        {
            AppSettings["refreshToken"] = userAccountEntity.GetRefreshToken();
            AppSettings["accessToken"] = userAccountEntity.GetAccessToken();
            AppSettings.Save();
        }
    }
}