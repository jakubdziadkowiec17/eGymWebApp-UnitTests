using System.ComponentModel.DataAnnotations;

namespace eGym.Models
{
    public class TicketModel
    {
        public int Id { get; set; }
        [Required]
        public string TicketName { get; set; }
        public bool ReducedTicket { get; set; }
        public int Price { get; set; }
        public int? NumberOfDays { get; set; }

    }
}
