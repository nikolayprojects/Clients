using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using ClientsManagement.DAL;
using ClientsManagement.DTO;
using static ClientsManagement.Models.ClientsModelHelper;

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
                {
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
                }

                var clientsTypes = clientsTypesRepository.Get();

                foreach (var item in clientsTypes)
                {
                    dispatcher.Invoke(() => ClientsTypesList.Add(new ClientTypeDTO()
                    {
                        ID = item.ID,
                        Name = item.Name
                    }));
                }
            });
        }

        public async Task AddClientAsync(ClientDTO clientDTO)
        {
            if (clientDTO == null)
                throw new ArgumentNullException(nameof(clientDTO));

            Clients entity = new Clients();

            await ToClientEntity(entity, clientDTO, clientsTypesRepository).ConfigureAwait(false);

            clientsRepository.Insert(entity);

            await unitOfWork.SaveAsync().ConfigureAwait(false);

            dispatcher.Invoke(() => ClientsList.Add(clientDTO));
        }

        public async Task<bool> ChangeClientAsync(ClientDTO clientDTO)
        {
            if (clientDTO == null)
                throw new ArgumentNullException(nameof(clientDTO));

            Clients entity = await clientsRepository.FindByIDAsync(clientDTO.ID).ConfigureAwait(false);

            if (clientsRepository.Update(await ToClientEntity(entity, clientDTO, clientsTypesRepository).ConfigureAwait(false)))
            {
                await unitOfWork.SaveAsync().ConfigureAwait(false);
                return true;
            }

            return false;
        }

        public async Task RemoveClientAsync(ClientDTO clientDTO)
        {
            if (clientDTO == null)
                throw new ArgumentNullException(nameof(clientDTO));

            Clients entity = await clientsRepository.FindByIDAsync(clientDTO.ID).ConfigureAwait(false);

            clientsRepository.Remove(entity);

            await unitOfWork.SaveAsync().ConfigureAwait(false);

            dispatcher.Invoke(() => ClientsList.Remove(clientDTO));
        }

        public async Task AddClientTypeAsync(ClientTypeDTO clientType)
        {
            ClientsTypes entity = new ClientsTypes() { Name = clientType.Name };

            clientsTypesRepository.Insert(entity);

            await unitOfWork.SaveAsync();

            clientType.ID = entity.ID;
        }

        public async Task RemoveClientTypeAsync(ClientTypeDTO clientTypeDTO)
        {
            ClientsTypes entity = await clientsTypesRepository.FindByIDAsync(clientTypeDTO.ID).ConfigureAwait(false);

            clientsTypesRepository.Remove(entity);
            
            await unitOfWork.SaveAsync().ConfigureAwait(false);

            await Task.Run(() =>
            {
                for (int i = ClientsList.Count - 1; i >= 0; i--)
                {
                    if (ClientsList[i].Type.ID == clientTypeDTO.ID)
                        dispatcher.Invoke(() => ClientsList.RemoveAt(i));
                }
            }).ConfigureAwait(false);
        }

        public async Task ChangedClientTypeAsync(ClientTypeDTO clientTypeDTO)
        {
            ClientsTypes entity = await clientsTypesRepository.FindByIDAsync(clientTypeDTO.ID).ConfigureAwait(false);

            clientsTypesRepository.Update(entity);

            await unitOfWork.SaveAsync().ConfigureAwait(false);
        }
    }

    static class ClientsModelHelper
    {
        public static async Task<Clients> ToClientEntity(Clients clientEntity, ClientDTO clientDTO, IRepository<ClientsTypes> repository)
        {
            clientEntity.INN = clientDTO.INN;
            clientEntity.Name = clientDTO.Name;
            clientEntity.DateContract = clientDTO.DateContract;
            clientEntity.Contacts = clientDTO.Contacts;
            clientEntity.ClientsTypes = await repository.FindByIDAsync(clientDTO.Type.ID).ConfigureAwait(false);

            return clientEntity;
        }
    }
}
