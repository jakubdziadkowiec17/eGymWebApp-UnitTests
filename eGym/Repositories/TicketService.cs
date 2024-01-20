using eGym.Models;
using eGym.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace eGym.Repositories
{
    public class TicketService : ITicketService
    {
        private readonly DatabaseContext ctx;
        public TicketService(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }
        public bool Add(TicketModel model)
        {
            try
            {
                ctx.Ticket.Add(model);
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
                ctx.Ticket.Remove(data);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public TicketModel GetById(int id)
        {
            return ctx.Ticket.Find(id);
        }

        public IQueryable<TicketModel> List()
        {
            var data = ctx.Ticket.AsQueryable();
            return data;
        }

        public bool Update(TicketModel model)
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
