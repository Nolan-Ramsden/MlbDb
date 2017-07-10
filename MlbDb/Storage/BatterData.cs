using MlbDb.Models;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace MlbDb.Storage
{
    public static class BatterData
    {
        public static async Task<Batter> GetBatterById(this MlbDatabase db, int id)
        {
            return await db.Batters
                .SingleOrDefaultAsync(b => b.BatterId == id);
        }
    }
}