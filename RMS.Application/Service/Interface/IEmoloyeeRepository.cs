using RMS.Domain.Models;

namespace RMS.Application.Service.Interface
{
    public interface IEmoloyeeRepository : IRepository<Employee>
    {
        void Update(Employee employee);
    }
}
