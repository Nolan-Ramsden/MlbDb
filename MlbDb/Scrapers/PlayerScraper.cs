using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace MlbDb.Scrapers
{
    static class PlayerScraper
    {
        static string BatterUrl = "http://gd2.mlb.com/{gameDir}/batters/{id}.xml";
        static string PitcherUrl = "http://gd2.mlb.com/{gameDir}/pitchers/{id}.xml";

        public static async Task<JObject> GetBatter(WebRequester requester, string gameDir, int id)
        {
            string url = CreateUrl(BatterUrl, gameDir, id);
            var result = await requester.RequestXml<JObject>(url);
            return SanitizeBatterResult(result);
        }

        public static async Task<JObject> GetPitcher(WebRequester requester, string gameDir, int id)
        {
            string url = CreateUrl(PitcherUrl, gameDir, id);
            var result = await requester.RequestXml<JObject>(url);
            return SanitizePitcherResult(result);
        }

        static string CreateUrl(string baseUrl, string gameDir, int id)
        {
            return baseUrl
                .Replace("{gameDir}", gameDir)
                .Replace("{id}", id.ToString());
        }

        static JObject SanitizeBatterResult(JObject result)
        {
            return result;
        }

        static JObject SanitizePitcherResult(JObject result)
        {
            return result;
        }
    }
}