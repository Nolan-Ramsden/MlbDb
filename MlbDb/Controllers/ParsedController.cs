using Microsoft.Practices.Unity;
using MlbDb.Models;
using MlbDb.Parsers;
using MlbDb.Scrapers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace MlbDb.Controllers
{
    [RoutePrefix("api/v1/parsed")]
    public class ParsedController : ApiController
    {
        [Dependency]
        public MlbDataParser Parser { get; set; }

        [Dependency]
        public MlbDataScraper Scraper { get; set; }

        [HttpGet]
        [Route("scoreboards")]
        public async Task<Scoreboard> GetScoreboard(DateTime? date = null)
        {
            if (!date.HasValue)
            {
                date = DateTime.Now;
            }
            JObject scoreboard = await Scraper.GetScoreboard(date.Value);
            return Parser.ParseScoreboard(scoreboard);
        }

        [HttpGet]
        [Route("standings")]
        public async Task<Dictionary<int, TeamAppearance>> GetStandings(DateTime? date = null)
        {
            if (!date.HasValue)
            {
                date = DateTime.Now;
            }
            JObject standings = await Scraper.GetStandings(date.Value);
            return Parser.ParseStandings(standings);
        }

        [HttpGet]
        [Route("boxscores")]
        public async Task<Game> GetBoxscore(string gameDir)
        {
            JObject boxscore = await Scraper.GetBoxscore(gameDir);
            return Parser.ParseBoxscore(boxscore);
        }

        [HttpGet]
        [Route("batters")]
        public async Task<BatterAppearance> GetBatter(string gameDir, int id)
        {
            JObject batter = await Scraper.GetBatter(gameDir, id);
            return Parser.ParseBatter(batter);
        }

        [HttpGet]
        [Route("pitchers")]
        public async Task<PitcherAppearance> GetPitcher(string gameDir, int id)
        {
            JObject pitcher = await Scraper.GetPitcher(gameDir, id);
            return Parser.ParsePitcher(pitcher);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Scraper.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
