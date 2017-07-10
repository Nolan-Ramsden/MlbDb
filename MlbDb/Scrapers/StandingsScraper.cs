using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace MlbDb.Scrapers
{
    static class StandingsScraper
    {
        static string BaseUrl = "http://mlb.mlb.com/lookup/json/named.historical_standings_schedule_date.bam?season={year}&game_date=%27{year}/{month}/{day}%27&sit_code=%27h0%27&league_id=103&league_id=104&all_star_sw=%27N%27&version=48";

        public static async Task<JObject> GetStandings(WebRequester requester, DateTime date)
        {
            // use yesterday's midnight, since we want standings as of that date, rather than future
            date = date.AddDays(-1);
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
