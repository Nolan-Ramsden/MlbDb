using System;
using Microsoft.Practices.Unity;
using MlbDb.Models;
using MlbDb.Storage;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace MlbDb.Controllers.Datalayer
{
    [RoutePrefix("api/v1/standings")]
    public class StandingsController : ApiController, IDisposable
    {
        [Dependency]
        public MlbDatabase Database { get; set; }

        [HttpGet]
        [Route("{id}")]
        public async Task<Standings> GetStandingsById(int id)
        {
            var standings = await Database.GetStandingsById(id);
            if (standings == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return standings;
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