using RMS.Domain.Models;

namespace RMS.Application.Service.Interface
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        void Update(Transaction transaction);

    }
}
