using System.ComponentModel.DataAnnotations;

namespace eGym.Models
{
    public class ResetPasswordModel
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }
    }
}
