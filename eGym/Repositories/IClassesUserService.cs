using eGym.Models;

namespace eGym.Repositories
{
    public interface IClassesUserService
    {
        bool Add(ClassesUserModel model);
        bool Update(ClassesUserModel model);
        ClassesUserModel GetById(int id);
        bool Delete(int id);
        IQueryable<ClassesUserModel> List();
    }
}
