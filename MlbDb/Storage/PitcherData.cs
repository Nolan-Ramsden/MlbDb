using MlbDb.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace MlbDb.Storage
{
    public static class PitcherData
    {
        public static async Task<Pitcher> GetPitcherById(this MlbDatabase db, int id)
        {
            return await db.Pitchers
                .SingleOrDefaultAsync(p => p.PitcherId == id);
        }

        public static async Task<PitcherAppearance> GetPitcherAppearanceById(this MlbDatabase db, int id)
        {
            return await db.PitcherAppearances
                .Include(p => p.Pitcher)
                .Include(p => p.StatLine)
                .Include(p => p.PreGameSnapshot.BasesEmpty)
                .Include(p => p.PreGameSnapshot.BasesLoaded)
                .Include(p => p.PreGameSnapshot.Career)
                .Include(p => p.PreGameSnapshot.Season)
                .Include(p => p.PreGameSnapshot.vsLefties)
                .Include(p => p.PreGameSnapshot.vsRighties)
                .Include(p => p.PreGameSnapshot.Month)
                .Include(p => p.PreGameSnapshot.MenOnBase)
                .Include(p => p.PreGameSnapshot.RunnersInScoringPosition)
                .Include(p => p.PreGameSnapshot.BasesLoaded)
                .SingleOrDefaultAsync(p => p.PitcherAppearanceId == id);
        }

        public static async Task<PitcherAppearance> GetPitchersLastAppearance(this MlbDatabase db, int pitcherId, DateTime beforeDate)
        {
            return await db.PitcherAppearances.AsNoTracking()
                .Where(p => p.Pitcher.PitcherId == pitcherId && p.Date < beforeDate && p.Date.Year == beforeDate.Year)
                .OrderByDescending(p => p.Date).ThenByDescending(p => p.PitcherAppearanceId)
                .Include(p => p.Pitcher)
                .Include(p => p.StatLine)
                .Include(p => p.PreGameSnapshot.BasesEmpty)
                .Include(p => p.PreGameSnapshot.BasesLoaded)
                .Include(p => p.PreGameSnapshot.Career)
                .Include(p => p.PreGameSnapshot.Season)
                .Include(p => p.PreGameSnapshot.vsLefties)
                .Include(p => p.PreGameSnapshot.vsRighties)
                .Include(p => p.PreGameSnapshot.Month)
                .Include(p => p.PreGameSnapshot.MenOnBase)
                .Include(p => p.PreGameSnapshot.RunnersInScoringPosition)
                .Include(p => p.PreGameSnapshot.BasesLoaded)
                .FirstOrDefaultAsync();
        }
    }
}               