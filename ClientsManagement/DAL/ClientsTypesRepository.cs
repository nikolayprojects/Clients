using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ClientsManagement.DAL
{
    class ClientsTypesRepository : IRepository<ClientsTypes>
    {
        readonly ClientsEntities context;
        readonly DbSet<ClientsTypes> clientsTypes;

        public ClientsTypesRepository(ClientsEntities context)
        {
            clientsTypes = context.ClientsTypes;
            this.context = context;
        }

        public IQueryable<ClientsTypes> Get()
        {
            return clientsTypes;
        }

        public Task<ClientsTypes> FindByIDAsync(int id)
        {
            return clientsTypes.FindAsync(id);
        }

        public void Insert(ClientsTypes entity)
        {
            clientsTypes.Add(entity);
        }

        public void Remove(ClientsTypes entity)
        {
            clientsTypes.Remove(entity);
        }

        public bool Update(ClientsTypes entity)
        {
            return context.Entry(entity).State == EntityState.Modified;
        }
    }
}