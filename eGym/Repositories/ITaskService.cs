using eGym.Models;

namespace eGym.Repositories
{
    public interface ITaskService
    {
        bool Add(TaskModel model);
        bool Update(TaskModel model);
        TaskModel GetById(int id);
        bool Delete(int id);
        IQueryable<TaskModel> List();
    }
}
