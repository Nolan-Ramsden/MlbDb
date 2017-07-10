using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MlbDb.Models
{
    [Table("scoreboards")]
    public class Scoreboard
    {
        [Key]
        public int ScoreboardId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public List<Game> Games { get; set; } = new List<Game>();
    }

    [Table("games")]
    public class Game
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int GameId { get; set; }

        [Required]
        public DateTime Date { get; set; } 

        [Required]
        public GameType Type { get; set; }

        [Required]
        public GameStatus Status { get; set; }

        [Required]
        public TeamAppearance Home { get; set; }

        [Required]
        public TeamAppearance Away { get; set; }

        [Required]
        public Location Location { get; set; }

        public Result Result { get; set; }

        public List<string> Extra { get; set; } = new List<string>();

        [Required]
        public string GameDetailLocation { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GameType
    {
        UNKNOWN = 0,
        SPRING_TRAINING = 1,
        EXHIBITION = 2,
        REGULAR_SEASON = 3,
        PLAYOFFS = 4,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GameStatus
    {
        UNKNOWN = 0,
        PREVIEW = 1,
        IN_PROGRESS = 2,
        RAIN_DELAY = 3,
        RESCHEDULED = 4,
        COMPLETE = 5,
        CANCELED = 6,
        OTHER = 7,
        SCHEDULED = 8,
    }
}
