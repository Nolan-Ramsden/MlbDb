using Microsoft.Practices.Unity;
using MlbDb.Models;
using NLog;
using System;
using System.Linq;
using System.Dynamic;
using System.Threading.Tasks;

namespace MlbDb.Services
{
    public class WinnerService
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [Dependency]
        public SnapshotService Scoreboard { get; set; }

        public async Task<object> GetGameResult(DateTime gameDate, string homeHint, string awayHint)
        {
            return (await Scoreboard.GetOrDownloadScoreboard(gameDate))
                .Games
                .Where(g => teamMatch(g.Home, homeHint) && teamMatch(g.Away, awayHint))
                .Select(g => formatGame(g))
                .ToList();
        }

        public static object formatGame(Game g)
        {
            dynamic r = new ExpandoObject();
            r.GameType = g.Type;
            r.GameStatus = g.Status;
            r.Winner = null;
            r.HomeWon = false;
            r.AwayWon = false;
            r.LoserName = null;
            r.WinnerName = null;
            r.HomeScore = 0;
            r.AwayScore = 0;
            r.HomeName = null;
            r.AwayName = null;
            if (!string.IsNullOrWhiteSpace(g?.Result?.Winner))
            {
                r.HomeScore = g.Result.Home.Runs;
                r.AwayScore = g.Result.Away.Runs;
                r.HomeName = g.Home.Team.Name;
                r.AwayName = g.Away.Team.Name;
                if (g.Result.Winner.Equals("Home"))
                {
                    r.Winner = "Home";
                    r.HomeWon = true;
                    r.WinnerName = g.Home.Team.Name;
                    r.LoserName = g.Away.Team.Name;
                }
                else if (g.Result.Winner.Equals("Away"))
                {
                    r.Winner = "Away";
                    r.AwayWon = true;
                    r.WinnerName = g.Away.Team.Name;
                    r.LoserName = g.Home.Team.Name;
                }
            }
            return r;
        }

        public static bool teamMatch(TeamAppearance team, string teamHint)
        {
            return
                (
                    teamHint == null ||
                    team.Team.Name == teamHint ||
                    team.Team.Code == teamHint ||
                    team.Team.FullName.Contains(teamHint)
                );
        }
    }
}