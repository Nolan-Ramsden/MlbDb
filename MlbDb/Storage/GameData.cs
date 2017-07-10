using MlbDb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace MlbDb.Storage
{
    public static class GameData
    {
        public static async Task<List<Game>> GetGamesByIds(this MlbDatabase db, IEnumerable<int> ids)
        {
            return await db.Games
               .FullIncludeGame()
               .Where(g => ids.Contains(g.GameId))
               .ToListAsync();
        }

        public static async Task<Game> GetGameById(this MlbDatabase db, int id)
        {
            return await db.Games
               .FullIncludeGame()
                .SingleOrDefaultAsync(g => g.GameId == id);
        }

        public static async Task<Game> GetTeamsLastGameBeforeDate(this MlbDatabase db, int teamId, DateTime before)
        {
            return await db.Games.AsNoTracking()
                .Where(g => g.Date < before && (g.Home.Team.TeamId == teamId || g.Away.Team.TeamId == teamId) && g.Status == GameStatus.COMPLETE)
                .FullIncludeGame()
                .OrderByDescending(g => g.Date)
                .FirstOrDefaultAsync();
        }

        public static IQueryable<Game> FullIncludeGame(this IQueryable<Game> games)
        {
            return games
                .Include(g => g.Home.Standings.Away)
                .Include(g => g.Home.Standings.Home)
                .Include(g => g.Home.Standings.InterLeague)
                .Include(g => g.Home.Standings.LastTen)
                .Include(g => g.Home.Standings.OneRunGames)
                .Include(g => g.Home.Standings.Totals)
                .Include(g => g.Home.Standings.VsCentral)
                .Include(g => g.Home.Standings.VsDivision)
                .Include(g => g.Home.Standings.VsEast)
                .Include(g => g.Home.Standings.VsLeft)
                .Include(g => g.Home.Standings.VsRight)
                .Include(g => g.Home.Standings.VsWest)
                .Include(g => g.Away.Standings.Away)
                .Include(g => g.Away.Standings.Home)
                .Include(g => g.Away.Standings.InterLeague)
                .Include(g => g.Away.Standings.LastTen)
                .Include(g => g.Away.Standings.OneRunGames)
                .Include(g => g.Away.Standings.Totals)
                .Include(g => g.Away.Standings.VsCentral)
                .Include(g => g.Away.Standings.VsDivision)
                .Include(g => g.Away.Standings.VsEast)
                .Include(g => g.Away.Standings.VsLeft)
                .Include(g => g.Away.Standings.VsRight)
                .Include(g => g.Away.Standings.VsWest)
                .Include(g => g.Home.Team)
                .Include(g => g.Away.Team)
                .Include(g => g.Home.Batters.Select(b => b.Batter))
                .Include(g => g.Home.Batters.Select(b => b.StatLine))
                .Include(g => g.Home.Batters.Select(b => b.PreGameSnapshot.BasesEmpty))
                .Include(g => g.Home.Batters.Select(b => b.PreGameSnapshot.BasesLoaded))
                .Include(g => g.Home.Batters.Select(b => b.PreGameSnapshot.Career))
                .Include(g => g.Home.Batters.Select(b => b.PreGameSnapshot.MenOnBase))
                .Include(g => g.Home.Batters.Select(b => b.PreGameSnapshot.Month))
                .Include(g => g.Home.Batters.Select(b => b.PreGameSnapshot.RunnersInScoringPosition))
                .Include(g => g.Home.Batters.Select(b => b.PreGameSnapshot.Season))
                .Include(g => g.Home.Batters.Select(b => b.PreGameSnapshot.vsLefties))
                .Include(g => g.Home.Batters.Select(b => b.PreGameSnapshot.vsRighties))
                .Include(g => g.Away.Batters.Select(b => b.Batter))
                .Include(g => g.Away.Batters.Select(b => b.StatLine))
                .Include(g => g.Away.Batters.Select(b => b.PreGameSnapshot.BasesEmpty))
                .Include(g => g.Away.Batters.Select(b => b.PreGameSnapshot.BasesLoaded))
                .Include(g => g.Away.Batters.Select(b => b.PreGameSnapshot.Career))
                .Include(g => g.Away.Batters.Select(b => b.PreGameSnapshot.MenOnBase))
                .Include(g => g.Away.Batters.Select(b => b.PreGameSnapshot.Month))
                .Include(g => g.Away.Batters.Select(b => b.PreGameSnapshot.RunnersInScoringPosition))
                .Include(g => g.Away.Batters.Select(b => b.PreGameSnapshot.Season))
                .Include(g => g.Away.Batters.Select(b => b.PreGameSnapshot.vsLefties))
                .Include(g => g.Away.Batters.Select(b => b.PreGameSnapshot.vsRighties))
                .Include(g => g.Home.Pitchers.Select(p => p.Pitcher))
                .Include(g => g.Home.Pitchers.Select(p => p.StatLine))
                .Include(g => g.Home.Pitchers.Select(p => p.PreGameSnapshot.BasesEmpty))
                .Include(g => g.Home.Pitchers.Select(p => p.PreGameSnapshot.BasesLoaded))
                .Include(g => g.Home.Pitchers.Select(p => p.PreGameSnapshot.Career))
                .Include(g => g.Home.Pitchers.Select(p => p.PreGameSnapshot.Season))
                .Include(g => g.Home.Pitchers.Select(p => p.PreGameSnapshot.vsLefties))
                .Include(g => g.Home.Pitchers.Select(p => p.PreGameSnapshot.vsRighties))
                .Include(g => g.Home.Pitchers.Select(p => p.PreGameSnapshot.Month))
                .Include(g => g.Home.Pitchers.Select(p => p.PreGameSnapshot.MenOnBase))
                .Include(g => g.Home.Pitchers.Select(p => p.PreGameSnapshot.RunnersInScoringPosition))
                .Include(g => g.Away.Pitchers.Select(p => p.Pitcher))
                .Include(g => g.Away.Pitchers.Select(p => p.StatLine))
                .Include(g => g.Away.Pitchers.Select(p => p.PreGameSnapshot.BasesEmpty))
                .Include(g => g.Away.Pitchers.Select(p => p.PreGameSnapshot.BasesLoaded))
                .Include(g => g.Away.Pitchers.Select(p => p.PreGameSnapshot.Career))
                .Include(g => g.Away.Pitchers.Select(p => p.PreGameSnapshot.Season))
                .Include(g => g.Away.Pitchers.Select(p => p.PreGameSnapshot.vsLefties))
                .Include(g => g.Away.Pitchers.Select(p => p.PreGameSnapshot.vsRighties))
                .Include(g => g.Away.Pitchers.Select(p => p.PreGameSnapshot.Month))
                .Include(g => g.Away.Pitchers.Select(p => p.PreGameSnapshot.MenOnBase))
                .Include(g => g.Away.Pitchers.Select(p => p.PreGameSnapshot.RunnersInScoringPosition))
                .Include(g => g.Location)
                .Include(g => g.Result.Away)
                .Include(g => g.Result.Home)
                .Include(g => g.Result.Innings);
        }
    }
}