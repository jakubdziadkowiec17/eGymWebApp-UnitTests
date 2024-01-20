using eGym.Models;
using eGym.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace eGym.Repositories
{
    public class TaskService : ITaskService
    {
        private readonly DatabaseContext ctx;
        public TaskService(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }
        public bool Add(TaskModel model)
        {
            try
            {
                ctx.Task.Add(model);
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
                ctx.Task.Remove(data);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public TaskModel GetById(int id)
        {
            return ctx.Task.Find(id);
        }

        public IQueryable<TaskModel> List()
        {
            var data = ctx.Task.AsQueryable();
            return data;
        }

        public bool Update(TaskModel model)
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
