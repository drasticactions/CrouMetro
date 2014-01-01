using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diabetus.Entity
{
    public class MessageEntity
    {
        public long CreatedAt { private set; get; }

        public String CreatedDate { private set; get; }

        public long StatusId { private set; get; }

        public UserEntity Recipient { private set; get; }

        public long RecipientId { private set; get; }

        public String RecipientScreenName { private set; get; }

        public UserEntity Sender { private set; get; }

        public long SenderId { private set; get; }

        public String SenderScreenName { private set; get; }

        public String Text { private set; get; }

        private MessageEntity()
        {
        }

        public static List<MessageEntity> Parse(String json, UserAccountEntity userAccountEntity)
        {
            List<MessageEntity> entity = new List<MessageEntity>();
            JArray a = JArray.Parse(json);
            foreach (JObject o in a)
            {
                MessageEntity message = new MessageEntity();
                message.CreatedAt = FixTime((String)o["created_at"]);
                message.CreatedDate = ToDate(message.CreatedAt);
                message.Recipient = UserEntity.Parse(((JObject)o["recipient"]).ToString(), userAccountEntity);
                message.RecipientId = long.Parse((String)o["recipient_id"]);
                message.RecipientScreenName = (String)o["recipient_screen_name"];
                message.Sender = UserEntity.Parse(((JObject)o["sender"]).ToString(), userAccountEntity);
                message.SenderId = long.Parse((String)o["sender_id"]);
                message.SenderScreenName = (String)o["sender_screen_name"];
                message.StatusId = long.Parse((String)o["id"]);
                message.StatusIdStr = (String)o["id_str"];
                message.Text = (String)o["text"];

                entity.Add(message);
            }

            return entity;
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

            DateTime date = new DateTime(int.Parse(year), FixMonth(mon), int.Parse(day), int.Parse(hour), int.Parse(min), int.Parse(sec), DateTimeKind.Local);
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
            DateTime time = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unix), ti);
            return time.ToString();
        }
    }
}
