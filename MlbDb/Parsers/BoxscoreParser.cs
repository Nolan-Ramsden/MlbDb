using MlbDb.Models;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;

namespace MlbDb.Parsers
{
    static class BoxscoreParser
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static Game Parse(JObject boxscore)
        {
            if (boxscore == null)
            {
                return null;
            }
            var boxscoreToken = boxscore.SelectToken("data.boxscore");
            if (boxscore == null || boxscoreToken.Type != JTokenType.Object)
            {
                Logger.Warn("Boxscore data empty, return blank game");
                return new Game();
            }
            var boxscoreO = boxscoreToken as JObject;
            var date = boxscoreO.SafeParseToken<DateTime>("date", Logger);
            return new Game()
            {
                GameId = boxscoreO.SafeParseToken<int>("game_pk", Logger),
                Date = date,
                Home = ParseTeamAppearance(boxscoreO, "home", date),
                Away = ParseTeamAppearance(boxscoreO, "away", date),
                Result = ParseResult(boxscoreO),
                Location = ParseLocation(boxscoreO),
                Extra = ParseExtraInfo(boxscoreO),
            };
        }

        static TeamAppearance ParseTeamAppearance(JObject game, string homeOrAway, DateTime day)
        {
            var appearance = new TeamAppearance()
            {
                Date = day,
                Team = ParseTeam(game, homeOrAway),
                Pitchers = ParseTeamPitchers(game, homeOrAway, day),
                Batters = ParseTeamBatters(game, homeOrAway, day),
            };
            return appearance;
        }

        private static List<BatterAppearance> ParseTeamBatters(JObject game, string homeOrAway, DateTime day)
        {
            var batters = game.SelectToken("batting");
            var parsedBatters = new List<BatterAppearance>();
            if (batters == null || batters.Type != JTokenType.Array)
            {
                Logger.Warn("No batter data in boxscore, or is not Array");
                return parsedBatters;
            }

            JToken thisTeamsBatters = null;
            foreach (var teamBatters in batters as JArray)
            {
                if (teamBatters.SafeParseToken<string>("team_flag", Logger) == homeOrAway)
                {
                    thisTeamsBatters = teamBatters;
                    break;
                }
            }
            if (thisTeamsBatters == null)
            {
                Logger.Warn("Unable to find batting stats for {0} team", homeOrAway);
                return parsedBatters;
            }

            var batterList = thisTeamsBatters["batter"];
            if (batterList != null)
            {
                if (batterList is JArray)
                {
                    foreach (var batter in batterList as JArray)
                    {
                        var b = ParseBatter(batter, day);
                        if (b != null)
                        {
                            parsedBatters.Add(b);
                        }
                    }
                }
                else if (batterList is JObject)
                {
                    var b = ParseBatter(batterList, day);
                    if (b != null)
                    {
                        parsedBatters.Add(b);
                    }
                }
            }

            return parsedBatters;
        }

        private static List<PitcherAppearance> ParseTeamPitchers(JObject game, string homeOrAway, DateTime day)
        {
            var pitchers = game.SelectToken("pitching");
            var parsedPitchers = new List<PitcherAppearance>();
            if (pitchers == null || pitchers.Type != JTokenType.Array)
            {
                Logger.Warn("No pitcher data in boxscore, or is not Array");
                return parsedPitchers;
            }

            JToken thisTeamsPitchers = null;
            foreach (var teamPitchers in pitchers as JArray)
            {
                if (teamPitchers.SafeParseToken<string>("team_flag", Logger) == homeOrAway)
                {
                    thisTeamsPitchers = teamPitchers;
                    break;
                }
            }
            if (thisTeamsPitchers == null)
            {
                Logger.Warn("Unable to find pitching stats for {0} team", homeOrAway);
                return parsedPitchers;
            }

            var pitcherList = thisTeamsPitchers["pitcher"];
            if (pitcherList != null)
            {
                if (pitcherList is JArray)
                {
                    int order = 1;
                    foreach (var pitcher in pitcherList as JArray)
                    {
                        var p = ParsePitcher(pitcher, day);
                        if (p != null)
                        {
                            p.Order = order;
                            parsedPitchers.Add(p);
                        }
                        order++;
                    }
                }
                else if (pitcherList is JObject)
                {
                    var p = ParsePitcher(pitcherList, day);
                    if (p != null)
                    {
                        p.Order = 1;
                        parsedPitchers.Add(p);
                    }
                }
            }

            return parsedPitchers;
        }

