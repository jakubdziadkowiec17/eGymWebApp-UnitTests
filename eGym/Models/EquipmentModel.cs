using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eGym.Models
{
    public class EquipmentModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Required]
        public int GymId { get; set; }
        public EquipmentStatus EquipmentStatus { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
    public enum EquipmentStatus
    {
        InUse,
        Broken,
        InRepair,
        Withdrawn
    }
}
