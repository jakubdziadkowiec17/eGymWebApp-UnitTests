using eGym.Models;
using eGym.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace eGym.Repositories
{
    public class NewsService : INewsService
    {
        private readonly DatabaseContext ctx;
        public NewsService(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }
        public bool Add(NewsModel model)
        {
            try
            {
                ctx.News.Add(model);
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
                ctx.News.Remove(data);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public NewsModel GetById(int id)
        {
            return ctx.News.Find(id);
        }

        public IQueryable<NewsModel> List()
        {
            var data = ctx.News.AsQueryable();
            return data;
        }

        public bool Update(NewsModel model)
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
