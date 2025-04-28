using RMS.Application.Service.Interface;
using RMS.Infrastructure.Data;
using RMS.Domain.Models;

namespace RMS.Application.Service
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        private readonly ApplicationDbContext _db;
        public TransactionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Transaction transaction)
        {
            Update(transaction);
        }
    }
}
