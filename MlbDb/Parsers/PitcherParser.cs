using MlbDb.Models;
using Newtonsoft.Json.Linq;
using NLog;
using System;

namespace MlbDb.Parsers
{
    static class PitcherParser
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static PitcherAppearance Parse(JObject pitcher)
        {
            if (pitcher == null)
            {
                return null;
            }
            pitcher = pitcher["Player"] as JObject;
            return new PitcherAppearance()
            {
                Pitcher = new Pitcher()
                {
                    PitcherId = pitcher.SafeParseToken<int>("@id", Logger),
                    Throws = pitcher.SafeParseToken<ThrowingHand>("@throws", Logger),
                    //BirthDate = pitcher.SafeParseToken<DateTime>("@dob", Logger),
                    FirstName = pitcher.SafeParseToken<string>("@first_name", Logger),
                    LastName = pitcher.SafeParseToken<string>("@last_name", Logger),
                    JerseyNumber = pitcher.SafeParseToken<int>("@jersey_number", Logger),
                    HeightInches = pitcher.SafeParseToken<string>("@height", Logger).ToHeightInches(Logger),
                    WeightPounds = pitcher.SafeParseToken<int>("@weight", Logger)
                },
                PreGameSnapshot = new PitcherStatSnap()
                {
                    BasesEmpty = ParseSimpleStats(pitcher, "Empty"),
                    BasesLoaded = ParseSimpleStats(pitcher, "Loaded"),
                    Career = ParseSimpleStats(pitcher, "career"),
                    MenOnBase = ParseSimpleStats(pitcher, "Men_On"),
                    Month = ParseSimpleStats(pitcher, "Month"),
                    RunnersInScoringPosition = ParseSimpleStats(pitcher, "RISP"),
                    Season = ParseSimpleStats(pitcher, "season"),
                    vsLefties = ParseSimpleStats(pitcher, "vs_LHB"),
                    vsRighties = ParseSimpleStats(pitcher, "vs_RHB"),
                }
            };
        }

        static SimplePitcherStatLine ParseSimpleStats(JToken pitcher, string subject)
        {
            var stats = pitcher[subject];
            if (stats == null)
            {
                Logger.Warn("No statline for {0}", subject);
                return new SimplePitcherStatLine();
            }
            double innings = stats.SafeParseToken<double>("@ip", Logger);
            int outs = (int)(innings) * 3 + (int)((innings % 1) * 10);
            return new SimplePitcherStatLine()
            {
                AtBats = stats.SafeParseToken<int>("@ab", Logger),
                BattingAverage = stats.SafeParseToken<double>("@avg", Logger),
                Outs = outs,
                Hits = stats.SafeParseToken<int>("@h", Logger),
                HomeRunsAllowed = stats.SafeParseToken<int>("@hr", Logger),
                EarnedRunAverage = stats.SafeParseToken<double>("@era", Logger),
                WHIP = stats.SafeParseToken<double>("@whip", Logger),
                RunsBattedIn = stats.SafeParseToken<int>("@rbi", Logger),
                StolenBases = stats.SafeParseToken<int>("@sb"),
                StrikeOuts = stats.SafeParseToken<int>("@so", Logger),
                Walks = stats.SafeParseToken<int>("@bb", Logger),
                Wins = stats.SafeParseToken<int>("@w"),
                Losses = stats.SafeParseToken<int>("@l"),
            };
        }
    }
}
