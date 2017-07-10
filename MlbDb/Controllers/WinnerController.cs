using Microsoft.Practices.Unity;
using MlbDb.Services;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace MlbDb.Controllers
{
    [RoutePrefix("api/v1/winner")]
    public class WinnerController : ApiController
    {
        [Dependency]
        public WinnerService Service { get; set; }

        [HttpGet]
        [Route("")]
        public async Task<object> GetWinner(DateTime? date = null, string homeHint = null, string awayHint = null)
        {
            if (!date.HasValue)
            {
                date = DateTime.Now;
            }
            return await Service.GetGameResult(date.Value, homeHint, awayHint);
        }
    }
}
