using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ClientsManagement.DAL
{
    class ClientsRepository : IRepository<Clients>
    {
        readonly ClientsEntities context;
        readonly DbSet<Clients> clients;

        public ClientsRepository(ClientsEntities context)
        {
            clients = context.Clients;
            this.context = context;
        }

        public IQueryable<Clients> Get()
        {
            return clients;
        }

        public void Insert(Clients entity)
        {
            clients.Add(entity);
        }

        public bool Update(Clients entity)
        {
            return context.Entry(entity).State == EntityState.Modified;
        }

        public void Remove(Clients entity)
        {
            clients.Remove(entity);
        }

        public Task<Clients> FindByIDAsync(int id)
        {
            return clients.FindAsync(id);
        }
    }
}