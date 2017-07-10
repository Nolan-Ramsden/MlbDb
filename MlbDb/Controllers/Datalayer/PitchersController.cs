using System;
using Microsoft.Practices.Unity;
using MlbDb.Models;
using MlbDb.Storage;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace MlbDb.Controllers.Datalayer
{
    [RoutePrefix("api/v1/pitchers")]
    public class PitchersController : ApiController, IDisposable
    {
        [Dependency]
        public MlbDatabase Database { get; set; }

        [HttpGet]
        [Route("{id}")]
        public async Task<Pitcher> GetPitcherById(int id)
        {
            var pitcher = await Database.GetPitcherById(id);
            if (pitcher == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return pitcher;
        }

        [HttpGet]
        [Route("appearance/{id}")]
        public async Task<PitcherAppearance> GetPitcherAppearanceById(int id)
        {
            var pitcher = await Database.GetPitcherAppearanceById(id);
            if (pitcher == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return pitcher;
        }

        [HttpGet]
        [Route("last/{id}")]
        public async Task<PitcherAppearance> GetLastPitcherAppearanceById(int id)
        {
            var pitcher = await Database.GetPitchersLastAppearance(id, DateTime.Now);
            if (pitcher == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return pitcher;
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