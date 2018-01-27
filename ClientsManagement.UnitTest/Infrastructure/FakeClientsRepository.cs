using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientsManagement.DAL;

namespace ClientsManagement.UnitTest.Infrastructure
{
    class FakeClientsRepository : IRepository<Clients>
    {
        readonly Dictionary<int, Clients> clients;

        public FakeClientsRepository(Dictionary<int, Clients> clients)
        {
            this.clients = clients;
        }

        public Task<Clients> FindByIDAsync(int id)
        {
            if (clients.TryGetValue(id, out Clients value))
            {
                return Task.FromResult(value);
            }

            return Task.FromResult<Clients>(null);
        }

        public IQueryable<Clients> Get()
        {
            return clients.Select(x => x.Value).AsQueryable();
        }

        public void Insert(Clients entity)
        {
            clients.Add(entity.ID, entity);
        }

        public void Remove(Clients entity)
        {
            clients.Remove(entity.ID);
        }

        public bool Update(Clients entity)
        {
            if (clients.TryGetValue(entity.ID, out Clients value))
            {
                value = entity;
            }
            else
            {
                throw new ArgumentException($"Запись с ID {entity.ID} отсутствует в словаре.");
            }

            return true;
        }
    }
}