using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace MlbDb.Scrapers
{
    static class BoxscoreScraper
    {
        static string BaseUrl = "http://gd2.mlb.com/{gameDir}/boxscore.json";

        public static async Task<JObject> GetBoxscore(WebRequester requester, string gameDir)
        {
            string url = CreateUrl(gameDir);
            var result = await requester.RequestJson<JObject>(url);
            return SanitizeResult(result);
        }

        static string CreateUrl(string gameDir)
        {
            return BaseUrl
                .Replace("{gameDir}", gameDir);
        }

        static JObject SanitizeResult(JObject result)
        {
            return result;
        }
    }
}