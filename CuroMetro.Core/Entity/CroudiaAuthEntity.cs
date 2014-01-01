using System;
using Newtonsoft.Json.Linq;

namespace CrouMetro.Core.Entity
{
    public class CroudiaAuthEntity
    {
        public string AccessToken { get; private set; }

        public string RefreshToken { get; private set; }

        public long ExpiresIn { get; private set; }

        public string TokenType { get; private set; }

        public static CroudiaAuthEntity Parse(string json)
        {
            var authEntity = new CroudiaAuthEntity();
            JObject o = JObject.Parse(json);
            authEntity.AccessToken = (String) o["access_token"];
            authEntity.RefreshToken = (String) o["refresh_token"];
            authEntity.ExpiresIn = (long) o["expires_in"];
            authEntity.TokenType = (String) o["token_type"];
            return authEntity;
        }
    }
}