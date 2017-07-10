using System;

namespace MlbDb.Models
{
    public static class Formulas
    {
        public static double Round(double val)
        {
            return Math.Round(val, 4);
        }

        public static double OnBasePercentage(int hits, int walks, int hitbypitch, int atbats, int sacflies)
        {
            if ((atbats + walks + hitbypitch + sacflies) == 0)
            {
                return 0;
            }
            return Round((double)(hits + walks + hitbypitch) / (double)(atbats + walks + hitbypitch + sacflies));
        }

        public static double BattingAverage(int hits, int atbats)
        {
            if (atbats == 0)
            {
                return 0;
            }
            return Round((double)hits / (double)atbats);
        }

        public static double StrikeoutsPerAtBat(int strikeouts, int atbats)
        {
            if (atbats == 0)
            {
                return 0;
            }
            return Round((double)strikeouts / (double)atbats);
        }

        public static double SluggingPercentage(int singles, int doubles, int triples, int homeruns, int atbats)
        {
            if (atbats == 0)
            {
                return 0;
            }
            return Round((double)(singles + 2 * doubles + 3 * triples + 4 * homeruns) / (double)atbats);
        }

        public static double OPS(double onBasePercentage, double sluggingPercentage)
        {
            return Round(onBasePercentage + sluggingPercentage);
        }

        public static double StolenBasePercentage(int stolenBases, int caughtStealing)
        {
            if (stolenBases + caughtStealing == 0)
            {
                return 0;
            }
            return Round((double)(stolenBases) / (double)(stolenBases + caughtStealing));
        }

        public static double GroundOutPercentage(int groundOuts, int atBats)
        {
            if (atBats == 0)
            {
                return 0;
            }
            return Round((double)(groundOuts) / (double)(atBats));
        }

        public static double HomerunsPerAtBat(int homeRuns, int atBats)
        {
            if (atBats == 0)
            {
                return 0;
            }
            return Round((double)(homeRuns) / (double)(atBats));
        }

        public static double WalksPerGame(int walks, int outs)
        {
            if (outs == 0)
            {
                return 0;
            }
            return Round((double)(walks * 27) / (double)(outs));
        }

        public static double HomerunsPerGame(int homeRunsAllowed, int outs)
        {
            if (outs == 0)
            {
                return 0;
            }
            return Round((double)(homeRunsAllowed * 27) / (double)(outs));
        }

        public static double WHIP(int walks, int hits, int outs)
        {
            if (outs == 0)
            {
                return 0;
            }
            return Round((double)(walks + hits) / ((double)outs / 3));
        }

        public static double StrikoutsPerGame(int strikeOuts, int outs)
        {
            if (outs == 0)
            {
                return 0;
            }
            return Round((double)(strikeOuts * 27) / (double)(outs));
        }

        public static double EarnedRunAverage(int runs, int outs)
        {
            if (outs == 0)
            {
                return 0;
            }
            return Round((double)(runs * 27) / (double)(outs));
        }
    }
}