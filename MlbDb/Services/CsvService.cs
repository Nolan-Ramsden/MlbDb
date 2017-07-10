using Microsoft.Practices.Unity;
using MlbDb.Controllers;
using MlbDb.Models;
using MlbDb.Parsers;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MlbDb.Services
{
    public class CsvService : IDisposable
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Dependency]
        public DetailedSnapshotService Snapshot { get; set; }

        public async Task CreateCsv(CsvRequest request)
        {
            int totalDays = (int)(request.EndDate - request.StartDate).TotalDays + 1;
            Logger.Info("Creating CSV for {0} days, using {1} fields, Saving to {2}", totalDays, request.Features.Count, request.FilePath);

            int rowCount = 0;
            JObject o = null;
            for (int i = 0; i < totalDays; i++)
            {
                o = await Snapshot.GetDetailedSnapshot(request.StartDate.AddDays(i));
                foreach(var game in o["Games"] as JArray)
                {
                    bool success = AddGameToCsv(game, request.FilePath, request.Features, request.Target);
                    if (success)
                    {
                        rowCount++;
                    }
                }

                Logger.Info("Completed CSV stream {0} of {1} ({2} rows)", (i + 1), totalDays, rowCount);
            }
            Logger.Info("Completed CSV stream from {0} to {1} ({2} days)", request.StartDate.ToShortDateString(), request.EndDate.ToShortDateString(), totalDays);
        }

        public async Task<List<object>> CreateCSVForScorboard(CsvRequest req)
        {
            var results = new List<object>();
            var snapshot = await Snapshot.GetDetailedSnapshot(req.StartDate);
            if (!req.RequireTarget)
            {
                req.Target = null;
            }

            foreach (var game in snapshot["Games"] as JArray)
            {
                if (isGameValid(game, req))
                {
                    string csv = CreateCSVRow(game, req.Features, req.Target);
                    results.Add(new
                    {
                        Date = game["Date"].Value<string>(),
                        Summary = CreateGameSummary(game),
                        Home = game.SelectToken("Home.Team"),
                        Away = game.SelectToken("Away.Team"),
                        DataRow = csv
                    });
                }
            }
            return results;
        }

        private string CreateGameSummary(JToken game)
        {
            return game.SelectToken("Away.Team.Name") + " - " + game.SelectToken("Away.StartingPitcher.Pitcher.FullName") + " at " +
                   game.SelectToken("Home.Team.Name") + " - " + game.SelectToken("Home.StartingPitcher.Pitcher.FullName");
        }

        private string CreateCSVRow(JToken game, List<string> fields, string targetField)
        {
            List<string> fieldValues = new List<string>();
            foreach (var field in fields)
            {
                var token = game.SelectToken(field);
                if (token == null)
                {
                    Logger.Warn("Error finding {0} in snapshot json, skipping game", field);
                    return null;
                }
                fieldValues.Add(GetTokenValue(token));
            }
            if (targetField != null)
            {
                var targetToken = game.SelectToken(targetField);
                if (targetToken == null)
                {
                    Logger.Warn("Error finding {0} in snapshot json, skipping game", targetField);
                    return null;
                }
                fieldValues.Add(GetTokenValue(targetToken));
            }
            return string.Join(",", fieldValues);
        }

        private bool AddGameToCsv(JToken game, string filePath, List<string> fields, string targetField)
        {
            try
            {
                string fieldValues = CreateCSVRow(game, fields, targetField);
                if (fieldValues == null)
                {
                    return false;
                }
                AppendToCSVFile(filePath, fieldValues);
                return true;
            } catch(Exception e)
            {
                Logger.Error("Error when writing to CSV\n {0}", e.ToString());
                return false;
            }
        }

        private void AppendToCSVFile(string filePath, string fieldValues)
        {
            using (var stream = File.AppendText(filePath))
            {
                stream.WriteLine(fieldValues);
            }
        }

        private string GetTokenValue(JToken val)
        {
            string strVal = val.Value<string>();
            double n;
            if (!Double.TryParse(strVal, out n))
            {
                return "\"" + strVal + "\""; 
            }
            else
            {
                return strVal;
            }
        }

        private bool isGameValid(JToken game, CsvRequest req)
        {
            string gameType = game.SafeParseToken<string>("Type", Logger);
            if (!req.RequiredGameTypes.Contains(gameType))
            {
                Logger.Info("Game type {0} is not in the allowed list of game types, discarding", gameType);
            }
            string gameStatus = game.SafeParseToken<string>("Status", Logger);
            if (!req.RequiredGameStatus.Contains(gameStatus))
            {
                Logger.Info("Game status {0} is not in the allowed list of game status', discarding", gameStatus);
            }
            string awayLeague = game.SafeParseToken<string>("Away.Team.League", Logger);
            if (!req.RequiredTeamLeagues.Contains(awayLeague))
            {
                Logger.Info("Away Team League {0} is not in the allowed list of team leagues, discarding", awayLeague);
            }
            string homeLeague = game.SafeParseToken<string>("Home.Team.League", Logger);
            if (!req.RequiredTeamLeagues.Contains(homeLeague))
            {
                Logger.Info("Home Team League {0} is not in the allowed list of team leaguees, discarding", homeLeague);
            }
            return true;
        }

        public void Dispose()
        {
            ((IDisposable)Snapshot).Dispose();
        }
    }
}