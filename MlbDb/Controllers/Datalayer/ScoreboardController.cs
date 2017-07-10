using Microsoft.Practices.Unity;
using MlbDb.Filters;
using MlbDb.Models;
using MlbDb.Storage;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace MlbDb.Controllers.Datalayer
{
    [RoutePrefix("api/v1/scoreboards")]
    public class ScoreboardController : ApiController, IDisposable
    {
        [Dependency]
        public MlbDatabase Database { get; set; }

        [HttpGet]
        [Route("")]
        public async Task<Scoreboard> GetScoreboard(DateTime? date = null)
        {
            if (!date.HasValue)
            {
                date = DateTime.Now;
            }

            var scoreboard = await Database.GetScoreboardByDate(date.Value);
            if (scoreboard == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return scoreboard;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<Scoreboard> GetScoreboardById(int id)
        {
            var scoreboard = await Database.GetScoreboardById(id);
            if (scoreboard == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return scoreboard;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Database.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
