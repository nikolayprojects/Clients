using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using ClientsManagement.DAL;
using ClientsManagement.DTO;

namespace ClientsManagement.Models
{
    public class ClientsModel
    {
        readonly IRepository<Clients> clientsRepository;
        readonly IRepository<ClientsTypes> clientsTypesRepository;
        readonly IClientsUnitOfWork unitOfWork;
        readonly Dispatcher dispatcher;

        public readonly ObservableCollection<ClientDTO> ClientsList;
        public readonly ObservableCollection<ClientTypeDTO> ClientsTypesList;

        public ClientsModel(IClientsUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            ClientsList = new ObservableCollection<ClientDTO>();
            ClientsTypesList = new ObservableCollection<ClientTypeDTO>();

            clientsRepository = unitOfWork.ClientsRepository;
            clientsTypesRepository = unitOfWork.ClientsTypesRepository;

            dispatcher = Dispatcher.CurrentDispatcher;

            this.unitOfWork = unitOfWork;
        }

        public Task LoadAllAsync()
        {
            return Task.Run(() =>
            {
                var clients = clientsRepository.Get();

                foreach (var item in clients)
                    dispatcher.Invoke(() => ClientsList.Add(new ClientDTO()
                    {
                        ID = item.ID,
                        INN = item.INN,
                        Name = item.Name,
                        DateContract = item.DateContract,
                        Contacts = item.Contacts,
                        Type = new ClientTypeDTO()
                        {
                            ID = item.ClientsTypes.ID,
                            Name = item.ClientsTypes.Name
                        }
                    }));

                var clientsTypes = clientsTypesRepository.Get();

                foreach (var item in clientsTypes)
                    dispatcher.Invoke(() => ClientsTypesList.Add(new ClientTypeDTO()
                    {
                        ID = item.ID,
                        Name = item.Name
                    }));
            });
        }

        async Task<Clients> ToClientEntity(Clients clientEntity, ClientDTO clientDTO)
        {
            clientEntity.INN = clientDTO.INN;
            clientEntity.Name = clientDTO.Name;
            clientEntity.DateContract = clientDTO.DateContract;
            clientEntity.Contacts = clientDTO.Contacts;
            clientEntity.ClientsTypes = await clientsTypesRepository.FindByIDAsync(clientDTO.Type.ID).ConfigureAwait(false);

            return clientEntity;
        }

        public async Task AddClientAsync(ClientDTO client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            clientsRepository.Insert(await ToClientEntity(new Clients(), client).ConfigureAwait(false));

            await unitOfWork.SaveAsync().ConfigureAwait(false);

            dispatcher.Invoke(() => ClientsList.Add(client));
        }

        public async Task<bool> ChangeClientAsync(ClientDTO client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            Clients entity = await clientsRepository.FindByIDAsync(client.ID).ConfigureAwait(false);

            if (clientsRepository.Update(await ToClientEntity(entity, client).ConfigureAwait(false)))
            {
                await unitOfWork.SaveAsync().ConfigureAwait(false);
                return true;
            }

            return false;
        }

        public async Task RemoveClientAsync(ClientDTO client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            Clients entity = await clientsRepository.FindByIDAsync(client.ID).ConfigureAwait(false);

            clientsRepository.Remove(entity);

            await unitOfWork.SaveAsync().ConfigureAwait(false);

            dispatcher.Invoke(() => ClientsList.Remove(client));
        }
    }
}
