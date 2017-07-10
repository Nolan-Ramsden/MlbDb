using DotConf;
using Microsoft.Practices.Unity;
using MlbDb.Models;
using MlbDb.Storage;
using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MlbDb.Services
{
    public class SnapshotService : IDisposable
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static ConfigField<int> SnapshotUpdateMinimumWait = new AutoConfigField<int>(
            key: "snapshot.update.minimumwait",
            description: "The minimum amount of time (in minutes) before re-updating a scoreboard",
            required: false,
            defaultVal: 60
        );

        public static ConfigField<int> SnapshotUpdateFinishedAt = new AutoConfigField<int>(
            key: "snapshot.update.finishedat",
            description: "The minimum amount of time (in hours) before declaring a scoreboard closed",
            required: false,
            defaultVal: 30
        );

        [Dependency]
        public DownloadService Downloader { get; set; }

        [Dependency]
        public MlbDatabase Database { get; set; }

        public async Task CrawlDates(DateTime startDate, DateTime endDate)
        {
            int totalDays = (int)(endDate - startDate).TotalDays + 1;
            Logger.Info("Crawling scoreboards from {0} to {1} ({2} days)", startDate.ToShortDateString(), endDate.ToShortDateString(), (int)totalDays);
            Scoreboard s = null;
            for (int i = 1; i <= totalDays; i++)
            {
                s = await GetOrDownloadScoreboard(startDate.AddDays(i));
                if (i % 5 == 0)
                {
                    Logger.Debug("Clearing DbContext Cache");
                    Database.Dispose();
                    Database = new MlbDatabase();
                }

                Logger.Info("Completed crawl {0} of {1}", i, totalDays);
            }
            Logger.Info("Completed scoreboard crawl from {0} to {1} ({2} days)", startDate.ToShortDateString(), endDate.ToShortDateString(), (int)totalDays);
        }

        public async Task<Scoreboard> GetOrDownloadScoreboard(DateTime date)
        {
            Logger.Debug("Get Or Download Scoreboard for {0}", date.ToShortDateString());
            Scoreboard scoreboard = await Database.GetScoreboardByDate(date);
            if (scoreboard == null)
            {
                Logger.Info("Scoreboard not downloaded yet");
                scoreboard = await Downloader.DownloadScoreboard(date);
                Logger.Debug("Scoreboard downloaded, saving into database");
                await Database.CreateScoreboard(scoreboard);
                await Database.SaveChangesAsync();
                Logger.Info("Scoreboard saved");
            }
            else if (NeedsUpdating(scoreboard))
            {
                Logger.Info("Scoreboard out of date, refreshing");
                var newScoreboard = await Downloader.DownloadScoreboard(date);
                Logger.Debug("Deleting old scoreboard and adding new one");
                Database.DeleteScoreboard(scoreboard);
                await Database.CreateScoreboard(newScoreboard);
                await Database.SaveChangesAsync();
                scoreboard = newScoreboard;
                Logger.Info("Scoreboard saved");
            }
            else
            {
                Logger.Info("Scoreboard already downloaded");
            }
            return scoreboard;
        }

        private bool NeedsUpdating(Scoreboard scoreboard)
        {
            if (scoreboard.UpdatedAt.Subtract(scoreboard.Date).TotalHours > SnapshotUpdateFinishedAt.Value)
            {
                Logger.Debug("Scoreboard updated over {0} hours after game times", SnapshotUpdateFinishedAt.Value);
                return false;
            }

            var awaitingStats = new GameStatus[] { GameStatus.COMPLETE, GameStatus.RESCHEDULED, GameStatus.CANCELED };
            if (scoreboard.Games.Any(g => !awaitingStats.Contains(g.Status)))
            {
                Logger.Debug("Some games in scoreboard are not completed yet");
                var minutesTillAllow = scoreboard.UpdatedAt.AddMinutes(SnapshotUpdateMinimumWait.Value).Subtract(DateTime.Now).TotalMinutes;
                if (minutesTillAllow > 0)
                {
                    Logger.Debug("Waiting another {0} minutes before allowing an update", minutesTillAllow);
                }
                return minutesTillAllow < 0;
            }
            Logger.Debug("All games are complete in scoreboard");
            return true;
        }

        public void Dispose()
        {
            Database.Dispose();
            Downloader.Dispose();
        }
    }
}
