using eGym.Models;

namespace eGym.Repositories
{
    public interface IPaymentService
    {
        bool Add(PaymentModel model);
        PaymentModel GetById(int id);
        bool Delete(int id);
        IQueryable<PaymentModel> List();
    }
}
