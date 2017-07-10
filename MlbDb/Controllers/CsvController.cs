using DotConf;
using Microsoft.Practices.Unity;
using MlbDb.Filters;
using MlbDb.Models;
using MlbDb.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MlbDb.Controllers
{
    [RoutePrefix("api/v1/csv")]
    public class CsvController : ApiController, IDisposable
    {
        [Dependency]
        public CsvService CsvService { get; set; }

        [HttpPost]
        [Route("create")]
        [ValidateModel]
        public async Task CreateCSV([FromBody]CsvRequest req)
        {
            req.ApplyDefaults();
            await CsvService.CreateCsv(req);
        }

        [HttpPost]
        [Route("single")]
        [ValidateModel]
        public async Task<object> CreateCSVForScorboard([FromBody]CsvRequest req)
        {
            req.ApplyDefaults();
            return await CsvService.CreateCSVForScorboard(req);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CsvService.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class CsvRequest
    {
        public static ConfigField<List<string>> ModelFeatures = new AutoConfigField<List<string>>(
            key: "csv.fields",
            description: "JSON Path expression for which fields to stream to CSV",
            required: true
        );

        public static ConfigField<string> TargetField = new AutoConfigField<string>(
            key: "csv.targetfield",
            description: "JSON Path expression for which fields to stream to CSV",
            required: true
        );

        [Required]
        public string FilePath { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool RequireTarget { get; set; } = false;

        public List<string> Features { get; set; }

        public string Target { get; set; }

        public List<string> RequiredGameTypes { get; set; }

        public List<string> RequiredGameStatus { get; set; }

        public List<string> RequiredTeamLeagues { get; set; }

        public void ApplyDefaults()
        {
            Features = Features ?? ModelFeatures;
            Target = Target ?? TargetField;
            RequiredGameTypes = RequiredGameTypes ?? new List<string>()
            {
                GameType.REGULAR_SEASON.ToString()
            };
            RequiredGameStatus = RequiredGameStatus ?? new List<string>()
            {
                GameStatus.COMPLETE.ToString(), GameStatus.SCHEDULED.ToString()
            };
            RequiredTeamLeagues = RequiredTeamLeagues ?? new List<string>()
            {
                TeamLeague.AMERICAN.ToString(), TeamLeague.NATIONAL.ToString()
            };
        }
    }
}