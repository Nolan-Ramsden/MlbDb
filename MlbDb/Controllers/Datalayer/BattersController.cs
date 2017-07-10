using System;
using Microsoft.Practices.Unity;
using MlbDb.Models;
using MlbDb.Storage;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace MlbDb.Controllers.Datalayer
{
    [RoutePrefix("api/v1/batters")]
    public class BattersController : ApiController, IDisposable
    {
        [Dependency]
        public MlbDatabase Database { get; set; }

        [HttpGet]
        [Route("{id}")]
        public async Task<Batter> GetBatterById(int id)
        {
            var batter = await Database.GetBatterById(id);
            if (batter == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return batter;
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