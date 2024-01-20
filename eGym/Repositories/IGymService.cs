using eGym.Models;

namespace eGym.Repositories
{
    public interface IGymService
    {
        bool Add(GymModel model);
        bool Update(GymModel model);
        GymModel GetById(int id);
        bool Delete(int id);
        IQueryable<GymModel> List();
    }
}
