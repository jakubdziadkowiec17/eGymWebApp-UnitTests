using eGym.Models;

namespace eGym.Repositories
{
    public interface IMyTicketService
    {
        Task<bool> Add(MyTicketModel model);
        Task<bool> Update(MyTicketModel model);
        MyTicketModel GetById(int id);
        bool Delete(int id);
        IQueryable<MyTicketModel> List();
    }
}
