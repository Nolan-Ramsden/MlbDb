using MlbDb.Models;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MlbDb.Parsers
{
    static class StandingsParser
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static Dictionary<int, TeamAppearance> Parse(JObject standings)
        {
            if (standings ==  null)
            {
                return null;
            }
            var results = new Dictionary<int, TeamAppearance>();
            var rootToken = standings.SelectToken("historical_standings_schedule_date.standings_all_date_rptr.standings_all_date");
            if (rootToken == null || rootToken.Type != JTokenType.Array)
            {
                Logger.Warn("No standings data or non-array, returning empty standings");
                return results;
            }
            var allEntries = (rootToken as JArray).SelectMany(league => league.SelectTokens("queryResults.row")).SelectMany(e => e.Children());
            foreach (var entry in allEntries)
            {
                TeamAppearance appearance = ParseTeamStandings(entry);
                results.Add(appearance.Team.TeamId, appearance);
            }
            return results;
        }

        static TeamAppearance ParseTeamStandings(JToken entry)
        {
            var leagueDivision = entry.SafeParseToken<string>("division");
            TeamLeague league = TeamLeague.UNKNOWN;
            if (leagueDivision.Split(' ').First().Equals("National"))
            {
                league = TeamLeague.NATIONAL;
            }
            else if (leagueDivision.Split(' ').First().Equals("American"))
            {
                league = TeamLeague.AMERICAN;
            }
            return new TeamAppearance()
            {
                Team = new Team()
                {
                    TeamId = entry.SafeParseToken<int>("team_id", Logger),
                    Code = entry.SafeParseToken<string>("file_code", Logger),
                    FullName = entry.SafeParseToken<string>("team_full", Logger),
                    Division = leagueDivision.Split(' ').Last().ToDivision(),
                    League = league,
                },
                Standings = new Standings()
                {
                    Points = entry.SafeParseToken<int>("points", Logger),
                    Place = entry.SafeParseToken<int>("place", Logger),
                    Totals = new WinLossSplit()
                    {
                        Losses = entry.SafeParseToken<int>("l", Logger),
                        Wins = entry.SafeParseToken<int>("w", Logger),
                    },
                    Away = ParseWinLoss(entry, "away"),
                    ExtraInnings = ParseWinLoss(entry, "extra_inn"),
                    Home = ParseWinLoss(entry, "home"),
                    InterLeague = ParseWinLoss(entry, "interleague"),
                    LastTen = ParseWinLoss(entry, "last_ten"),
                    OneRunGames = ParseWinLoss(entry, "one_run"),
                    VsCentral = ParseWinLoss(entry, "vs_central"),
                    VsDivision = ParseWinLoss(entry, "vs_division"),
                    VsEast = ParseWinLoss(entry, "vs_east"),
                    VsLeft = ParseWinLoss(entry, "vs_left"),
                    VsRight = ParseWinLoss(entry, "vs_right"),
                    VsWest = ParseWinLoss(entry, "vs_west"),
                }
            };
        }

        private static WinLossSplit ParseWinLoss(JToken entry, string field)
        {
            string val = entry.SafeParseToken<string>(field, Logger);
            if (string.IsNullOrEmpty(val))
            {
                return new WinLossSplit();
            }
            return new WinLossSplit()
            {
                Wins = Convert.ToInt32(val.Split('-')[0]),
                Losses = Convert.ToInt32(val.Split('-')[1]),
            };
        }
    }
}
