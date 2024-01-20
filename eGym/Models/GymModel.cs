using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eGym.Models
{
    public class GymModel
    {
        public int Id { get; set; }
        [Required]
        public string GymName { get; set; }
        public string Locality { get; set; }
        public string OpeningHours { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Map { get; set; }
    }
}
