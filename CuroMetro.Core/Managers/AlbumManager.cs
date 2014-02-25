using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CrouMetro.Core.Entity;
using HtmlAgilityPack;

namespace CrouMetro.Core.Managers
{
    public class AlbumManager
    {
        public static async Task<List<MediaEntity>> GetAlbumList(int offSet, String screenName,
            UserAccountEntity userAccountEntity)
        {
            if (userAccountEntity.GetAccessToken().Equals("refresh"))
            {
                await AuthenticationManager.RefreshAccessToken(userAccountEntity);
            }
            var doc = new HtmlDocument();
            var theAuthClient = new HttpClient();
            var requestMsg = new HttpRequestMessage(new HttpMethod("GET"),
                "https://croudia.com/voices/album/" + screenName + "?offset=" + offSet);
            HttpResponseMessage response = await theAuthClient.SendAsync(requestMsg);
            string responseContent = await response.Content.ReadAsStringAsync();
            responseContent = RemoveHtmlComments(responseContent);
            doc.LoadHtml(responseContent);
            List<long> idList =
                doc.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "").Contains("contents"))
                    .Select(link => ConvertToLong(link.GetAttributeValue("id", "")))
                    .ToList();
            List<long> imageList =
                doc.DocumentNode.Descendants("img")
                    .Where(node => node.GetAttributeValue("src", "").Contains("?large"))
                    .Select(link => ExtractId(link.GetAttributeValue("src", "")))
                    .ToList();
            List<string> postList = doc.DocumentNode.Descendants("p").Select(link => link.InnerText).ToList();
            return
                imageList.Select(t => "https://croudia.com/testimages/download/" + t)
                    .Select((url, i) => new MediaEntity(idList[i], url, postList[i], "", i))
                    .ToList();
        }

        private static string RemoveHtmlComments(string input)
        {
            string output = string.Empty;
            string[] temp = Regex.Split(input, "<!--");
            return (from s in temp
                let str = string.Empty
                select !s.Contains("-->") ? s : s.Substring(s.IndexOf("-->", StringComparison.Ordinal) + 3)
                into str
                where str.Trim() != string.Empty
                select str).Aggregate(output, (current, str) => current + str.Trim());
        }

        private static long ConvertToLong(string txt)
        {
            return (long) Convert.ToDouble(txt);
        }

        private static long ExtractId(string txt)
        {
            string re1 = ".*?"; // Non-greedy match on filler
            string re2 = "(\\d+)"; // Integer Number 1
            String int1 = string.Empty;
            var r = new Regex(re1 + re2, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(txt);
            if (m.Success)
            {
                int1 = m.Groups[1].ToString();
            }
            return (long) Convert.ToDouble(int1);
        }
    }
}