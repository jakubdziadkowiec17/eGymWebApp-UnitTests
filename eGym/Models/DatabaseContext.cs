using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace eGym.Models
{
    public class DatabaseContext : IdentityDbContext<UserModel>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
        public DbSet<GymModel> Gym { get; set; }
        public DbSet<TicketModel> Ticket { get; set; }
        public DbSet<MyTicketModel> MyTicket { get; set; }
        public DbSet<OpinionModel> Opinion { get; set; }
        public DbSet<AdModel> Ad { get; set; }
        public DbSet<NewsModel> News { get; set; }
        public DbSet<TaskModel> Task { get; set; }
        public DbSet<EquipmentModel> Equipment { get; set; }
        public DbSet<PaymentModel> Payment { get; set; }
        public DbSet<ClassesModel> Classes { get; set; }
        public DbSet<ClassesUserModel> ClassesUser { get; set; }
    }
}