        static BatterAppearance ParseBatter(JToken batter, DateTime day)
        {
            string lastname = batter.SafeParseToken<string>("name", Logger);
            return new BatterAppearance()
            {
                Date = day,
                Order = (int)Math.Floor((double)(batter.SafeParseToken<int>("bo") / 100)),
                Batter = new Batter()
                {
                    BatterId = batter.SafeParseToken<int>("id"),
                    FirstName = batter.SafeParseToken<string>("name_display_first_last", Logger).Replace(lastname, ""),
                    LastName = lastname,
                },
                Position = batter.SafeParseToken<Position>("pos", Logger),
                StatLine = new BatterStatLine()
                {
                    Assists = batter.SafeParseToken<int>("a", Logger),
                    AtBats = batter.SafeParseToken<int>("ab", Logger),
                    CaughtStealing = batter.SafeParseToken<int>("cs", Logger),
                    Doubles = batter.SafeParseToken<int>("d", Logger),
                    Errors = batter.SafeParseToken<int>("e", Logger),
                    GroundOuts = batter.SafeParseToken<int>("go"),
                    PutOuts = batter.SafeParseToken<int>("po", Logger),
                    SacrificeBunts = batter.SafeParseToken<int>("sac", Logger),
                    Hits = batter.SafeParseToken<int>("h", Logger),
                    HitByPitch = batter.SafeParseToken<int>("hbp", Logger),
                    HomeRuns = batter.SafeParseToken<int>("hr", Logger),
                    SacrificeFlies = batter.SafeParseToken<int>("sf", Logger),
                    StolenBases = batter.SafeParseToken<int>("sb", Logger),
                    StrikeOuts = batter.SafeParseToken<int>("so", Logger),
                    Triples = batter.SafeParseToken<int>("t", Logger),
                    Walks = batter.SafeParseToken<int>("bb", Logger),
                    RunsBattedIn = batter.SafeParseToken<int>("rbi", Logger),
                },
            };
        }

        static PitcherAppearance ParsePitcher(JToken pitcher, DateTime day)
        {
            string lastName = pitcher.SafeParseToken<string>("name", Logger);
            return new PitcherAppearance()
            {
                Date = day,
                Pitcher = new Pitcher()
                {
                    PitcherId = pitcher.SafeParseToken<int>("id", Logger),
                    FirstName = pitcher.SafeParseToken<string>("name_display_first_last", Logger).Replace(lastName, string.Empty).Trim(),
                    LastName = lastName,
                },
                StatLine = new PitcherStatLine()
                {
                    Hits = pitcher.SafeParseToken<int>("h"),
                    Runs = pitcher.SafeParseToken<int>("r"),
                    Walks = pitcher.SafeParseToken<int>("bb"),
                    Outs = pitcher.SafeParseToken<int>("out"),
                    StrikeOuts = pitcher.SafeParseToken<int>("so"),
                    EarnedRuns = pitcher.SafeParseToken<int>("er"),
                    BattersFaced = pitcher.SafeParseToken<int>("bf"),
                    HomeRunsAllowed = pitcher.SafeParseToken<int>("hr"),
                    Save = pitcher.SafeParseToken<int>("sv") == 1,
                    Loser = pitcher.SafeParseToken<int>("l") == 1,
                    Winner = pitcher.SafeParseToken<int>("w") == 1,
                    Hold = pitcher.SafeParseToken<int>("hld") == 1,
                }
            };
        }

        static List<string> ParseExtraInfo(JObject game)
        {
            return new List<string>();
        }

        static Location ParseLocation(JObject game)
        {
            var venue = game.SafeParseToken<string>("venue_name");
            var venueId = game.SafeParseToken<int>("venue_id");
            if (venueId == 0 && venue == null)
            {
                Logger.Warn("Game location info totally empty, returning the unknown location");
                return new Location()
                {
                    LocationId = 999,
                    Place = "Unknown",
                    Venue = "Unknown",
                    Timezone = Models.TimeZone.UNKNOWN
                };
            }
            return new Location()
            {
                LocationId = venueId,
                Venue = venue,
            };
        }

        static Result ParseResult(JObject game)
        {
            var linescore = game["linescore"] as JObject;
            if (linescore == null)
            {
                Logger.Debug("No linescore yet");
                return null;
            }

            var r = new Result()
            {
                Home = ParseTeamResult(linescore, "home"),
                Away = ParseTeamResult(linescore, "away"),
            };
            if (linescore["inning_line_score"] as JArray != null)
            {
                foreach (var inning in linescore["inning_line_score"] as JArray)
                {
                    r.Innings.Add(new InningResult()
                    {
                        InningIndex = inning["inning"].ToInt(),
                        HomeScores = inning["home"].ToInt(),
                        AwayScores = inning["away"].ToInt(),
                    });
                }
            }
            return r;
        }

        static GameResult ParseTeamResult(JObject result, string homeOrAway)
        {
            return new GameResult()
            {
                Runs = result.SafeParseToken<int>(homeOrAway + "_team_runs"),
                Hits = result.SafeParseToken<int>(homeOrAway + "_team_hits"),
                Errors = result.SafeParseToken<int>(homeOrAway + "_team_errors"),
            };
        }

        static Team ParseTeam(JObject game, string homeOrAway)
        {
            return new Team()
            {
                TeamId = game.SafeParseToken<int>(homeOrAway + "_id", Logger),
                Code = game.SafeParseToken<string>(homeOrAway + "_team_code", Logger),
                City = game.SafeParseToken<string>(homeOrAway + "_sname", Logger),
                FullName = game.SafeParseToken<string>(homeOrAway + "_fname", Logger),
            };
        }
    }
}
