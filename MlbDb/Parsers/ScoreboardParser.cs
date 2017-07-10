using System;
using System.Linq;
using MlbDb.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using NLog;

namespace MlbDb.Parsers
{
    static class ScoreboardParser
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static Scoreboard Parse(JObject scoreboard)
        {
            if (scoreboard == null)
            {
                return null;
            }
            AssertMinimumObject(scoreboard);
            DateTime date = ParseScoreboardDate(scoreboard);
            Scoreboard sb = new Scoreboard()
            {
                Date = date,
                UpdatedAt = DateTime.Now,
                Games = new List<Game>(),
            };

            JToken gamesOfDay = scoreboard.SelectToken("data.games.game");
            if (gamesOfDay == null)
            {
                Logger.Debug("No games today, returning empty scoreboard");
                return sb;
            }
            else if (gamesOfDay.Type == JTokenType.Object)
            {
                sb.Games.Add(ParseGame(gamesOfDay as JObject, date));
            }
            else if (gamesOfDay.Type == JTokenType.Array)
            {
                sb.Games = (gamesOfDay as JArray).Children().Select(g => ParseGame(g as JObject, date)).ToList();
            }
            else
            {
                Logger.Warn("Unknown data.games.game type {0}, returning empty scoreboard", gamesOfDay.Type);
            }

            return sb;
        }

        static bool AssertMinimumObject(JObject scoreboard)
        {
            return
                scoreboard.SelectToken("data.games").HasValues &&
                scoreboard.SelectToken("data.games.next_day_date").HasValues;
        }

        static DateTime ParseScoreboardDate(JObject scoreboard)
        {
            return DateTime.Parse(scoreboard.SelectToken("data.games.next_day_date").Value<string>()).AddDays(-1);
        }

        static Game ParseGame(JObject game, DateTime day)
        {
            return new Game()
            {
                Date = day,
                GameId = game.SafeParseToken<int>("game_pk", Logger),
                Type = game.SafeParseToken<GameType>("game_type", Logger),
                Status = game.SafeParseToken<GameStatus>("status.status", Logger),
                GameDetailLocation = game.SafeParseToken<string>("game_data_directory", Logger),
                Home = ParseTeamAppearance(game, "home", day),
                Away = ParseTeamAppearance(game, "away", day),
                Result = ParseResult(game),
                Location = ParseLocation(game),
                Extra = ParseExtraInfo(game),
            };
        }

        static TeamAppearance ParseTeamAppearance(JObject game, string homeOrAway, DateTime day)
        {
            var appearance = new TeamAppearance()
            {
                Date = day,
                Team = ParseTeam(game, homeOrAway)
            };
            var starter = ParseStarter(game, homeOrAway, day);
            if (starter != null)
            {
                appearance.Pitchers.Add(starter);
            }
            return appearance;
        }

        static PitcherAppearance ParseStarter(JObject game, string homeOrAway, DateTime day)
        {
            var pitcherToken = game[homeOrAway + "_probable_pitcher"];
            if (pitcherToken == null)
            {
                Logger.Debug("No probable pitcher for {0} team", homeOrAway);
                return null;
            }
            int? pitcherId = pitcherToken["id"]?.ToInt();
            if (!pitcherId.HasValue || pitcherId == 0)
            {
                Logger.Warn("No pitcher id for probable start on {0} team", homeOrAway);
                return null;
            }

            string firstN = pitcherToken.SafeParseToken<string>("first", Logger);
            string lastN = pitcherToken.SafeParseToken<string>("last", Logger);
            int number = pitcherToken.SafeParseToken<int>("number", Logger);
            ThrowingHand throws = pitcherToken.SafeParseToken<ThrowingHand>("throwinghand", Logger);

            return new PitcherAppearance()
            {
                Date = day,
                Order = 1,
                Pitcher = new Pitcher()
                {
                    PitcherId = pitcherId.Value,
                    FirstName = firstN,
                    LastName = lastN,
                    JerseyNumber = number,
                    Throws = throws,
                }
            };
        }

        static List<string> ParseExtraInfo(JObject game)
        {
            return new List<string>();
        }

        static Location ParseLocation(JObject game)
        {
            var location = new Location()
            {
                LocationId = game.SafeParseToken<int>("venue_id"),
                Place = game.SafeParseToken<string>("location"),
                Venue = game.SafeParseToken<string>("venue"),
                Timezone = game.SafeParseToken<Models.TimeZone>("time_zone"),
            };
            if (string.IsNullOrEmpty(location.Place))
            {
                location.Place = "Uknown";
            }
            if (string.IsNullOrEmpty(location.Venue))
            {
                location.Venue = "Uknown";
            }
            return location;
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
            if (linescore["inning"] as JArray != null)
            {
                int i = 1;
                foreach (var inning in linescore["inning"] as JArray)
                {
                    r.Innings.Add(new InningResult()
                    {
                        InningIndex = i,
                        HomeScores = inning["home"].ToInt(),
                        AwayScores = inning["away"].ToInt(),
                    });
                    i++;
                }
            }
            return r;
        }

        static GameResult ParseTeamResult(JObject result, string homeOrAway)
        {
            return new GameResult()
            {
                Errors = result.SafeParseToken<int>("e." + homeOrAway, Logger),
                Hits = result.SafeParseToken<int>("h." + homeOrAway, Logger),
                Runs = result.SafeParseToken<int>("r." + homeOrAway, Logger),
                StolenBases = result.SafeParseToken<int>("sb." + homeOrAway, Logger),
                StrikeOuts = result.SafeParseToken<int>("so." + homeOrAway, Logger),
                HomeRuns = result.SafeParseToken<int>("hr." + homeOrAway, Logger),
            };
        }

        static Team ParseTeam(JObject game, string homeOrAway)
        {
            return new Team()
            {
                TeamId = game.SafeParseToken<int>(homeOrAway + "_team_id", Logger),
                City = game.SafeParseToken<string>(homeOrAway + "_team_city", Logger),
                Code = game.SafeParseToken<string>(homeOrAway + "_code", Logger),
                Name = game.SafeParseToken<string>(homeOrAway + "_team_name", Logger),
                League = game.SafeParseToken<TeamLeague>(homeOrAway + "_league_id", Logger),
                Division = game.SafeParseToken<TeamDivision>(homeOrAway + "_division", Logger),
                Timezone = game.SafeParseToken<Models.TimeZone>(homeOrAway + "_time_zone", Logger),
            };
        }
    }
}
