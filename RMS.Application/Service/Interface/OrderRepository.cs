using RMS.Domain.Models;
using RMS.Infrastructure.Data;

namespace RMS.Application.Service.Interface
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderRepository(ApplicationDbContext db):base(db)
        {
            _db = db;  
        }

        public void Update(Order obj)
        {
            _db.Orders.Add(obj);  
        }
    }
}
