using MlbDb.Models;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace MlbDb.Storage
{
    public static class StandingsData
    {
        public static async Task<Standings> GetStandingsById(this MlbDatabase db, int id)
        {
            return await db.Standings
                .SingleOrDefaultAsync(s => s.StandingsId == id);
        }
    }
}