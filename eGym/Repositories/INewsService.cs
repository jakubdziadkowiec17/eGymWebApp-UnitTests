using eGym.Models;

namespace eGym.Repositories
{
    public interface INewsService
    {
        bool Add(NewsModel model);
        bool Update(NewsModel model);
        NewsModel GetById(int id);
        bool Delete(int id);
        IQueryable<NewsModel> List();
    }
}
