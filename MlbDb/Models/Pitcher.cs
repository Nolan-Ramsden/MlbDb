using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MlbDb.Models
{
    [Table("pitchers")]
    public class Pitcher
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PitcherId { get; set; }

        [Required]
        public int JerseyNumber { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public int HeightInches { get; set; }

        [Required]
        public int WeightPounds { get; set; }

        [Required]
        public ThrowingHand Throws { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }

    [Table("pitcherappearances")]
    public class PitcherAppearance
    {
        [Key]
        public int PitcherAppearanceId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        public Pitcher Pitcher { get; set; }

        public PitcherStatLine StatLine { get; set; }

        public PitcherStatSnap PreGameSnapshot { get; set; }
    }

    [Table("pitcherstatlines")]
    public class PitcherStatLine
    {
        [Key]
        public int PitcherStatLineId { get; set; }

        [Required]
        public int Outs { get; set; }

        [Required]
        public int Runs { get; set; }

        [Required]
        public int StrikeOuts { get; set; }

        [Required]
        public int EarnedRuns { get; set; }

        [Required]
        public int BattersFaced { get; set; }

        [Required]
        public int HomeRunsAllowed { get; set; }

        [Required]
        public int Walks { get; set; }

        [Required]
        public int Hits { get; set; }

        [Required]
        public bool Winner { get; set; }

        [Required]
        public bool Loser { get; set; }

        [Required]
        public bool Save { get; set; }

        [Required]
        public bool Hold { get; set; }

        public double WalksPerGame
        {
            get
            {
                return Formulas.WalksPerGame(Walks, Outs);
            }
        }

        public double HitsPerGame
        {
            get
            {
                return Formulas.WalksPerGame(Hits, Outs);
            }
        }

        public double RunAverage
        {
            get
            {
                return Formulas.EarnedRunAverage(Runs, Outs);
            }
        }

        public double StrikoutsPerGame
        {
            get
            {
                return Formulas.StrikoutsPerGame(StrikeOuts, Outs);
            }
        }

        public double HomerunsPerGame
        {
            get
            {
                return Formulas.HomerunsPerGame(HomeRunsAllowed, Outs);
            }
        }

        public PitcherStatLine Plus(PitcherStatLine p)
        {
            return new PitcherStatLine()
            {
                Outs = this.Outs + p.Outs,
                StrikeOuts = this.StrikeOuts + p.StrikeOuts,
                Runs = this.Runs + p.Runs,
                EarnedRuns = this.EarnedRuns + p.EarnedRuns,
                BattersFaced = this.BattersFaced + p.BattersFaced,
                Walks = this.Walks + p.Walks,
                HomeRunsAllowed = this.HomeRunsAllowed + p.HomeRunsAllowed,
                Hits = this.Hits + p.Hits,
                Winner = this.Winner || p.Winner,
                Loser = this.Loser || p.Loser,
                Save = this.Save || p.Save,
                Hold = this.Hold || p.Hold,
            };
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ThrowingHand
    {
        UNKNOWN = 0,
        RIGHT = 1,
        LEFT = 2,
        BOTH = 3
    }

    [Table("pitcherstatsnaps")]
    public class PitcherStatSnap
    {
        [Key]
        public int PitcherStatSnapId { get; set; }

        [Required]
        public SimplePitcherStatLine Career { get; set; }

        [Required]
        public SimplePitcherStatLine Season { get; set; }

        [Required]
        public SimplePitcherStatLine Month { get; set; }

        [Required]
        public SimplePitcherStatLine BasesEmpty { get; set; }

        [Required]
        public SimplePitcherStatLine MenOnBase { get; set; }

        [Required]
        public SimplePitcherStatLine RunnersInScoringPosition { get; set; }

        [Required]
        public SimplePitcherStatLine BasesLoaded { get; set; }

        [Required]
        public SimplePitcherStatLine vsLefties { get; set; }

        [Required]
        public SimplePitcherStatLine vsRighties { get; set; }

        public PitcherStatSnap Plus(PitcherStatSnap sb)
        {
            return new PitcherStatSnap()
            {
                Career = this.Career.Plus(sb.Career),
                Season = this.Season.Plus(sb.Season),
                Month = this.Month.Plus(sb.Month),
                BasesEmpty = this.BasesEmpty.Plus(sb.BasesEmpty),
                MenOnBase = this.MenOnBase.Plus(sb.MenOnBase),
                RunnersInScoringPosition = this.RunnersInScoringPosition.Plus(sb.RunnersInScoringPosition),
                BasesLoaded = this.BasesLoaded.Plus(sb.BasesLoaded),
                vsLefties = this.vsLefties.Plus(sb.vsLefties),
                vsRighties = this.vsRighties.Plus(sb.vsRighties),
            };
        }
    }

    [Table("simplepitcherstatlines")]
    public class SimplePitcherStatLine
    {
        [Key]
        public int SimplePitcherStatLineId { get; set; }

        [Required]
        public double BattingAverage { get; set; }

        [Required]
        public double WHIP { get; set; }

        [Required]
        public int AtBats { get; set; }

        [Required]
        public int StrikeOuts { get; set; }

        [Required]
        public int RunsBattedIn { get; set; }

        [Required]
        public int HomeRunsAllowed { get; set; }

        [Required]
        public int Walks { get; set; }

        [Required]
        public int Hits { get; set; }

        [Required]
        public int StolenBases { get; set; }

        [Required]
        public int Outs { get; set; }

        [Required]
        public double EarnedRunAverage { get; set; }

        [Required]
        public int Wins { get; set; }

        [Required]
        public int Losses { get; set; }

        public double WalksPerGame
        {
            get
            {
                return Formulas.WalksPerGame(Walks, Outs);
            }
        }

        public double HitsPerGame
        {
            get
            {
                return Formulas.WalksPerGame(Hits, Outs);
            }
        }

        public double WalksHitsInningsPitched
        {
            get
            {
                return Formulas.WHIP(Walks, Hits, Outs);
            }
        }

        public double StrikoutsPerGame
        {
            get
            {
                return Formulas.StrikoutsPerGame(StrikeOuts, Outs);
            }
        }

        public double HomerunsPerGame
        {
            get
            {
                return Formulas.HomerunsPerGame(HomeRunsAllowed, Outs);
            }
        }

        private int sumCount = 1;
        public void setSumCount(int count)
        {
            this.sumCount = count;
        }

        public int getSumCount()
        {
            return this.sumCount;
        }

        public SimplePitcherStatLine Plus(SimplePitcherStatLine sl)
        {
            double myMultiplier = (double)this.getSumCount() / (double)(this.getSumCount() + sl.getSumCount());
            double theirMultiplier = 1 - myMultiplier;
            var sb = new SimplePitcherStatLine()
            {
                Hits = this.Hits + sl.Hits,
                Walks = this.Walks + sl.Walks,
                AtBats = this.AtBats + sl.AtBats,
                HomeRunsAllowed = this.HomeRunsAllowed + sl.HomeRunsAllowed,
                StrikeOuts = this.StrikeOuts + sl.StrikeOuts,
                StolenBases = this.StolenBases + sl.StolenBases,
                RunsBattedIn = this.RunsBattedIn + sl.RunsBattedIn,
                Outs = this.Outs + sl.Outs,
                Wins = this.Wins + sl.Wins,
                Losses = this.Losses + sl.Losses,
                BattingAverage = myMultiplier * BattingAverage + theirMultiplier * sl.BattingAverage,
                EarnedRunAverage = myMultiplier * EarnedRunAverage + theirMultiplier * sl.EarnedRunAverage,
                WHIP = myMultiplier * WHIP + theirMultiplier * sl.WHIP,
            };
            sb.setSumCount(sl.getSumCount() + this.getSumCount());
            return sb;
        }
    }
}