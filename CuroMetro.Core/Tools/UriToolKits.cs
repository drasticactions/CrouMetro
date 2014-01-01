using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CrouMetro.Core.Tools
{
    public static class UriToolKits
    {
        private static readonly Regex QueryStringRegex = new Regex(@"[\?&](?<name>[^&=]+)=(?<value>[^&=]+)");


        public static IEnumerable<KeyValuePair<string, string>> ParseQueryString(this string uri)
        {
            if (uri == null)
                throw new ArgumentException("uri");

            MatchCollection matches = QueryStringRegex.Matches(uri);
            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                yield return new KeyValuePair<string, string>(match.Groups["name"].Value, match.Groups["value"].Value);
            }
        }
    }
}