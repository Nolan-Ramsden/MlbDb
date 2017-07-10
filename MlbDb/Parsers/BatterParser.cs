using MlbDb.Models;
using Newtonsoft.Json.Linq;
using NLog;
using System;

namespace MlbDb.Parsers
{
    static class BatterParser
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static BatterAppearance Parse(JObject batter)
        {
            if (batter == null)
            {
                return null;
            }
            batter = batter["Player"] as JObject;
            return new BatterAppearance()
            {
                Batter = new Batter()
                {
                    BatterId = batter.SafeParseToken<int>("@id", Logger),
                    Bats = batter.SafeParseToken<BatSide>("@bats", Logger),
                    Throws = batter.SafeParseToken<ThrowingHand>("@throws", Logger),
                    NaturalPosition = batter.SafeParseToken<Position>("@pos", Logger),
                    //BirthDate = batter.SafeParseToken<DateTime>("@dob", Logger),
                    FirstName = batter.SafeParseToken<string>("@first_name", Logger),
                    LastName = batter.SafeParseToken<string>("@last_name", Logger),
                    JerseyNumber = batter.SafeParseToken<int>("@jersey_number", Logger),
                    HeightInches = batter.SafeParseToken<string>("@height", Logger).ToHeightInches(Logger),
                    WeightPounds = batter.SafeParseToken<int>("@weight", Logger)
                },
                PreGameSnapshot = new BatterStatSnap()
                {
                    BasesEmpty = ParseSimpleStats(batter, "Empty"),
                    BasesLoaded = ParseSimpleStats(batter, "Loaded"),
                    Career = ParseSimpleStats(batter, "career"),
                    MenOnBase = ParseSimpleStats(batter, "Men_On"),
                    Month = ParseSimpleStats(batter, "month"),
                    RunnersInScoringPosition = ParseSimpleStats(batter, "RISP"),
                    Season = ParseSimpleStats(batter, "season"),
                    vsLefties = ParseSimpleStats(batter, "vs_LHP"),
                    vsRighties = ParseSimpleStats(batter, "vs_RHP"),
                }
            };
        }

        static SimpleBatterStatLine ParseSimpleStats(JToken batter, string subject)
        {
            var stats = batter[subject];
            if (stats == null)
            {
                Logger.Warn("No statline for {0}", subject);
                return new SimpleBatterStatLine();
            }
            return new SimpleBatterStatLine()
            {
                AtBats = stats.SafeParseToken<int>("@ab", Logger),
                CaughtStealing = stats.SafeParseToken<int>("@cs", Logger),
                Hits = stats.SafeParseToken<int>("@h", Logger),
                HomeRuns = stats.SafeParseToken<int>("@hr", Logger),
                OnBasePlusSlugging = stats.SafeParseToken<double>("@ops", Logger),
                RunsBattedIn = stats.SafeParseToken<int>("@rbi", Logger),
                StolenBases = stats.SafeParseToken<int>("@sb", Logger),
                StrikeOuts = stats.SafeParseToken<int>("@so", Logger),
                Walks = stats.SafeParseToken<int>("@bb", Logger),
            };
        }
    }
}
