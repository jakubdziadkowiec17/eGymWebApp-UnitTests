using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eGym.Models
{
    public class PaymentModel
    {
        public int Id { get; set; }
        [Required]
        public int GymId { get; set; }
        [Required]
        public bool Deposit { get; set; }
        [Required]
        public int Sum { get; set; }
        public string Source { get; set; }
        public string ? SourceId { get; set; }
        public string ? Description { get; set; }
        public DateTime PaymentDate { get; set; }
        public string UserEnteringThePayment { get; set; }
    }
}
