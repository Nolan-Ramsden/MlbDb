using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace MlbDb.Scrapers
{
    static class ScoreboardScraper
    {
        static string BaseUrl = "http://gd2.mlb.com/components/game/mlb/year_{year}/month_{month}/day_{day}/master_scoreboard.json";

        public static async Task<JObject> GetScoreboard(WebRequester requester, DateTime date)
        {
            string url = CreateUrl(date);
            var result = await requester.RequestJson<JObject>(url);
            return SanitizeResult(result);
        }

        static string CreateUrl(DateTime date)
        {
            return BaseUrl
                .Replace("{year}", date.Year.ToString().PadLeft(4, '0'))
                .Replace("{month}", date.Month.ToString().PadLeft(2, '0'))
                .Replace("{day}", date.Day.ToString().PadLeft(2, '0'));
        }

        static JObject SanitizeResult(JObject result)
        {
            return result;
        }
    }
}