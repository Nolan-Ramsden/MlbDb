using NLog;
using MlbDb.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace MlbDb.Parsers
{
    public class MlbDataParser
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Dictionary<int, TeamAppearance> ParseStandings(JObject standings)
        {
            Logger.Debug("Parsing standings data");
            var result = StandingsParser.Parse(standings);
            Logger.Debug("Parsed standings data");
            return result;
        }

        public Game ParseBoxscore(JObject boxScore)
        {
            Logger.Debug("Parsing boxscore data");
            var result = BoxscoreParser.Parse(boxScore);
            Logger.Debug("Parsed boxscore data");
            return result;
        }

        public Scoreboard ParseScoreboard(JObject scoreboard)
        {
            Logger.Debug("Parsing scoreboard data");
            var result = ScoreboardParser.Parse(scoreboard);
            Logger.Debug("Parsed scoreboard data");
            return result;
        }

        public PitcherAppearance ParsePitcher(JObject pitcher)
        {
            Logger.Debug("Parsing pitcher data");
            var result = PitcherParser.Parse(pitcher);
            Logger.Debug("Parsed pitcher data");
            return result;
        }

        public BatterAppearance ParseBatter(JObject batter)
        {
            Logger.Debug("Parsing batter data");
            var result = BatterParser.Parse(batter);
            Logger.Debug("Parsed batter data");
            return result;
        }
    }
}
