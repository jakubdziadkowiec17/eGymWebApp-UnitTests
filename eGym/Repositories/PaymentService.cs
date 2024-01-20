using eGym.Models;
using eGym.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace eGym.Repositories
{
    public class PaymentService : IPaymentService
    {
        private readonly DatabaseContext ctx;
        public PaymentService(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }
        public bool Add(PaymentModel model)
        {
            try
            {
                ctx.Payment.Add(model);
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
                ctx.Payment.Remove(data);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public PaymentModel GetById(int id)
        {
            return ctx.Payment.Find(id);
        }

        public IQueryable<PaymentModel> List()
        {
            var data = ctx.Payment.AsQueryable();
            return data;
        }
    }
}
