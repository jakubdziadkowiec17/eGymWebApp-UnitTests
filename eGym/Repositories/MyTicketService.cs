using eGym.Models;
using eGym.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace eGym.Repositories
{
    public class MyTicketService : IMyTicketService
    {
        private readonly DatabaseContext ctx;
        public MyTicketService(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }
        public async Task<bool> Add(MyTicketModel model)
        {
            try
            {
                ctx.MyTicket.Add(model);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var data = this.GetById(id);
                if (data == null)
                    return false;
                ctx.MyTicket.Remove(data);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public MyTicketModel GetById(int id)
        {
            return ctx.MyTicket.Find(id);
        }

        public IQueryable<MyTicketModel> List()
        {
            var data = ctx.MyTicket.AsQueryable();
            data = data.OrderByDescending(myTicket => myTicket.Id);
            return data;
        }

        public async Task<bool> Update(MyTicketModel model)
        {
            try
            {
                ctx.Update(model);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
