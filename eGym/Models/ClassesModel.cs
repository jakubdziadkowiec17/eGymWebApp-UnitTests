using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eGym.Models
{
    public class ClassesModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int NumberOfPeople { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public string EmployeeId { get; set; }
        public int GymId { get; set; }

        
    }
}
