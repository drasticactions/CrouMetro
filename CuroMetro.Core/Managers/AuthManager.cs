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
        private const string KEY_TOKENEXIST = "TOKEN_DOES_EXIST";
        private const string KEY_TOKENVALUE = "TOKEN_VALUE";
        private static readonly IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;

        /// <summary>
        ///     新しくアクセストークンを取得します。
        ///     また、Accountオブジェクトを新規に作成します。
        /// </summary>
        /// <param name="code">認可コード</param>
        /// <param name="account">対象のアカウント</param>
        public static async Task<bool> RequestAccessToken(String code)
        {
            var dic = new Dictionary<String, String>();
            dic["grant_type"] = "authorization_code";
            dic["client_id"] = Constants.CONSUMER_KEY;
            dic["client_secret"] = Constants.CONSUMER_SECRET;
            dic["code"] = code;
            //String response;

            //UserAccountEntity account = null;
            var theAuthClient = new HttpClient();
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, EndPoints.OAUTH_TOKEN);
            HttpContent header = new FormUrlEncodedContent(dic);
            HttpResponseMessage response = await theAuthClient.PostAsync(EndPoints.OAUTH_TOKEN, header);
            string responseContent = await response.Content.ReadAsStringAsync();
            //Http.HttpPost(EndPoints.OAUTH_TOKEN, dic, ref account, out response);

            CroudiaAuthEntity authEntity = CroudiaAuthEntity.Parse(responseContent);


            if (!appSettings.Any())
            {
                appSettings.Add("accessToken", authEntity.AccessToken);
            }
            else
            {
                appSettings["accessToken"] = authEntity.AccessToken;
            }

            if (!appSettings.Any())
            {
                appSettings.Add("refreshToken", authEntity.RefreshToken);
            }
            else
            {
                appSettings["refreshToken"] = authEntity.RefreshToken;
            }

            appSettings.Save();
            return true;
        }

        /// <summary>
        ///     アクセストークンを更新します。
        /// </summary>
        /// <param name="account">対象のアカウント</param>
        public static async Task<bool> RefreshAccessToken(UserAccountEntity account)
        {
            //var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var dic = new Dictionary<String, String>();
            dic["grant_type"] = "refresh_token";
            dic["client_id"] = Constants.CONSUMER_KEY;
            dic["client_secret"] = Constants.CONSUMER_SECRET;
            dic["refresh_token"] = account.GetRefreshToken();

            account.SetAccessToken("updating", null);
            account.SetRefreshTime(1000);
            //String response;
            var theAuthClient = new HttpClient();
            HttpContent header = new FormUrlEncodedContent(dic);
            HttpResponseMessage response = await theAuthClient.PostAsync(EndPoints.OAUTH_TOKEN, header);
            string responseContent2 = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                JObject o = JObject.Parse(responseContent);
                account.SetAccessToken((String) o["access_token"], (String) o["refresh_token"]);
                account.SetRefreshTime(long.Parse((String) o["expires_in"]));

                CroudiaAuthEntity authEntity = CroudiaAuthEntity.Parse(responseContent);
                appSettings["refreshToken"] = authEntity.RefreshToken;
                appSettings["accessToken"] = authEntity.AccessToken;
                appSettings.Save();
                //Auth.SaveUserCredentials(account);
                return true;
            }
            return false;
        }

        public static async Task<bool> VerifyAccount(UserAccountEntity userAccountEntity)
        {
            var theAuthClient = new HttpClient();
            var requestMsg = new HttpRequestMessage(new HttpMethod("GET"), EndPoints.ACCOUNT_VERIFY);
            requestMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer",
                userAccountEntity.GetAccessToken());
            HttpResponseMessage response = await theAuthClient.SendAsync(requestMsg);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                userAccountEntity.SetUserEntity(UserEntity.Parse(responseContent, userAccountEntity));
                SaveUserCredentials(userAccountEntity);
                return true;
            }
            return false;
        }

        public static void SaveUserCredentials(UserAccountEntity userAccountEntity)
        {
            appSettings["refreshToken"] = userAccountEntity.GetRefreshToken();
            appSettings["accessToken"] = userAccountEntity.GetAccessToken();
            appSettings.Save();
        }

        public static void SaveClientToken(string responseData)
        {
            if ((string) appSettings["accessToken"] == null)
            {
                appSettings.Add("accessToken", responseData);
            }
            else
            {
                appSettings["accessToken"] = responseData;
            }
            appSettings.Save();
        }

        public static string GetClientToken()
        {
            string token = string.Empty;

            try
            {
                token = (string) appSettings["accessToken"];
            }
            catch (KeyNotFoundException e)
            {
                token = "";
            }

            return token;
        }
    }
}