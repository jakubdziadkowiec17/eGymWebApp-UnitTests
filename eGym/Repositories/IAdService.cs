using eGym.Models;

namespace eGym.Repositories
{
    public interface IAdService
    {
        Task<bool> Add(AdModel model);
        AdModel GetById(int id);
        bool Delete(int id);
        IQueryable<AdModel> List();
    }
}
