using eGym.Models;

namespace eGym.Repositories
{
    public interface IEquipmentService
    {
        bool Add(EquipmentModel model);
        bool Update(EquipmentModel model);
        EquipmentModel GetById(string id);
        bool Delete(string id);
        IQueryable<EquipmentModel> List();
    }
}
