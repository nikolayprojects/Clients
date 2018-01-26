using System;
using System.Threading.Tasks;
using ClientsManagement.DAL;

namespace ClientsManagement.Models
{
    public interface IUnitOfWork : IDisposable
    {
        Task SaveAsync();
    }

    public interface IClientsUnitOfWork : IUnitOfWork
    {
        IRepository<Clients> ClientsRepository { get; }
        IRepository<ClientsTypes> ClientsTypesRepository { get; }
    }

    public class ClientsUnitOfWork : IClientsUnitOfWork
    {
        readonly ClientsEntities context;
        IRepository<DAL.Clients> clientsRepository;
        IRepository<ClientsTypes> clientsTypesRepository;

        public IRepository<Clients> ClientsRepository
        {
            get { return clientsRepository ?? (clientsRepository = new ClientsRepository(context)); }
        }

        public IRepository<ClientsTypes> ClientsTypesRepository
        {
            get { return clientsTypesRepository ?? (clientsTypesRepository = new ClientsTypesRepository(context)); }
        }

        public ClientsUnitOfWork()
        {
            context = new ClientsEntities();
        }

        public Task SaveAsync()
        {
            return context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}