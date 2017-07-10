using Microsoft.Practices.Unity;
using MlbDb.Models;
using MlbDb.Storage;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace MlbDb.Controllers.Datalayer
{
    [RoutePrefix("api/v1/games")]
    public class GamesController : ApiController, IDisposable
    {
        [Dependency]
        public MlbDatabase Database { get; set; }

        [HttpGet]
        [Route("{id}")]
        public async Task<Game> GetGameById(int id)
        {
            var game = await Database.GetGameById(id);
            if (game == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return game;
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
