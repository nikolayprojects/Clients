using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientsManagement.DAL;
using ClientsManagement.Models;

namespace ClientsManagement.UnitTest.Infrastructure
{
    class FakeClientsUnitOfWork : IClientsUnitOfWork
    {
        readonly Dictionary<int, Clients> clients;
        readonly Dictionary<int, ClientsTypes> clientsTypes;

        public IRepository<Clients> ClientsRepository { get; }
        public IRepository<ClientsTypes> ClientsTypesRepository { get; }

        public FakeClientsUnitOfWork()
        {
            clientsTypes = new Dictionary<int, ClientsTypes>();
            clients = new Dictionary<int, Clients>();

            ClientsRepository = new FakeClientsRepository(clients);
            ClientsTypesRepository = new FakeClientsTypesRepository(clientsTypes);
        }

        public void LoadTestData()
        {
            clientsTypes.Add(1, new ClientsTypes() { ID = 1, Name = "Физ.лицо" });
            clientsTypes.Add(2, new ClientsTypes() { ID = 2, Name = "Юр.лицо" });

            clients.Add(1, new Clients()
            {
                ID = 1,
                INN = "111111111110",
                Name = "Новый клиент 1",
                DateContract = new DateTime(2018, 01, 10),
                Contacts = "Нижний Новгород Ивлева",
                Type = clientsTypes[1].ID,
                ClientsTypes = clientsTypes[1]
            });

            clients.Add(2, new Clients()
            {
                ID = 2,
                INN = "111111111111",
                Name = "Новый клиент 2",
                DateContract = new DateTime(2018, 01, 10),
                Contacts = "Нижний Новгород Ванеева",
                Type = clientsTypes[2].ID,
                ClientsTypes = clientsTypes[2]
            });
        }

        public void Dispose()
        {

        }

        public Task SaveAsync()
        {
            return Task.FromResult<object>(null);
        }
    }
}
