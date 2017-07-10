using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Threading.Tasks;

namespace MlbDb.Scrapers
{
    public class MlbDataScraper : IDisposable
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected WebRequester Requester = new WebRequester();

        public async Task<JObject> GetScoreboard(DateTime date)
        {
            Logger.Debug("Request for scoreboard on {0}", date);
            return await ScoreboardScraper.GetScoreboard(Requester, date);
        }

        public async Task<JObject> GetBoxscore(string gameDir)
        {
            Logger.Debug("Request for boxscore from {0}", gameDir);
            return await BoxscoreScraper.GetBoxscore(Requester, gameDir);
        }

        public async Task<JObject> GetStandings(DateTime date)
        {
            Logger.Debug("Request for standings on {0}", date);
            return await StandingsScraper.GetStandings(Requester, date);
        }

        public async Task<JObject> GetBatter(string gameDir, int id)
        {
            Logger.Debug("Request for batter {0}", id);
            return await PlayerScraper.GetBatter(Requester, gameDir, id);
        }

        public async Task<JObject> GetPitcher(string gameDir, int id)
        {
            Logger.Debug("Request for pitcher {0}", id);
            return await PlayerScraper.GetPitcher(Requester, gameDir, id);
        }

        public void Dispose()
        {
            Requester.Dispose();
        }
    }
}