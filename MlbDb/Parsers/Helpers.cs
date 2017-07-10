using System;
using Newtonsoft.Json.Linq;
using NLog;
using MlbDb.Models;

namespace MlbDb.Parsers
{
    public static class Helpers
    {
        public static T SafeParseToken<T>(this JToken token, string key, Logger logger = null)
        {
            var jtoken = token.SelectToken(key);
            if (jtoken == null)
            {
                logger?.Warn("No {0} found in JSON", key);
                return default(T);
            }
            try
            {
                if (typeof(T) == typeof(int))
                {
                    return (T)Convert.ChangeType(jtoken.ToInt(), typeof(T));
                }
                if (typeof(T) == typeof(double))
                {
                    return (T)Convert.ChangeType(jtoken.ToDouble(), typeof(T));
                }
                else if (typeof(T) == typeof(Models.TimeZone))
                {
                    return (T)Convert.ChangeType(jtoken.Value<string>().ToTimeZone(logger), typeof(T));
                }
                else if (typeof(T) == typeof(Models.ThrowingHand))
                {
                    return (T)Convert.ChangeType(jtoken.Value<string>().ToThrowingHand(logger), typeof(T));
                }
                else if (typeof(T) == typeof(TeamDivision))
                {
                    return (T)Convert.ChangeType(jtoken.Value<string>().ToDivision(logger), typeof(T));
                }
                else if (typeof(T) == typeof(TeamLeague))
                {
                    return (T)Convert.ChangeType(jtoken.Value<int>().ToLeague(logger), typeof(T));
                }
                else if (typeof(T) == typeof(GameStatus))
                {
                    return (T)Convert.ChangeType(jtoken.Value<string>().ToGameStatus(jtoken, logger), typeof(T));
                }
                else if (typeof(T) == typeof(GameType))
                {
                    return (T)Convert.ChangeType(jtoken.Value<string>().ToGameType(logger), typeof(T));
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    return (T)Convert.ChangeType(DateTime.Parse(jtoken.Value<string>()), typeof(T));
                }
                else if (typeof(T) == typeof(Position))
                {
                    return (T)Convert.ChangeType(jtoken.Value<string>().ToPosition(logger), typeof(T));
                }
                else if (typeof(T) == typeof(BatSide))
                {
                    return (T)Convert.ChangeType(jtoken.Value<string>().ToBatSide(logger), typeof(T));
                }
                else
                {
                    return jtoken.Value<T>();
                }
            }
            catch (Exception e)
            {
                T res = default(T);
                logger?.Warn("Error when parsing {0}, returning {1} : {2}", typeof(T), res, e);
                return res;
            }
        }

        public static TeamDivision ToDivision(this string division, Logger logger = null)
        {
            switch (division)
            {
                case "E":
                case "East":
                    return TeamDivision.EAST;
                case "W":
                case "West":
                    return TeamDivision.WEST;
                case "C":
                case "Central":
                    return TeamDivision.CENTRAL;
                default:
                    logger?.Warn("Unrecognized team divison {0}, returning unknown", division);
                    return TeamDivision.UNKNOWN;
            }
        }

        public static TeamLeague ToLeague(this int leagueId, Logger logger = null)
        {
            if (leagueId == 103)
            {
                return TeamLeague.AMERICAN;
            }
            else if (leagueId == 104)
            {
                return TeamLeague.NATIONAL;
            }
            else
            {
                logger?.Warn("Unrecognized league ID {0} team", leagueId);
                return TeamLeague.UNKNOWN;
            }
        }

        public static Models.TimeZone ToTimeZone(this string timezone, Logger logger = null)
        {
            switch (timezone)
            {
                case "ET":
                case "EST":
                case "EDT":
                    return Models.TimeZone.EASTERN;
                case "MT":
                case "MST":
                case "MDT":
                    return Models.TimeZone.MOUNTAIN;
                case "CT":
                case "CST":
                case "CDT":
                    return Models.TimeZone.CENTRAL;
                case "PT":
                case "PST":
                case "PDT":
                    return Models.TimeZone.PACIFIC; ;
                default:
                    logger?.Warn("Unrecognized timezone {0}, returning UNKNOWN", timezone);
                    return Models.TimeZone.UNKNOWN;
            }
        }

