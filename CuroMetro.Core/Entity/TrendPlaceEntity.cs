using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CrouMetro.Core.Entity
{
    public class TrendPlaceEntity
    {
        public String CreatedDate { private set; get; }

        public String TrendAsOfDate { private set; get; }

        public TrendLocationEntity Locations { private set; get; }

        public List<TrendEntity> Trends { private set; get; }

        public static TrendPlaceEntity Parse(String json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            JObject o = JObject.Parse(json);
            var entity = new TrendPlaceEntity
            {
                CreatedDate = ToDate(FixTime((String)o["created_at"])),
                TrendAsOfDate = (String)o["as_of"],
                Locations = (JObject)o["locations"] == null
                    ? null
                    : TrendLocationEntity.Parse((JObject)o["locations"]),
                Trends = TrendEntity.Parse((JArray)o["trends"])
            };
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
