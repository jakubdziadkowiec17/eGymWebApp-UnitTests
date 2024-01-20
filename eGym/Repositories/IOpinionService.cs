using eGym.Models;

namespace eGym.Repositories
{
    public interface IOpinionService
    {
        Task<bool> Add(OpinionModel model);
        Task<bool> Update(OpinionModel model);
        OpinionModel GetRecord(string userId, int id);
        OpinionModel GetById(int id);
        bool Delete(int id);
        IQueryable<OpinionModel> List();
    }
}
