using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace CrouMetro.Core.Entity
{
    public class SecretMailEntity
    {
        public long CreatedAt { private set; get; }

        public String CreatedDate { private set; get; }
        public String IdStr { private set; get; }

        public long ID { private set; get; }

        public long RecipientId { private set; get; }

        public UserEntity Recipient { private set; get; }

        public String RecipientScreenName { private set; get; }

        public long SenderId { private set; get; }

        public UserEntity Sender { private set; get; }

        public String SenderScreenName { private set; get; }

        public String MessageContent { private set; get; }

        public static List<SecretMailEntity> Parse(String json, UserAccountEntity userAccountEntity)
        {
            var entity = new List<SecretMailEntity>();
            JArray a = JArray.Parse(json);
            foreach (JToken jToken in a)
            {
                var o = (JObject) jToken;
                var message = new SecretMailEntity
                {
                    CreatedAt = FixTime((String) o["created_at"]),
                    Recipient = UserEntity.Parse(o["recipient"].ToString(), userAccountEntity),
                    RecipientScreenName = (String) o["recipient_screen_name"],
                    RecipientId = long.Parse((String) o["recipient_id"]),
                    Sender = UserEntity.Parse(o["sender"].ToString(), userAccountEntity),
                    SenderScreenName = (String) o["sender_screen_name"],
                    SenderId = long.Parse((String) o["sender_id"]),
                    MessageContent = (String) o["text"],
                    ID = long.Parse((String) o["id"]),
                    IdStr = (String) o["id_str"]
                };
                message.CreatedDate = ToDate(message.CreatedAt);
                entity.Add(message);
            }

            return entity;
        }

        public static SecretMailEntity ParseMail(JObject o, UserAccountEntity userAccountEntity)
        {
            var message = new SecretMailEntity
            {
                CreatedAt = FixTime((String) o["created_at"]),
                Recipient = UserEntity.Parse(o["recipient"].ToString(), userAccountEntity),
                RecipientScreenName = (String) o["recipient_screen_name"],
                RecipientId = long.Parse((String) o["recipient_id"]),
                Sender = UserEntity.Parse(o["sender"].ToString(), userAccountEntity),
                SenderScreenName = (String) o["sender_screen_name"],
                SenderId = long.Parse((String) o["sender_id"]),
                MessageContent = (String) o["text"],
                ID = long.Parse((String) o["id"]),
                IdStr = (String) o["id_str"]
            };
            message.CreatedDate = ToDate(message.CreatedAt);
            return message;
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

        private static String ToDate(long unix)
        {
            TimeZoneInfo ti = TimeZoneInfo.Local;
            DateTime time =
                TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unix), ti);
            return time.ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
}