using eGym.Models;

namespace eGym.Repositories
{
    public interface ITicketService
    {
        bool Add(TicketModel model);
        bool Update(TicketModel model);
        TicketModel GetById(int id);
        bool Delete(int id);
        IQueryable<TicketModel> List();
    }
}
