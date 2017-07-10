using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MlbDb.Models
{
    [Table("locations")]
    public class Location
    {
        [Key]
        public int LocationId { get; set; }

        [Required]
        public string Place { get; set; }

        [Required]
        public string Venue { get; set; }

        [Required]
        public TimeZone Timezone { get; set; }
    }
}