using Microsoft.Practices.Unity;
using MlbDb.Models;
using MlbDb.Storage;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MlbDb.Controllers.Datalayer
{
    [RoutePrefix("api/v1/teams")]
    public class TeamsController : ApiController, IDisposable
    {
        [Dependency]
        public MlbDatabase Database { get; set; }

        [HttpGet]
        [Route("")]
        public async Task<List<Team>> GetTeams()
        {
            return await Database.Teams.ToListAsync();
        }

        [HttpGet]
        [Route("")]
        public async Task<Team> GetTeamByString(string name)
        {
            var team = await Database.GetTeamByName(name);
            if (team == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return team;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<Team> GetTeamById(int id)
        {
            var team = await Database.GetTeamById(id);
            if (team == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return team;
        }

        [HttpGet]
        [Route("appearances/{id}")]
        public async Task<TeamAppearance> GetTeamAppearanceById(int id)
        {
            var team = await Database.GetTeamAppearanceById(id);
            if (team == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return team;
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