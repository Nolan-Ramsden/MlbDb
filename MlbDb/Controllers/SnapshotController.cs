using Microsoft.Practices.Unity;
using MlbDb.Models;
using MlbDb.Services;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace MlbDb.Controllers
{
    [RoutePrefix("api/v1/snapshots")]
    public class SnapshotController : ApiController, IDisposable
    {
        [Dependency]
        public SnapshotService Snapshot { get; set; }

        [HttpGet]
        [Route("")]
        public async Task<Scoreboard> GetScoreboard(DateTime? date = null)
        {
            if (!date.HasValue)
            {
                date = DateTime.Now;
            }
            return await Snapshot.GetOrDownloadScoreboard(date.Value);
        }

        [HttpGet]
        [Route("crawl")]
        public async Task GetScoreboard(DateTime startDate, DateTime? endDate = null)
        {
            if (!endDate.HasValue)
            {
                endDate = DateTime.Now;
            }
            await Snapshot.CrawlDates(startDate, endDate.Value);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Snapshot.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
