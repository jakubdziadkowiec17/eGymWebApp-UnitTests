using System.ComponentModel.DataAnnotations;

namespace eGym.Models
{
    public class ResetPasswordAdminModel
    {
        public string UserId { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }
    }
}
