using eGym.Models;
using eGym.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace eGym.Repositories
{
    public class OpinionService : IOpinionService
    {
        private readonly DatabaseContext ctx;
        public OpinionService(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }
        public async Task<bool> Add(OpinionModel model)
        {
            try
            {
                ctx.Opinion.Add(model);
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
                ctx.Opinion.Remove(data);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public OpinionModel GetRecord(string userId,int id)
        {
            return ctx.Opinion
        .Where(opinion => opinion.UserId == userId && opinion.GymId == id)
        .SingleOrDefault();
        }
        public OpinionModel GetById(int id)
        {
            return ctx.Opinion.Find(id);
        }

        public IQueryable<OpinionModel> List()
        {
            var data = ctx.Opinion.AsQueryable();
            return data;
        }

        public async Task<bool> Update(OpinionModel model)
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
