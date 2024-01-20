using eGym.Models;

namespace eGym.Repositories
{
    public interface IClassesService
    {
        bool Add(ClassesModel model);
        bool Update(ClassesModel model);
        ClassesModel GetById(int id);
        bool Delete(int id);
        IQueryable<ClassesModel> List();
    }
}
