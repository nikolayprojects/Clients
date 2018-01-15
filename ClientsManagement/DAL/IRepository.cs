using System.Linq;
using System.Threading.Tasks;

namespace ClientsManagement.DAL
{
    public interface IRepository<T>
    {
        IQueryable<T> Get();
        void Insert(T entity);
        bool Update(T entity);
        void Remove(T entity);
        Task<T> FindByIDAsync(int id);
    }
}