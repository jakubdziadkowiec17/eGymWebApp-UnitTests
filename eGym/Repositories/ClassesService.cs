using eGym.Models;
using eGym.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace eGym.Repositories
{
    public class ClassesService : IClassesService
    {
        private readonly DatabaseContext ctx;
        public ClassesService(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }
        public bool Add(ClassesModel model)
        {
            try
            {
                ctx.Classes.Add(model);
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
                ctx.Classes.Remove(data);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public ClassesModel GetById(int id)
        {
            return ctx.Classes.Find(id);
        }

        public IQueryable<ClassesModel> List()
        {
            var data = ctx.Classes.AsQueryable();
            return data;
        }

        public bool Update(ClassesModel model)
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
