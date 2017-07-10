using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MlbDb.Models
{
    [Table("batters")]
    public class Batter
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BatterId { get; set; }

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

        [Required]
        public BatSide Bats { get; set; }

        [Required]
        public Position NaturalPosition { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }

    [Table("batterappearances")]
    public class BatterAppearance
    {
        [Key]
        public int BatterAppearanceId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        public Batter Batter { get; set; }

        [Required]
        public Position Position { get; set; }

        public BatterStatLine StatLine { get; set; }

        public BatterStatSnap PreGameSnapshot { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum BatSide
    {
        UNKNOWN = 0,
        LEFT = 1,
        RIGHT = 2,
        SWITCH = 3,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Position
    {
        UNKNOWN = 0,
        PITCHER = 1,
        CATCHER = 2,
        FIRST_BASE = 3,
        SECOND_BASE = 4,
        THIRD_BASE = 5,
        SHORTSTOP = 6,
        LEFT_FIELD = 7,
        CENTER_FIELD = 8,
        RIGHT_FIELD = 9,
        DESIGNATED_HITTER = 10,
        PINCH_HITTER = 11,
        PINCH_RUNNER = 12,
        OUTFIELD = 13,
        MULTI = 14,
    }

    [Table("batterstatsnaps")]
    public class BatterStatSnap
    {
        [Key]
        public int BatterStatSnapId { get; set; }

        [Required]
        public SimpleBatterStatLine Career { get; set; }

        [Required]
        public SimpleBatterStatLine Season { get; set; }

        [Required]
        public SimpleBatterStatLine Month { get; set; }

        [Required]
        public SimpleBatterStatLine BasesEmpty { get; set; }

        [Required]
        public SimpleBatterStatLine MenOnBase { get; set; }

        [Required]
        public SimpleBatterStatLine RunnersInScoringPosition { get; set; }

        [Required]
        public SimpleBatterStatLine BasesLoaded { get; set; }

        [Required]
        public SimpleBatterStatLine vsLefties { get; set; }

        [Required]
        public SimpleBatterStatLine vsRighties { get; set; }

        public BatterStatSnap Plus(BatterStatSnap sb)
        {
            return new BatterStatSnap()
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

    [Table("batterstatlines")]
    public class BatterStatLine
    {
        [Key]
        public int BatterStatLineId { get; set; }

        [Required]
        public int AtBats { get; set; }

        [Required]
        public int StrikeOuts { get; set; }

        [Required]
        public int RunsBattedIn { get; set; }

        [Required]
        public int SacrificeFlies { get; set; }

        [Required]
        public int HomeRuns { get; set; }

        [Required]
        public int Walks { get; set; }

        [Required]
        public int HitByPitch { get; set; }

        [Required]
        public int Doubles { get; set; }

        [Required]
        public int Triples { get; set; }

        [Required]
        public int Errors { get; set; }

        [Required]
        public int Assists { get; set; }

        [Required]
        public int Hits { get; set; }

        [Required]
        public int StolenBases { get; set; }

        [Required]
        public int CaughtStealing { get; set; }

        [Required]
        public int GroundOuts { get; set; }

        [Required]
        public int PutOuts { get; set; }

        [Required]
        public int SacrificeBunts { get; set; }

        public double StrikeoutsPerAtBat
        {
            get
            {
                return Formulas.StrikeoutsPerAtBat(StrikeOuts, AtBats);
            }
        }

        public double BattingAverage
        {
            get
            {
                return Formulas.BattingAverage(Hits, AtBats);
            }
        }

        public double OnBasePercentage
        {
            get
            {
                return Formulas.OnBasePercentage(Hits, Walks, HitByPitch, AtBats, SacrificeFlies);
            }
        }

        public double SluggingPercentage
        {
            get
            {
                return Formulas.SluggingPercentage((Hits - Doubles - Triples - HomeRuns), Doubles, Triples, HomeRuns, AtBats);
            }
        }

        public double OnBasePlusSlugging
        {
            get
            {
                return Formulas.OPS(OnBasePercentage, SluggingPercentage);
            }
        }

        public double StolenBasePercentage
        {
            get
            {
                return Formulas.StolenBasePercentage(StolenBases, CaughtStealing);
            }
        }

        public double GroundoutPercentage
        {
            get
            {
                return Formulas.GroundOutPercentage(GroundOuts, AtBats);
            }
        }

        public double HomerunsPerAtBat
        {
            get
            {
                return Formulas.HomerunsPerAtBat(HomeRuns, AtBats);
            }
        }

        public BatterStatLine Plus(BatterStatLine b)
        {
            return new BatterStatLine()
            {
                AtBats = this.AtBats + b.AtBats,
                StrikeOuts = this.StrikeOuts + b.StrikeOuts,
                RunsBattedIn = this.RunsBattedIn + b.RunsBattedIn,
                SacrificeFlies = this.SacrificeFlies + b.SacrificeFlies,
                HomeRuns = this.HomeRuns + b.HomeRuns,
                Walks = this.Walks + b.Walks,
                HitByPitch = this.HitByPitch + b.HitByPitch,
                Doubles = this.Doubles + b.Doubles,
                Triples = this.Triples + b.Triples,
                Errors = this.Errors + b.Errors,
                Assists = this.Assists + b.Assists,
                Hits = this.Hits + b.Hits,
                StolenBases = this.StolenBases + b.StolenBases,
                CaughtStealing = this.CaughtStealing + b.CaughtStealing,
                GroundOuts = this.GroundOuts + b.GroundOuts,
                PutOuts = this.PutOuts + b.PutOuts,
                SacrificeBunts = this.SacrificeBunts + b.SacrificeBunts,
            };
        }
    }

    [Table("simplebatterstatlines")]
    public class SimpleBatterStatLine
    {
        [Key]
        public int SimpleBatterStatLineId { get; set; }

        [Required]
        public double OnBasePlusSlugging { get; set; }

        [Required]
        public int AtBats { get; set; }

        [Required]
        public int StrikeOuts { get; set; }

        [Required]
        public int RunsBattedIn { get; set; }

        [Required]
        public int HomeRuns { get; set; }

        [Required]
        public int Walks { get; set; }

        [Required]
        public int Hits { get; set; }

        [Required]
        public int StolenBases { get; set; }

        [Required]
        public int CaughtStealing { get; set; }

        public double StolenBasePercentage
        {
            get
            {
                return Formulas.StolenBasePercentage(StolenBases, CaughtStealing);
            }
        }

        public double StrikeoutsPerAtBat
        {
            get
            {
                return Formulas.StrikeoutsPerAtBat(StrikeOuts, AtBats);
            }
        }

        public double HomerunsPerAtBat
        {
            get
            {
                return Formulas.HomerunsPerAtBat(HomeRuns, AtBats);
            }
        }

        public double BattingAverage
        {
            get
            {
                return Formulas.BattingAverage(Hits, AtBats);
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

        public SimpleBatterStatLine Plus(SimpleBatterStatLine sl)
        {
            double myMultiplier = (double)this.getSumCount() / (double)(this.getSumCount() + sl.getSumCount());
            double theirMultiplier = 1 - myMultiplier;
            var sb = new SimpleBatterStatLine()
            {
                Hits = this.Hits + sl.Hits,
                Walks = this.Walks + sl.Walks,
                AtBats = this.AtBats + sl.AtBats,
                HomeRuns = this.HomeRuns + sl.HomeRuns,
                StrikeOuts = this.StrikeOuts + sl.StrikeOuts,
                StolenBases = this.StolenBases + sl.StolenBases,
                RunsBattedIn = this.RunsBattedIn + sl.RunsBattedIn,
                CaughtStealing = this.CaughtStealing + sl.CaughtStealing,
                OnBasePlusSlugging = myMultiplier * this.OnBasePlusSlugging + theirMultiplier * sl.OnBasePlusSlugging,
            };
            sb.setSumCount(sl.getSumCount() + this.getSumCount());
            return sb;
        }
    }
}