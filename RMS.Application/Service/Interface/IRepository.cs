using System.Linq.Expressions;

namespace RMS.Application.Service.Interface
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T,bool>> ? filter = null);
        T GetById(Expression<Func<T, bool>> filter);
        void  Add(T entity);
        void Delete(T entity);
    }
}
