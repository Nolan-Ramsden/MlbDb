using Microsoft.Practices.Unity;
using MlbDb.Services;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace MlbDb.Controllers
{
    [RoutePrefix("api/v1/snapshots/detailed")]
    public class DetailedSnapshotController : ApiController, IDisposable
    {
        [Dependency]
        public DetailedSnapshotService Snapshot { get; set; }

        [HttpGet]
        [Route("")]
        public async Task<object> GetScoreboard(DateTime? date = null)
        {
            if (!date.HasValue)
            {
                date = DateTime.Now;
            }
            return await Snapshot.GetDetailedSnapshot(date.Value);
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
