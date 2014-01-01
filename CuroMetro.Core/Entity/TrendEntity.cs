using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CrouMetro.Core.Entity
{
    public class TrendEntity
    {
        public String Name { private set; get; }

        public String Query { private set; get; }

        public bool? PromotedContent { private set; get; }

        public static List<TrendEntity> Parse(JArray a)
        {
            var trendList = new List<TrendEntity>();
            if (a != null)
            {
                trendList.AddRange(from JObject o in a
                                   select new TrendEntity
                                   {
                                       Name = (String)o["name"] ?? string.Empty,
                                       Query = (String)o["query"] ?? string.Empty,
                                       PromotedContent = o["promoted_content"] != null && (bool)o["promoted_content"]
                                   });
            }
            return trendList;
        }
    }
}
