using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace eGym.Models
{
    public class UserModel : IdentityUser
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public int GymId { get; set; }
        public string Address { get; set; }
    }
}