        public static int ToInt(this JToken token, int failVal = 0)
        {
            try
            {
                return token.Value<int>();
            }
            catch (Exception)
            {
                try
                {
                    return Convert.ToInt32(token.Value<string>());
                }
                catch (Exception)
                {
                    return failVal;
                }
            }
        }

        public static double ToDouble(this JToken token, double failVal = 0)
        {
            try
            {
                return token.Value<double>();
            }
            catch (Exception)
            {
                try
                {
                    return Convert.ToDouble(token.Value<string>());
                }
                catch (Exception)
                {
                    return failVal;
                }
            }
        }

        public static ThrowingHand ToThrowingHand(this string throws, Logger logger = null)
        {
            if (throws == null)
            {
                return ThrowingHand.UNKNOWN;
            }

            switch (throws)
            {
                case "RHP":
                case "R":
                    return ThrowingHand.RIGHT;
                case "LHP":
                case "L":
                    return ThrowingHand.LEFT;
                default:
                    logger?.Warn("Unrecognized throwing hand {0}", throws);
                    return ThrowingHand.UNKNOWN;
            }
        }

        public static GameType ToGameType(this string gameType, Logger logger = null)
        {
            switch (gameType)
            {
                case "E":
                    return GameType.EXHIBITION;
                case "S":
                case "R":
                    return GameType.REGULAR_SEASON;
                case "P":
                    return GameType.PLAYOFFS;
                default:
                    logger?.Warn("Unrecognized game type {0}, returning unknown", gameType);
                    return GameType.UNKNOWN;
            }
        }

        public static GameStatus ToGameStatus(this string gameStatus, JToken game, Logger logger = null)
        {
            switch (gameStatus)
            {
                case "Preview":
                case "Pre-Game":
                    if (game.SelectToken("away_probable_pitcher") != null 
                        && game.SelectToken("home_probable_pitcher") != null)
                    {
                        return GameStatus.PREVIEW;
                    }
                    return GameStatus.SCHEDULED;
                case "Final":
                case "Game Over":
                    return GameStatus.COMPLETE;
                case "In Progress":
                    return GameStatus.IN_PROGRESS;
                case "Postponed":
                    return GameStatus.RESCHEDULED;
                default:
                    logger?.Warn("Unrecognized game status {0}, returning unknown", gameStatus);
                    return GameStatus.UNKNOWN;
            }
        }

        public static Position ToPosition(this string position, Logger logger = null)
        {
            string filteredPosition = position;
            if (position.StartsWith("PR-") || position.StartsWith("PH-"))
            {
                filteredPosition = filteredPosition.Split('-')[1];
            }
            if (position.Contains("-"))
            {
                filteredPosition = position.Split('-')[0];
            }

            switch (filteredPosition)
            {
                case "P":
                    return Position.PITCHER;
                case "C":
                    return Position.CATCHER;
                case "1B":
                    return Position.FIRST_BASE;
                case "2B":
                    return Position.SECOND_BASE;
                case "3B":
                    return Position.THIRD_BASE;
                case "SS":
                    return Position.SHORTSTOP;
                case "LF":
                    return Position.LEFT_FIELD;
                case "CF":
                    return Position.CENTER_FIELD;
                case "RF":
                    return Position.RIGHT_FIELD;
                case "DH":
                    return Position.DESIGNATED_HITTER;
                case "PH":
                    return Position.PINCH_HITTER;
                case "PR":
                    return Position.PINCH_RUNNER;
                case "OF":
                    return Position.OUTFIELD;
                default:
                    logger?.Warn("Unrecognized position {0}, returning unknown", position);
                    return Position.UNKNOWN;
            }
        }

        public static BatSide ToBatSide(this string batSide, Logger logger = null)
        {
            switch (batSide)
            {
                case "L":
                    return BatSide.LEFT;
                case "R":
                    return BatSide.RIGHT;
                case "S":
                    return BatSide.SWITCH;
                default:
                    logger?.Warn("Unrecognized batside {0}, returning unknown", batSide);
                    return BatSide.UNKNOWN;
            }
        }

        public static int ToHeightInches(this string heightStr, Logger logger = null)
        {
            try
            {
                string[] pieces = heightStr.Split('-');
                return Convert.ToInt32(pieces[0]) * 12 + Convert.ToInt32(pieces[1]);
            }
            catch(Exception)
            {
                logger?.Warn("Unable to parse height string {0}, return 6 feet", heightStr);
                return 72;
            }
        }
    }
}