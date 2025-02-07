using RMS.Domain.Models;

namespace RMS.Application.Service.Interface
{
    public interface IOrderDetailRepository  : IRepository<OrderDetail>
    {
        void Update(OrderDetail obj);
    }
}
