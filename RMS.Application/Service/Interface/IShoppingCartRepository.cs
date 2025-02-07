using RMS.Domain.Models;

namespace RMS.Application.Service.Interface
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        void Update(ShoppingCart items);
    }
}
