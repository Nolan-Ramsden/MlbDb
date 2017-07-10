using Microsoft.Practices.Unity;
using MlbDb.Models;
using MlbDb.Parsers;
using MlbDb.Scrapers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MlbDb.Services
{
    public class DownloadService : IDisposable
    {
        [Dependency]
        public MlbDataParser Parser { get; set; }

        [Dependency]
        public MlbDataScraper Scraper { get; set; }

        public async Task<Scoreboard> DownloadScoreboard(DateTime date)
        {
            var scoreboard = Parser.ParseScoreboard(await Scraper.GetScoreboard(date));
            var teamStandings = await GetStandingsIfApplicable(date);
            foreach(var game in scoreboard.Games)
            {
                if (game.GameDetailLocation != null)
                {
                    await DownloadAndUpdateGame(game);
                    // Grab all the player info from their own data files
                    foreach (var batter in game.Away.Batters.Union(game.Home.Batters))
                    {
                        await DownloadAndUpdateBatter(game.GameDetailLocation, batter);
                    }
                    foreach (var pitcher in game.Away.Pitchers.Union(game.Home.Pitchers))
                    {
                        await DownloadAndUpdatePitcher(game.GameDetailLocation, pitcher);
                    }
                }
                
                // Add in their win/loss splits if date isnt too far in future
                if (teamStandings.ContainsKey(game.Away.Team.TeamId))
                {
                    game.Away.Standings = teamStandings[game.Away.Team.TeamId].Standings;
                }
                if (teamStandings.ContainsKey(game.Home.Team.TeamId))
                {
                    game.Home.Standings = teamStandings[game.Home.Team.TeamId].Standings;
                }
            }
            return scoreboard;
        }

        private async Task<Dictionary<int, TeamAppearance>> GetStandingsIfApplicable(DateTime date)
        {
            if (DateTime.Now.AddDays(1).Subtract(date).Days < 0)
            {
                return new Dictionary<int, TeamAppearance>(); 
            }
            return Parser.ParseStandings(await Scraper.GetStandings(date));
        }

        private async Task DownloadAndUpdateGame(Game game)
        {
            var gameDetails = Parser.ParseBoxscore(await Scraper.GetBoxscore(game.GameDetailLocation));
            if (gameDetails == null)
            {
                return;
            }
            if (gameDetails.Away.Batters.Any())
            {
                game.Away.Batters = gameDetails.Away.Batters;
            }
            if (gameDetails.Home.Batters.Any())
            {
                game.Home.Batters = gameDetails.Home.Batters;
            }
            if (gameDetails.Away.Pitchers.Any())
            {
                game.Away.Pitchers = gameDetails.Away.Pitchers;
            }
            if (gameDetails.Home.Pitchers.Any())
            {
                game.Home.Pitchers = gameDetails.Home.Pitchers;
            }
            game.Away.Team.FullName = gameDetails.Away.Team.FullName;
            game.Home.Team.FullName = gameDetails.Home.Team.FullName;
            game.Result = gameDetails.Result;
            game.Extra.AddRange(gameDetails.Extra);
        }

        private async Task DownloadAndUpdatePitcher(string gameDetail, PitcherAppearance pitcher)
        {
            var pitcherDetails = Parser.ParsePitcher(await Scraper.GetPitcher(gameDetail, pitcher.Pitcher.PitcherId));
            if (pitcherDetails == null)
            {
                return;
            }
            pitcher.Pitcher = pitcherDetails.Pitcher;
            pitcher.PreGameSnapshot = pitcherDetails.PreGameSnapshot;
        }

        private async Task DownloadAndUpdateBatter(string gameDetail, BatterAppearance batter)
        {
            var batterDetails = Parser.ParseBatter(await Scraper.GetBatter(gameDetail, batter.Batter.BatterId));
            if (batterDetails == null)
            {
                return;
            }
            batter.Batter = batterDetails.Batter;
            batter.PreGameSnapshot = batterDetails.PreGameSnapshot;
        }

        public void Dispose()
        {
            Scraper.Dispose();
        }
    }
}
