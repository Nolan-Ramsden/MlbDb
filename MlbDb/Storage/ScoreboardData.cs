using MlbDb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MlbDb.Storage
{
    public static class ScoreboardData
    {
        public static async Task<Scoreboard> GetScoreboardByDate(this MlbDatabase db, DateTime date)
        {
            var scoreboard = await db.Scoreboards
                .Include(s => s.Games)
                .SingleOrDefaultAsync(s => s.Date.Day == date.Day
                                   && s.Date.Month == date.Month
                                   && s.Date.Year == date.Year);
            if (scoreboard != null)
            {
                scoreboard.Games = await db.GetGamesByIds(scoreboard.Games.Select(g => g.GameId));
            }
            return scoreboard;
        }

        public static async Task<Scoreboard> GetScoreboardById(this MlbDatabase db, int id)
        {
            var scoreboard = await db.Scoreboards
                .Include(s => s.Games)
                .SingleOrDefaultAsync(s => s.ScoreboardId == id);
            if (scoreboard != null)
            {
                scoreboard.Games = await db.GetGamesByIds(scoreboard.Games.Select(g => g.GameId));
            }
            return scoreboard;
        }

        public static async Task CreateScoreboard(this MlbDatabase db, Scoreboard scoreboard)
        {
            // Remove any games with same gameID just in case
            var gameIds = scoreboard.Games.Select(g => g.GameId);
            db.Games.RemoveRange(await db.Games.Where(g => gameIds.Contains(g.GameId)).ToListAsync());

            // Avoid edge case where player plays two different teams in same day
            var allBatters = scoreboard.Games.SelectMany(s => s.Away.Batters).Union(scoreboard.Games.SelectMany(s => s.Home.Batters))
                    .GroupBy(b => b.Batter.BatterId).ToDictionary(g => g.First().Batter.BatterId, g => g.First().Batter);
            var allPitchers = scoreboard.Games.SelectMany(s => s.Away.Pitchers).Union(scoreboard.Games.SelectMany(s => s.Home.Pitchers))
                    .GroupBy(b => b.Pitcher.PitcherId).ToDictionary(g => g.First().Pitcher.PitcherId, g => g.First().Pitcher);
            // Check if there are existing players we can use
            foreach (var game in scoreboard.Games)
            {
                await ReplaceDuplicates(db, game.Home, allBatters, allPitchers);
                await ReplaceDuplicates(db, game.Away, allBatters, allPitchers);
                db.Games.Add(game);
            }
            db.Scoreboards.Add(scoreboard);
        }

        public static void DeleteScoreboard(this MlbDatabase db, Scoreboard scoreboard)
        {
            // TODO: Add additional cleanup since cascades aren't enabled
            db.BatterAppearances.RemoveRange(scoreboard.Games.SelectMany(p => p.Away.Batters));
            db.BatterAppearances.RemoveRange(scoreboard.Games.SelectMany(p => p.Home.Batters));
            db.PitcherAppearances.RemoveRange(scoreboard.Games.SelectMany(p => p.Away.Pitchers));
            db.PitcherAppearances.RemoveRange(scoreboard.Games.SelectMany(p => p.Home.Pitchers));
            db.TeamAppearances.RemoveRange(scoreboard.Games.Select(g => g.Away));
            db.TeamAppearances.RemoveRange(scoreboard.Games.Select(g => g.Home));
            db.Games.RemoveRange(scoreboard.Games);
            db.Scoreboards.Remove(scoreboard);
        }

        private static async Task ReplaceDuplicates(this MlbDatabase db, TeamAppearance appearance, Dictionary<int, Batter> batterSet, Dictionary<int, Pitcher> pitcherSet)
        {
            // Replace player entries
            appearance.Batters = appearance.Batters.GroupBy(b => b.Batter.BatterId).Select(g => g.First()).ToList();
            var batterIDs = appearance.Batters.Select(b => b.Batter.BatterId);
            var existingBatters = (await db.Batters.Where(b => batterIDs.Contains(b.BatterId))
                .ToListAsync())
                .ToDictionary(b => b.BatterId, b => b);
            foreach(var batter in appearance.Batters)
            {
                batter.Batter = batterSet[batter.Batter.BatterId];
                if (existingBatters.ContainsKey(batter.Batter.BatterId))
                {
                    var existingBatter = existingBatters[batter.Batter.BatterId];
                    if ((existingBatter.HeightInches <= 0 || existingBatter.WeightPounds <= 0)
                      && (batter.Batter.HeightInches > 0 && batter.Batter.WeightPounds > 0))
                    {
                        existingBatter.Bats = batter.Batter.Bats;
                        existingBatter.FirstName = batter.Batter.FirstName;
                        existingBatter.LastName = batter.Batter.LastName;
                        existingBatter.HeightInches = batter.Batter.HeightInches;
                        existingBatter.JerseyNumber = batter.Batter.JerseyNumber;
                        existingBatter.NaturalPosition = batter.Batter.NaturalPosition;
                        existingBatter.Throws = batter.Batter.Throws;
                        existingBatter.WeightPounds = batter.Batter.WeightPounds;
                    }
                    batter.Batter = existingBatter;
                }
            }

            appearance.Pitchers = appearance.Pitchers.GroupBy(p => p.Pitcher.PitcherId).Select(g => g.First()).ToList();
            var pitcherIDs = appearance.Pitchers.Select(p => p.Pitcher.PitcherId);
            var existingPitchers = (await db.Pitchers.Where(p => pitcherIDs.Contains(p.PitcherId))
                .ToListAsync())
                .ToDictionary(p => p.PitcherId, p => p);
            foreach (var pitcher in appearance.Pitchers)
            {
                pitcher.Pitcher = pitcherSet[pitcher.Pitcher.PitcherId];
                if (existingPitchers.ContainsKey(pitcher.Pitcher.PitcherId))
                {
                    var existingPitcher = existingPitchers[pitcher.Pitcher.PitcherId];
                    if ((existingPitcher.HeightInches <= 0 || existingPitcher.WeightPounds <= 0)
                      && (pitcher.Pitcher.HeightInches > 0 && pitcher.Pitcher.WeightPounds > 0))
                    {
                        existingPitcher.FirstName = pitcher.Pitcher.FirstName;
                        existingPitcher.LastName = pitcher.Pitcher.LastName;
                        existingPitcher.HeightInches = pitcher.Pitcher.HeightInches;
                        existingPitcher.JerseyNumber = pitcher.Pitcher.JerseyNumber;
                        existingPitcher.Throws = pitcher.Pitcher.Throws;
                        existingPitcher.WeightPounds = pitcher.Pitcher.WeightPounds;
                    }
                    pitcher.Pitcher = existingPitcher;
                }
            }

            // Replace team entry
            var existingTeam = await db.Teams.SingleOrDefaultAsync(t => t.TeamId == appearance.Team.TeamId);
            if (existingTeam != null)
            {
                appearance.Team = existingTeam;
            }
        }
    }
}
