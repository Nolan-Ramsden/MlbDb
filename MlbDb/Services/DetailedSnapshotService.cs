using Microsoft.Practices.Unity;
using MlbDb.Models;
using MlbDb.Storage;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MlbDb.Services
{
    public class DetailedSnapshotService : IDisposable
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Dependency]
        public SnapshotService Snapshot { get; set; }

        public async Task<JObject> GetDetailedSnapshot(DateTime date, bool requireRegularSeason = true, bool requireComplete = false)
        {
            var snapshot = await Snapshot.GetOrDownloadScoreboard(date);
            if (requireRegularSeason)
            {
                snapshot.Games = snapshot.Games.Where(g => g.Type == GameType.REGULAR_SEASON).ToList();
            }
            if (requireComplete)
            {
                snapshot.Games = snapshot.Games.Where(g => g.Status == GameStatus.COMPLETE).ToList();
            }

            var snapJson = JObject.FromObject(snapshot);
            foreach(var gameJson in snapJson["Games"] as JArray)
            {
                var game = snapshot.Games.Where(g => g.GameId == gameJson["GameId"].Value<int>()).Single();
                await DetailGame(game, gameJson);
            }
            return snapJson;
        }

        private async Task DetailGame(Game game, JToken gameJson)
        {
            await DetailTeam(game, gameJson["Home"]);
            await DetailTeam(game, gameJson["Away"]);
        }

        private async Task DetailTeam(Game game, JToken teamJson)
        {
            var lastGame = await Snapshot.Database.GetTeamsLastGameBeforeDate(teamJson["Team"]["TeamId"].Value<int>(), game.Date);
            if (lastGame == null)
            {
                Logger.Warn("No last game for {0}", teamJson["Team"]["Name"]);
                return;
            }
            Logger.Debug("Last game for {0} was {1} hours ago", teamJson["Team"]["Name"], (game.Date.Subtract(lastGame.Date).TotalHours));
            var lastGameJson = JObject.FromObject(lastGame);
            
            // find if I was home or away last game and update as needed
            Team me = null;
            TeamAppearance myAppearance = null;
            bool IWasHome = lastGame.Home.Team.TeamId == teamJson["Team"]["TeamId"].Value<int>();
            if (IWasHome)
            {
                me = lastGame.Home.Team;
                myAppearance = lastGame.Home;
                lastGameJson["Me"] = lastGameJson["Home"];
                lastGameJson["Them"] = lastGameJson["Away"];
            }
            else
            {
                me = lastGame.Away.Team;
                myAppearance = lastGame.Away;
                lastGameJson["Me"] = lastGameJson["Away"];
                lastGameJson["Them"] = lastGameJson["Home"];
            }
            lastGameJson["Away"].Parent.Remove();
            lastGameJson["Home"].Parent.Remove();
            teamJson["LastGame"] = lastGameJson;

            // time change and time since last game + bullpen rest
            var roadData = await Snapshot.Database.GetRoadData(me.TeamId, game.Date);
            teamJson["Rest"] = JObject.FromObject(new
            {
                HoursSinceLastGame = game.Date.Subtract(lastGame.Date).TotalHours,
                HoursTimeChangeFromLastGame = (int)game.Location.Timezone - (int)lastGame.Location.Timezone,
                HoursTimeChangeFromHomeTown = (int)game.Location.Timezone - (int)me.Timezone,
                LastGameTotalInnings = lastGame.Result.Innings.Count,
                DaysOnRoad = roadData.DaysInARow,
                RecentPercentageOnRoad = roadData.PercentageOfLast,
                LastGameBullpenOuts = myAppearance.Pitchers.Sum(p => p.StatLine.Outs) - myAppearance.StartingPitcher.StatLine.Outs,
                RecentBullpenOuts = roadData.RecentBullpenOuts,
            });

            // detail out the starting pitcher with some extra stuff since it wont be here yet
            var startingP = teamJson["StartingPitcher"];
            var pitcherIdToken = startingP?.SelectToken("Pitcher.PitcherId");
            if (startingP != null && pitcherIdToken != null)
            {
                var lastOuting = await Snapshot.Database.GetPitchersLastAppearance(pitcherIdToken.Value<int>(), game.Date);
                if (lastOuting != null)
                {
                    teamJson["StartingPitcherDetailed"] = JObject.FromObject(lastOuting);
                }
                else
                {
                    Logger.Warn("No previous pitcher entry for {0}", startingP["Pitcher"]["FullName"].Value<string>());
                }
            }
            else
            {
                Logger.Warn("No starting pitcher entry");
            }
        }

        public void Dispose()
        {
            ((IDisposable)Snapshot).Dispose();
        }
    }
}