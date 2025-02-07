using RMS.Domain.Models;

namespace RMS.Application.Service.Interface
{
    public interface IMenuRepository : IRepository<Menu>
    {
        void Update(Menu items);
    }
}
