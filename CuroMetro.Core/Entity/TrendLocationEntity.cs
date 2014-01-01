using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CrouMetro.Core.Entity
{
    public class TrendLocationEntity
    {
        public String Name { private set; get; }

        public int WoeId { private set; get; }

        public static TrendLocationEntity Parse(JObject o)
        {
            var trendLocation = new TrendLocationEntity
            {
                Name = (String)o["name"] ?? string.Empty,
                WoeId = o["woeid"] != null ? (int)o["woeid"] : 0
            };

            return trendLocation;
        }
    }
}
