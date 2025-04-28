using RMS.Application.Service.Interface;
using RMS.Domain.Models;
using RMS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Application.Service
{
    public class InventoryRepository : Repository<Inventory>, IInventoryRepository
    {
        private readonly ApplicationDbContext _db;
        public InventoryRepository(ApplicationDbContext db): base(db) 
        {   
            _db = db;  
        }

        public void Update(Inventory inventory)
        {
            _db.Update(inventory);
        }
    }
}
