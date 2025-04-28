using RMS.Domain.Models;

namespace RMS.Application.Service.Interface
{
    public interface IInventoryRepository : IRepository<Inventory>
    {
        void Update(Inventory inventory);
    }
}
