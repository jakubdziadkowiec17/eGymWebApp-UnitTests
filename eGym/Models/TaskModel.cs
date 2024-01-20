using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eGym.Models
{
    public class TaskModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public string UserId { get; set; }
        public TaskStatus TaskStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ? EndDate { get; set; }
    }
    public enum TaskStatus
    {
        Created,
        InProgress,
        Completed
    }
}
