using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace CrouMetro.Core.Entity
{
    public class PostEntity
    {
        private PostEntity()
        {
        }

        public long CreatedAt { private set; get; }

        public DateTime CreatedDate { private set; get; }

        public Boolean IsFavorited { private set; get; }

        public Boolean CanBeFavorited { private set; get; }

        public long FavoritedCount { private set; get; }

        public long StatusID { private set; get; }

        public String InReplyToScreenName { private set; get; }

        public long InReplyToStatusID { private set; get; }

        public string HasMediaString { private set; get; }

        public bool HasMedia { private set; get; }

        public long InReplyToUserID { private set; get; }

        public long SpreadCount { private set; get; }

        public Boolean IsSpreaded { private set; get; }

        public Boolean CanBeSpread { private set; get; }

        public PostEntity SpreadStatus { private set; get; }

        public PostEntity ReplyStatus { private set; get; }

        public String Post { private set; get; }


        public UserEntity User { private set; get; }


        public String SourceName { private set; get; }

        public String SourceUrl { private set; get; }

        public String ImageUrl { private set; get; }

        public String ImageLargeUrl { private set; get; }

        public String VideoUrl { private set; get; }

        public String VideoId { private set; get; }

        public IEnumerable<String> Links { private set; get; }

        public String Media { private set; get; }

        public List<HyperlinkButton> hyperLinkSet { private set; get; }

        public bool IsCreator { private set; get; }

        public bool IsNotCreator { private set; get; }

        public SolidColorBrush ReplyQuoteColor { private set; get; }

        public string SpreadBy { private set; get; }

        public static List<PostEntity> Parse(String json, UserAccountEntity userAccountEntity)
        {
            var entity = new List<PostEntity>();
            JArray a = JArray.Parse(json);
            foreach (var jToken in a)
            {
                var o = (JObject) jToken;
                var post = new PostEntity
                {
                    CreatedAt = FixTime((String) o["created_at"]),
                    FavoritedCount = long.Parse((String) o["favorited_count"]),
                    InReplyToScreenName = (String) o["in_reply_to_screen_name"],
                    InReplyToStatusID =
                        long.Parse(String.IsNullOrEmpty((String) o["in_reply_to_status_id"])
                            ? "0"
                            : (String) o["in_reply_to_status_id"]),
                    InReplyToUserID =
                        long.Parse(String.IsNullOrEmpty((String) o["in_reply_to_user_id"])
                            ? "0"
                            : (String) o["in_reply_to_user_id"]),
                    IsFavorited = Boolean.Parse((String) o["favorited"]),
                    IsSpreaded = Boolean.Parse((String) o["spread"]),
                    CanBeFavorited = !Boolean.Parse((String) o["favorited"]),
                    CanBeSpread = !Boolean.Parse((String) o["spread"]),
                    ReplyStatus =
                        (JObject) o["reply_status"] == null
                            ? null
                            : ParsePost((JObject) o["reply_status"], userAccountEntity),
                    SourceName = (String) o["source"]["name"],
                    SourceUrl = (String) o["source"]["url"],
                    SpreadCount = long.Parse((String) o["spread_count"]),
                    SpreadStatus =
                        (JObject) o["spread_status"] == null
                            ? null
                            : ParsePost((JObject) o["spread_status"], userAccountEntity),
                    StatusID = long.Parse((String) o["id"]),
                    Post = (String) o["text"],
                    User = UserEntity.Parse(o["user"].ToString(), userAccountEntity),
                    ImageUrl =
                        (JObject) o["entities"]["media"] == null
                            ? null
                            : ParseEntities((JObject) o["entities"], "media_url_https"),
                    ImageLargeUrl =
                        (JObject) o["entities"]["media"] == null
                            ? null
                            : ParseEntities((JObject) o["entities"], "media_url_https") + "?large",
                    Media =
                        (JObject) o["entities"]["media"] == null ? null : ParseEntities((JObject) o["entities"], "type")
                };
                post.Links =
                    post.Post.Split("\t\n ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .Where(s => s.StartsWith("http://") || s.StartsWith("www."));
                post.IsCreator = post.User.UserID == userAccountEntity.GetUserEntity().UserID;
                post.CreatedDate = ToDate(post.CreatedAt);
                post.IsNotCreator = !post.IsCreator;
                if (post.Media != null)
                {
                    if (post.Media.Equals("photo"))
                    {
                        post.HasMediaString = "画像 ";
                        post.HasMedia = true;
                    }
                }
                if (post.SpreadStatus != null)
                {
                    post.SpreadStatus.SpreadBy = string.Format("{0}さんがイイネ！しました。", post.User.Name);
                    post.SpreadStatus.IsSpreaded = true;
                    entity.Add(post.SpreadStatus);
                }
                else
                {
                    entity.Add(post);
                }
            }

            return entity;
        }

        private static String ParseEntities(JObject o1, String obj)
        {
            var o = (JObject) o1["media"];
            return o[obj].ToString();
        }

        public static PostEntity ParsePost(JObject o, UserAccountEntity userAccountEntity)
        {
            var post = new PostEntity
            {
                CreatedAt = FixTime((String) o["created_at"]),
                FavoritedCount = long.Parse((String) o["favorited_count"]),
                InReplyToScreenName = (String) o["in_reply_to_screen_name"],
                InReplyToStatusID =
                    long.Parse(String.IsNullOrEmpty((String) o["in_reply_to_status_id"])
                        ? "0"
                        : (String) o["in_reply_to_status_id"]),
                InReplyToUserID =
                    long.Parse(String.IsNullOrEmpty((String) o["in_reply_to_user_id"])
                        ? "0"
                        : (String) o["in_reply_to_user_id"]),
                IsFavorited = Boolean.Parse((String) o["favorited"]),
                IsSpreaded = Boolean.Parse((String) o["spread"]),
                ReplyStatus =
                    (JObject) o["reply_status"] == null
                        ? null
                        : ParsePost((JObject) o["reply_status"], userAccountEntity),
                SourceName = (String) o["source"]["name"],
                SourceUrl = (String) o["source"]["url"],
                SpreadCount = long.Parse((String) o["spread_count"]),
                SpreadStatus =
                    (JObject) o["spread_status"] == null
                        ? null
                        : ParsePost((JObject) o["spread_status"], userAccountEntity),
                StatusID = long.Parse((String) o["id"]),
                Post = (String) o["text"],
                User = UserEntity.Parse(o["user"].ToString(), userAccountEntity),
                ImageUrl =
                    (JObject) o["entities"]["media"] == null
                        ? null
                        : ParseEntities((JObject) o["entities"], "media_url_https"),
                ImageLargeUrl =
                    (JObject) o["entities"]["media"] == null
                        ? null
                        : ParseEntities((JObject) o["entities"], "media_url_https") + "?large",
                Media = (JObject) o["entities"]["media"] == null ? null : ParseEntities((JObject) o["entities"], "type")
            };
            if (post.User != null)
            {
                post.IsCreator = post.User.UserID == userAccountEntity.GetUserEntity().UserID;
            }
            return post;
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

        private static DateTime ToDate(long unix)
        {
            TimeZoneInfo ti = TimeZoneInfo.Local;
            DateTime time =
                TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unix), ti);
            return time;
        }

        public PostEntity Clone()
        {
            var entity = new PostEntity
            {
                CreatedAt = CreatedAt,
                CreatedDate = CreatedDate,
                FavoritedCount = FavoritedCount,
                ImageUrl = ImageUrl,
                InReplyToScreenName = InReplyToScreenName,
                InReplyToStatusID = InReplyToStatusID,
                InReplyToUserID = InReplyToUserID,
                IsFavorited = IsFavorited,
                IsSpreaded = IsSpreaded,
                Media = Media,
                ReplyStatus = ReplyStatus,
                SourceName = SourceName,
                SourceUrl = SourceUrl,
                SpreadCount = SpreadCount,
                SpreadStatus = SpreadStatus,
                StatusID = StatusID,
                Post = Post,
                User = User
            };
            return entity;
        }
    }
}