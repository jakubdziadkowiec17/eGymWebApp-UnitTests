using eGym.Models;
using eGym.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace eGym.Repositories
{
    public class AdService : IAdService
    {
        private readonly DatabaseContext ctx;
        public AdService(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }
        public async Task<bool> Add(AdModel model)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                ctx.Ad.Add(model);
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
                ctx.Ad.Remove(data);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public AdModel GetById(int id)
        {
            return ctx.Ad.Find(id);
        }

        public IQueryable<AdModel> List()
        {
            var data = ctx.Ad.AsQueryable();
            data = data.OrderByDescending(ad => ad.CreatedDate);
            return data;
        }
    }
}
