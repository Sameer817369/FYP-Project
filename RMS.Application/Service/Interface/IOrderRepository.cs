using RMS.Domain.Models;

namespace RMS.Application.Service.Interface
{
    public interface IOrderRepository : IRepository<Order>
    {
        void Update(Order obj);
    }
}
