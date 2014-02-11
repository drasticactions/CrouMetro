using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;

namespace CrouMetro.Core.Entity
{
    public class UserAccountEntity
    {
        private static readonly IsolatedStorageSettings AppSettings = IsolatedStorageSettings.ApplicationSettings;
        private readonly AccountData _data;
        private UserEntity _entity;
        private Boolean _isCalled;

        public UserAccountEntity()
        {
            string accessToken;
            string refreshToken;
            try
            {
                accessToken = (string) AppSettings["accessToken"];
            }
            catch (KeyNotFoundException e)
            {
                accessToken = string.Empty;
            }
            try
            {
                refreshToken = (string) AppSettings["refreshToken"];
            }
            catch (KeyNotFoundException e)
            {
                refreshToken = string.Empty;
            }
            _data = new AccountData(accessToken, refreshToken, 3600);
            _entity = null;
            _isCalled = false;
        }

        public void SetUserEntity(UserEntity entity)
        {
            _entity = entity;
        }

        public UserEntity GetUserEntity()
        {
            return _entity;
        }

        public void SetAccessToken(String token, String refresh)
        {
            _data.AccessToken = token;
            _data.RefreshToken = refresh;
            if (_data.RefreshToken != null)
            {
                _isCalled = false;
            }
        }

        public void SetRefreshTime(long time)
        {
            _data.RefreshTime = time;
            _data.StartTime = GetUnixTime(DateTime.Now);
        }

        public String GetAccessToken()
        {
            if (GetUnixTime(DateTime.Now) - _data.StartTime < _data.RefreshTime) return _data.AccessToken;
            if (_isCalled) return _data.AccessToken;
            _isCalled = true;
            return "refresh";
        }

        public String GetRefreshToken()
        {
            return _data.RefreshToken;
        }

        public static long GetUnixTime(DateTime time)
        {
            time = time.ToUniversalTime();
            TimeSpan timeSpam = time - (new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local));
            return (long) timeSpam.TotalSeconds;
        }

        public override string ToString()
        {
            return GetAccessToken() + ":" + GetRefreshToken();
        }

        private class AccountData
        {
            public String AccessToken;
            public long RefreshTime;
            public String RefreshToken;
            public long StartTime;

            public AccountData(String token, String refresh, int time)
            {
                AccessToken = token;
                RefreshToken = refresh;
                RefreshTime = time;
                StartTime = GetUnixTime(DateTime.Now);
            }
        }
    }
}