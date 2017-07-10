using MlbDb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MlbDb.Storage
{
    public static class TeamData
    {
        public static async Task<Team> GetTeamById(this MlbDatabase db, int id)
        {
            return await db.Teams.SingleOrDefaultAsync(b => b.TeamId == id);
        }

        public static async Task<Team> GetTeamByName(this MlbDatabase db, string name)
        {
            string lower = name.ToLower();
            return await db.Teams.SingleOrDefaultAsync(b =>
                b.Code.ToLower().Equals(lower) ||
                b.FullName.ToLower().Contains(lower));
        }

        public static async Task<TeamAppearance> GetTeamAppearanceById(this MlbDatabase db, int id)
        {
            return await db.TeamAppearances.SingleOrDefaultAsync(b => b.TeamAppearanceId == id);
        }

        public static async Task<RoadData> GetRoadData(this MlbDatabase db, int teamId, DateTime ending)
        {
            int numGames = 20;
            int bullpenGames = 5;
            var lastN = await db.Games.Where(g =>
                (g.Home.Team.TeamId == teamId || g.Away.Team.TeamId == teamId) &&
                (g.Date < ending && g.Date.Year == ending.Year))
                .Include(g => g.Home.Team)
                .Include(g => g.Home.Pitchers.Select(p => p.StatLine))
                .Include(g => g.Away.Pitchers.Select(p => p.StatLine))
                .OrderByDescending(g => g.Date)
                .Take(numGames)
                .ToListAsync();
            int count = 0;
            int inARow = 0;
            int totalAway = 0;
            int bullpenOuts = 0;
            bool streakBroken = false;
            foreach (var game in lastN.Where(g => g?.Home?.Team != null))
            {
                if (game.Home.Team.TeamId == teamId)
                {
                    streakBroken = true;
                    if (count < bullpenGames)
                    {
                        bullpenOuts += game.Home.BullpenOuts;
                    }
                }
                else
                {
                    if (!streakBroken)
                        inARow++;
                    totalAway++;
                    if (count < bullpenGames)
                    {
                        bullpenOuts += game.Away.BullpenOuts;
                    }
                }
                count++;
            }
            return new RoadData()
            {
                LastN = numGames,
                DaysInARow = inARow,
                PercentageOfLast = (double)totalAway / (double)numGames,
                RecentBullpenOuts = bullpenOuts,
            };
        }
    }

    public class RoadData
    {
        public int LastN { get; set; }
        public int DaysInARow { get; set; }
        public double PercentageOfLast { get; set; }
        public int RecentBullpenOuts { get; set; }
    }
}
