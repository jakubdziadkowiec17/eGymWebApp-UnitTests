using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eGym.Models
{
    public class AdModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string ImagePath { get; set; }
        public DateTime CreatedDate { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }
        public string UserId { get; set; }
    }
}
