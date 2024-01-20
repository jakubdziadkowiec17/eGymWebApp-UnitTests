using eGym.Models;
using eGym.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace eGym.Repositories
{
    public class ClassesUserService : IClassesUserService
    {
        private readonly DatabaseContext ctx;
        public ClassesUserService(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }
        public bool Add(ClassesUserModel model)
        {
            try
            {
                ctx.ClassesUser.Add(model);
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
                ctx.ClassesUser.Remove(data);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public ClassesUserModel GetById(int id)
        {
            return ctx.ClassesUser.Find(id);
        }

        public IQueryable<ClassesUserModel> List()
        {
            var data = ctx.ClassesUser.AsQueryable();
            return data;
        }

        public bool Update(ClassesUserModel model)
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
