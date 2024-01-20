using eGym.Models;
using eGym.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace eGym.Repositories
{
    public class EquipmentService : IEquipmentService
    {
        private readonly DatabaseContext ctx;
        public EquipmentService(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }
        public bool Add(EquipmentModel model)
        {
            try
            {
                ctx.Equipment.Add(model);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(string id)
        {
            try
            {
                var data = this.GetById(id);
                if (data == null)
                    return false;
                ctx.Equipment.Remove(data);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public EquipmentModel GetById(string id)
        {
            return ctx.Equipment.Find(id);
        }

        public IQueryable<EquipmentModel> List()
        {
            var data = ctx.Equipment.AsQueryable();
            return data;
        }

        public bool Update(EquipmentModel model)
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
