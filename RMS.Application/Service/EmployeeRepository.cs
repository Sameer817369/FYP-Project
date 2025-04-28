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
    public class EmployeeRepository : Repository<Employee>, IEmoloyeeRepository
    {
        private readonly ApplicationDbContext _db;
        public EmployeeRepository(ApplicationDbContext db): base(db)
        {
            _db = db; 
        }
        public void Update(Employee employee)
        {
            _db.Update(employee);
        }
    }
}
