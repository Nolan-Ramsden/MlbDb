using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MlbDb.Models
{
    [Table("teams")]
    public class Team
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TeamId { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public TeamDivision Division { get; set; }

        [Required]
        public TeamLeague League { get; set; }

        [Required]
        public TimeZone Timezone { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TeamDivision
    {
        UNKNOWN = 0,
        EAST, WEST, CENTRAL
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TeamLeague
    {
        UNKNOWN = 0,
        NATIONAL, AMERICAN,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TimeZone
    {
        UNKNOWN = 0,
        EASTERN = -5,
        PACIFIC = -8,
        CENTRAL = -6,
        MOUNTAIN = -7,
        HAWAIIAN = -10,
    }

    [Table("teamappearances")]
    public class TeamAppearance
    {
        [Key]
        public int TeamAppearanceId { get; set; }

        [Required]
        public Team Team { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public List<PitcherAppearance> Pitchers { get; set; } = new List<PitcherAppearance>();

        public List<BatterAppearance> Batters { get; set; } = new List<BatterAppearance>();

        public Standings Standings { get; set; }

        public PitcherAppearance StartingPitcher
        {
            get
            {
                if (!Pitchers.Any())
                {
                    return null;
                }
                return Pitchers.Single(p => p.Order == 1);
            }
        }

        public PitcherStatLine PitchingTotals
        {
            get
            {
                var stats = Pitchers.Select(p => p.StatLine).Where(p => p != null);
                if (stats.Any())
                {
                    return stats.Aggregate((p1, p2) => p1.Plus(p2));
                }
                else
                {
                    return new PitcherStatLine();
                }
            }
        }

        public BatterStatLine BattingTotals
        {
            get
            {
                var stats = Batters.Select(b => b.StatLine).Where(b => b != null);
                if (stats.Any())
                {
                    return stats.Aggregate((p1, p2) => p1.Plus(p2));
                }
                else
                {
                    return new BatterStatLine();
                }
            }
        }

        public BatterStatSnap TeamBattingStatSnap
        {
            get
            {
                var stats = Batters.Select(b => b.PreGameSnapshot).Where(b => b != null);
                if (stats.Any())
                {
                    return stats.Aggregate((p1, p2) => p1.Plus(p2));
                }
                else
                {
                    return new BatterStatSnap();
                }
            }
        }

        public int BullpenOuts
        {
            get
            {
                var pitchers = Pitchers.Except(new PitcherAppearance[] { this.StartingPitcher });
                return pitchers.Sum(p => p.StatLine.Outs);
            }
        }

        public PitcherStatSnap TeamPitchingStatSnapNoStarter
        {
            get
            {
                var pitchers = Pitchers.Except(new PitcherAppearance[] { this.StartingPitcher });
                var stats = pitchers.Select(b => b.PreGameSnapshot).Where(b => b != null);
                if (stats.Any())
                {
                    return stats.Aggregate((p1, p2) => p1.Plus(p2));
                }
                else
                {
                    return new PitcherStatSnap()
                    {
                        Career = new SimplePitcherStatLine(),
                        Season = new SimplePitcherStatLine(),
                        RunnersInScoringPosition = new SimplePitcherStatLine()
                    };
                }
            }
        }
    }
}