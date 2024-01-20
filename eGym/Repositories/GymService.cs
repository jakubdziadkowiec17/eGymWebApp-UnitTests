using eGym.Models;
using eGym.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace eGym.Repositories
{
    public class GymService : IGymService
    {
        private readonly DatabaseContext ctx;
        public GymService(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }
        public bool Add(GymModel model)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                ctx.Gym.Add(model);
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
                ctx.Gym.Remove(data);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public GymModel GetById(int id)
        {
            return ctx.Gym.Find(id);
        }

        public IQueryable<GymModel> List()
        {
            var data = ctx.Gym.AsQueryable();
            data = data.OrderByDescending(gym => gym.CreatedDate);
            return data;
        }

        public bool Update(GymModel model)
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
