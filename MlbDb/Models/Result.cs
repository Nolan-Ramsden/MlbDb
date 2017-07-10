using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MlbDb.Models
{
    [Table("results")]
    public class Result
    {
        [Key]
        public int ResultId { get; set; }

        [Required]
        public GameResult Home { get; set; } = new GameResult();

        [Required]
        public GameResult Away { get; set; } = new GameResult();

        [Required]
        public List<InningResult> Innings { get; set; } = new List<InningResult>();

        public string Winner
        {
            get
            {
                if (Home.Runs > Away.Runs)
                {
                    return "Home";
                }
                else if (Away.Runs > Home.Runs)
                {
                    return "Away";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }

    [Table("inningresults")]
    public class InningResult
    {
        [Key]
        public int InningResultId { get; set; }

        [Required]
        public int InningIndex { get; set; }

        [Required]
        public int HomeScores { get; set; }

        [Required]
        public int AwayScores { get; set; }
    }

    [Table("gameresults")]
    public class GameResult
    {
        [Key]
        public int GameResultId { get; set; }

        [Required]
        public int Runs { get; set; }

        [Required]
        public int Hits { get; set; }

        [Required]
        public int Errors { get; set; }

        [Required]
        public int StolenBases { get; set; }

        [Required]
        public int StrikeOuts { get; set; }

        [Required]
        public int HomeRuns { get; set; }
    }
}