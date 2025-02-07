using RMS.Application.Service.Interface;
using RMS.Domain.Models;
using RMS.Infrastructure.Data;

namespace RMS.Application.Service
{
    public class MenuRepository : Repository<Menu>, IMenuRepository
    {
        private readonly ApplicationDbContext _db;
        public MenuRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }

        public void Update(Menu items)
        {
            _db.Menus.Update(items);
        }
    }
}
