using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MlbDb.Models
{
    [Table("standings")]
    public class Standings
    {
        [Key]
        public int StandingsId { get; set; }

        [Required]
        public int Points { get; set; }

        [Required]
        public int Place { get; set; }

        [Required]
        public WinLossSplit Totals { get; set; }

        [Required]
        public WinLossSplit VsLeft { get; set; }

        [Required]
        public WinLossSplit VsCentral { get; set; }

        [Required]
        public WinLossSplit Home { get; set; }

        [Required]
        public WinLossSplit VsRight { get; set; }

        [Required]
        public WinLossSplit VsDivision { get; set; }

        [Required]
        public WinLossSplit VsEast { get; set; }

        [Required]
        public WinLossSplit ExtraInnings { get; set; }

        [Required]
        public WinLossSplit InterLeague { get; set; }

        [Required]
        public WinLossSplit VsWest { get; set; }

        [Required]
        public WinLossSplit LastTen { get; set; }

        [Required]
        public WinLossSplit OneRunGames { get; set; }

        [Required]
        public WinLossSplit Away { get; set; }
    }

    [Table("winlosssplits")]
    public class WinLossSplit
    {
        [Key]
        public int WinLossSplitId { get; set; }

        [Required]
        public int Wins { get; set; }

        [Required]
        public int Losses { get; set; }

        public decimal Percentage
        {
            get
            {
                if (Wins + Losses <= 0)
                {
                    return 0;
                }
                return ((decimal)Wins) / ((decimal)Wins + Losses);
            }
        }
    }
}