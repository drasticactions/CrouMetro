using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace CrouMetro.Core.Entity
{
    public class UserEntity
    {
        private UserEntity()
        {
        }

        public long CreatedAt { private set; get; }

        public String Description { private set; get; }

        public long FavoritesCount { private set; get; }

        public Boolean IsFollowRequest { private set; get; }

        public long FollowersCount { private set; get; }

        public Boolean IsFollowing { private set; get; }

        public long FriendsCount { private set; get; }

        public long UserID { private set; get; }

        public String Location { private set; get; }

        public String Name { private set; get; }

        public String ProfileImage { private set; get; }

        public Boolean IsProtected { private set; get; }

        public String ScreenName { private set; get; }

        public long StatusCount { private set; get; }

        public String URL { private set; get; }

        public bool IsCurrentUser { private set; get; }

        public bool IsNotCurrentUser { private set; get; }

        public bool IsNotFollowing { private set; get; }

        public static List<UserEntity> ParseUserList(String json, UserAccountEntity userAccountEntity)
        {
            var userEntities = new List<UserEntity>();
            JArray a = JArray.Parse(json);
            foreach (JToken jToken in a)
            {
                var o = (JObject) jToken;
                var user = new UserEntity
                {
                    CreatedAt = (String) o["created_at"] == null ? 0 : FixTime((String) o["created_at"]),
                    Description = (String) o["description"] ?? string.Empty,
                    FavoritesCount =
                        (String) o["favorites_count"] == null ? 0 : long.Parse((String) o["favorites_count"]),
                    FollowersCount =
                        (String) o["followers_count"] == null ? 0 : long.Parse((String) o["followers_count"]),
                    FriendsCount = (String) o["friends_count"] == null ? 0 : long.Parse((String) o["friends_count"]),
                    IsFollowing =
                        (String) o["following"] != null &&
                        (!((String) o["following"]).Equals("underdevelopment") && Boolean.Parse((String) o["following"])),
                    IsFollowRequest =
                        (String) o["follow_request_sent"] != null &&
                        (!((String) o["follow_request_sent"]).Equals("underdevelopment") &&
                         Boolean.Parse((String) o["follow_request_sent"])),
                    IsProtected = (String) o["protected"] != null && Boolean.Parse((String) o["protected"]),
                    Location = (String) o["location"] ?? string.Empty,
                    Name = Regex.Replace((String) o["name"], @"\t|\n|\r", ""),
                    ProfileImage = (String) o["profile_image_url_https"],
                    ScreenName = (String) o["screen_name"],
                    StatusCount = (String) o["statuses_count"] == null ? 0 : long.Parse((String) o["statuses_count"]),
                    URL = (String) o["url"] ?? string.Empty,
                    UserID = long.Parse((String) o["id"])
                };
                user.IsNotFollowing = !user.IsFollowing;
                if (userAccountEntity != null && userAccountEntity.GetUserEntity() != null)
                {
                    user.IsCurrentUser = user.UserID == userAccountEntity.GetUserEntity().UserID;
                    user.IsNotCurrentUser = !user.IsCurrentUser;
                }

                userEntities.Add(user);
            }
            return userEntities;
        }

        public static UserEntity Parse(String json, UserAccountEntity userAccountEntity)
        {
            JObject o = JObject.Parse(json);
            var user = new UserEntity
            {
                CreatedAt = (String) o["created_at"] == null ? 0 : FixTime((String) o["created_at"]),
                Description = (String) o["description"] ?? string.Empty,
                FavoritesCount = (String) o["favorites_count"] == null ? 0 : long.Parse((String) o["favorites_count"]),
                FollowersCount = (String) o["followers_count"] == null ? 0 : long.Parse((String) o["followers_count"]),
                FriendsCount = (String) o["friends_count"] == null ? 0 : long.Parse((String) o["friends_count"]),
                IsFollowing =
                    (String) o["following"] != null &&
                    (!((String) o["following"]).Equals("underdevelopment") && Boolean.Parse((String) o["following"])),
                IsFollowRequest =
                    (String) o["follow_request_sent"] != null &&
                    (!((String) o["follow_request_sent"]).Equals("underdevelopment") &&
                     Boolean.Parse((String) o["follow_request_sent"])),
                IsProtected = (String) o["protected"] != null && Boolean.Parse((String) o["protected"]),
                Location = (String) o["location"] ?? string.Empty,
                Name = Regex.Replace((String) o["name"], @"\t|\n|\r", ""),
                ProfileImage = (String) o["profile_image_url_https"],
                ScreenName = (String) o["screen_name"],
                StatusCount = (String) o["statuses_count"] == null ? 0 : long.Parse((String) o["statuses_count"]),
                URL = (String) o["url"] ?? string.Empty,
                UserID = long.Parse((String) o["id"])
            };
            user.IsNotFollowing = !user.IsFollowing;
            if (userAccountEntity != null && userAccountEntity.GetUserEntity() != null)
            {
                user.IsCurrentUser = user.UserID == userAccountEntity.GetUserEntity().UserID;
                user.IsNotCurrentUser = !user.IsCurrentUser;
            }
            return user;
        }

        private static long FixTime(String time)
        {
            String[] times = time.Split(' ');
            String day = times[1];
            String mon = times[2];
            String year = times[3];
            String hour = times[4].Split(':')[0];
            String min = times[4].Split(':')[1];
            String sec = times[4].Split(':')[2];
            String utf = times[5];

            var date = new DateTime(int.Parse(year), FixMonth(mon), int.Parse(day), int.Parse(hour), int.Parse(min),
                int.Parse(sec), DateTimeKind.Local);
            return UserAccountEntity.GetUnixTime(date);
        }

        private static int FixMonth(String m)
        {
            switch (m)
            {
                case "Jan":
                    return 1;
                case "Feb":
                    return 2;
                case "Mar":
                    return 3;
                case "Apr":
                    return 4;
                case "May":
                    return 5;
                case "Jun":
                    return 6;
                case "Jul":
                    return 7;
                case "Aug":
                    return 8;
                case "Sep":
                    return 9;
                case "Oct":
                    return 10;
                case "Nov":
                    return 11;
                case "Dec":
                    return 12;
                default:
                    return 1;
            }
        }
    }
}