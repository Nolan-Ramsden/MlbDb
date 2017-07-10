using Microsoft.Practices.Unity;
using MlbDb.Scrapers;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace MlbDb.Controllers
{
    [RoutePrefix("api/v1/raw")]
    public class RawController : ApiController
    {
        [Dependency]
        public MlbDataScraper Scraper { get; set; }

        [HttpGet]
        [Route("scoreboards")]
        public async Task<JObject> GetScoreboard(DateTime? date = null)
        {
            if (!date.HasValue)
            {
                date = DateTime.Now;
            }
            return await Scraper.GetScoreboard(date.Value);
        }

        [HttpGet]
        [Route("standings")]
        public async Task<JObject> GetStandings(DateTime? date = null)
        {
            if (!date.HasValue)
            {
                date = DateTime.Now;
            }
            return await Scraper.GetStandings(date.Value);
        }

        [HttpGet]
        [Route("boxscores")]
        public async Task<JObject> GetBoxscore(string gameDir)
        {
            return await Scraper.GetBoxscore(gameDir);
        }

        [HttpGet]
        [Route("batters")]
        public async Task<JObject> GetBatter(string gameDir, int id)
        {
            return await Scraper.GetBatter(gameDir, id);
        }

        [HttpGet]
        [Route("pitchers")]
        public async Task<JObject> GetPitcher(string gameDir, int id)
        {
            return await Scraper.GetPitcher(gameDir, id);
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
