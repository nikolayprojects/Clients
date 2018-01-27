using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientsManagement.DAL;

namespace ClientsManagement.UnitTest.Infrastructure
{
    class FakeClientsTypesRepository : IRepository<ClientsTypes>
    {
        readonly Dictionary<int, ClientsTypes> clientsTypes;

        public FakeClientsTypesRepository(Dictionary<int, ClientsTypes> clientsTypes)
        {
            this.clientsTypes = clientsTypes;
        }

        public Task<ClientsTypes> FindByIDAsync(int id)
        {
            if (clientsTypes.TryGetValue(id, out ClientsTypes value))
            {
                return Task.FromResult(value);
            }

            return Task.FromResult<ClientsTypes>(null);
        }

        public IQueryable<ClientsTypes> Get()
        {
            return clientsTypes.Select(x => x.Value).AsQueryable();
        }

        public void Insert(ClientsTypes entity)
        {
            clientsTypes.Add(entity.ID, entity);
        }

        public void Remove(ClientsTypes entity)
        {
            clientsTypes.Remove(entity.ID);
        }

        public bool Update(ClientsTypes entity)
        {
            if (clientsTypes.TryGetValue(entity.ID, out ClientsTypes value))
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