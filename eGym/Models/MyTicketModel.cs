using System.ComponentModel.DataAnnotations;

namespace eGym.Models
{
    public class MyTicketModel
    {
        public int Id { get; set; }
        [Required]
        public int TicketId { get; set; }
        public string? UserId { get; set; }
        [Required]
        public int GymId { get; set; }
        public string TicketName { get; set; }
        public bool ReducedTicket { get; set; }
        public int Price { get; set; }
        public int? NumberOfDays { get; set; }
        public TicketStatus TicketStatus { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string GymName { get; set; }

    }
    public enum TicketStatus
    {
        NotPaid,
        Paid,
        Active
    }
}
