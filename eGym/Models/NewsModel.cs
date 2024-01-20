using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eGym.Models
{
    public class NewsModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; }
    }
}
