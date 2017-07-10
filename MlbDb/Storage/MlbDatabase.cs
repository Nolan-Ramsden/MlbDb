using DotConf;
using MlbDb.Models;
using NLog;
using System;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using MySql.Data.Entity;

namespace MlbDb.Storage
{
    //[DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class MlbDatabase : DbContext
    {
        public static ConfigField<string> DatabaseURL = new AutoConfigField<string>(
            key: "data.database.url",
            description: "The URL of the database to store MLB models to",
            required: false,
            defaultVal: "MlbDatabase"
        );

        public static ConfigField<bool> ForceSync = new AutoConfigField<bool>(
            key: "data.database.forcesync",
            description: "Recreate the DB model no matter if it exists or not",
            required: false
        );

        public static ConfigField<int> TimeoutSeconds = new AutoConfigField<int>(
            key: "data.database.timeout",
            description: "How long to wait for a query to respond in seconds",
            required: false,
            defaultVal: 240
        );

        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static bool _firstEntry = false;

        public MlbDatabase() : base(DatabaseURL)
        {
            Database.Log = (string s) => Logger.Trace(s);
            Database.CommandTimeout = TimeoutSeconds;
            Database.SetInitializer<MlbDatabase>(new MlbDbInitializer ());
            if (!_firstEntry)
            {
                if (ForceSync)
                {
                    Logger.Info("MlbDB Database Force Sync is true, deleting before creating");
                    this.Database.Delete();
                }
                if (!this.Database.Exists())
                {
                    Logger.Info("MlbDB Database does not exist, creating");
                    this.Database.Create();
                }
                else
                {
                    Logger.Info("MlbDB Database already exists");
                }
                _firstEntry = true;
            }
        }

        public DbSet<Batter> Batters { get; set; }
        public DbSet<BatterAppearance> BatterAppearances { get; set; }
        public DbSet<BatterStatLine> BatterStats{ get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Pitcher> Pitchers { get; set; }
        public DbSet<PitcherAppearance> PitcherAppearances { get; set; }
        public DbSet<PitcherStatLine> PitcherStats { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Scoreboard> Scoreboards { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Standings> Standings { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamAppearance> TeamAppearances { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Properties()
                .Where(x => x.PropertyType == typeof(bool))
                .Configure(x => x.HasColumnType("bit"));
        }

        public override Task<int> SaveChangesAsync()
        {
            try
            {
                return base.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Logger.Error(e, "Unable to save changes");
                if (e is DbEntityValidationException)
                {
                    Logger.Error("Validation Errors: \r\n\t- {0}",
                        string.Join("\r\n\t- ", (e as DbEntityValidationException).EntityValidationErrors.Select(v => v.ValidationErrors.First().ErrorMessage)));
                }
                throw;
            }
        }
    }

    class MlbDbInitializer : DropCreateDatabaseIfModelChanges<MlbDatabase>
    {

    }
}