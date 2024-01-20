using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eGym.Models
{
    public class ClassesUserModel
    {
        public int Id { get; set; }
        public string ? UserId { get; set; }
        [Required]
        public int ClassesId { get; set; }
        public DateTime DateOfReservation { get; set; }
        public DateTime ? PaymentDate { get; set; }
    }
}
