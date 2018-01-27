using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientsManagement.DAL;
using ClientsManagement.DTO;
using ClientsManagement.Util;
using static ClientsManagement.Models.ClientsModelHelper;

namespace ClientsManagement.Models
{
    public class ClientsModel
    {
        readonly IRepository<Clients> clientsRepository;
        readonly IRepository<ClientsTypes> clientsTypesRepository;
        readonly IClientsUnitOfWork unitOfWork;
        readonly ICollectionWrapper<ClientDTO> colwrapClientDTO;
        readonly ICollectionWrapper<ClientTypeDTO> colwrapClientTypeDTO;

        public readonly IList<ClientDTO> ClientsList;
        public readonly IList<ClientTypeDTO> ClientsTypesList;

        public ClientsModel(IClientsUnitOfWork unitOfWork, ICollectionWrapperFactory collectionWrapperFactory)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork));

            if (collectionWrapperFactory == null)
                throw new ArgumentNullException(nameof(collectionWrapperFactory));

            colwrapClientDTO = collectionWrapperFactory.Create<ClientDTO>();
            colwrapClientTypeDTO = collectionWrapperFactory.Create<ClientTypeDTO>();

            ClientsList = colwrapClientDTO.Collection;
            ClientsTypesList = colwrapClientTypeDTO.Collection;
            
            clientsRepository = unitOfWork.ClientsRepository;
            clientsTypesRepository = unitOfWork.ClientsTypesRepository;

            this.unitOfWork = unitOfWork;
        }

        public Task LoadAllAsync()
        {
            Dictionary<int, ClientTypeDTO> clientsTypes = new Dictionary<int, ClientTypeDTO>();

            return Task.Run(() =>
            {
                var clientsTypesDTO = clientsTypesRepository.Get();

                foreach (var item in clientsTypesDTO)
                {
                    ClientTypeDTO clientTypeDTO = new ClientTypeDTO()
                    {
                        ID = item.ID,
                        Name = item.Name
                    };

                    colwrapClientTypeDTO.Add(clientTypeDTO);

                    clientsTypes.Add(item.ID, clientTypeDTO);
                }

                var clientsDTO = clientsRepository.Get();

                foreach (var item in clientsDTO)
                {
                    ClientDTO clientDTO = new ClientDTO()
                    {
                        ID = item.ID,
                        INN = item.INN,
                        Name = item.Name,
                        DateContract = item.DateContract,
                        Contacts = item.Contacts,
                        Type = clientsTypes[item.Type]
                    };

                    colwrapClientDTO.Add(clientDTO);
                }
            });
        }

        public async Task AddClientAsync(ClientDTO clientDTO)
        {
            if (clientDTO == null)
                throw new ArgumentNullException(nameof(clientDTO));

            if (ClientsTypesList.Count == 0)
                throw new InvalidOperationException("Операция не может быть выполнена, т.к. список клиентов пуст.");

            Clients entity = new Clients();

            await ClientDTOToClientEntity(entity, clientDTO, clientsTypesRepository).ConfigureAwait(false);

            clientsRepository.Insert(entity);

            await unitOfWork.SaveAsync().ConfigureAwait(false);

            clientDTO.ID = entity.ID;

            colwrapClientDTO.Add(clientDTO);
        }

        public async Task<bool> UpdateClientAsync(ClientDTO clientDTO)
        {
            if (clientDTO == null)
                throw new ArgumentNullException(nameof(clientDTO));

            if (ClientsTypesList.Count == 0)
                throw new InvalidOperationException("Операция не может быть выполнена, т.к. список клиентов пуст.");

            Clients entity = await clientsRepository.FindByIDAsync(clientDTO.ID).ConfigureAwait(false);

            await ClientDTOToClientEntity(entity, clientDTO, clientsTypesRepository).ConfigureAwait(false);

            if (clientsRepository.Update(entity))
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

            colwrapClientDTO.Remove(clientDTO);
        }

        public async Task AddClientTypeAsync(ClientTypeDTO clientTypeDTO)
        {
            ClientsTypes entity = new ClientsTypes() { Name = clientTypeDTO.Name };

            clientsTypesRepository.Insert(entity);

            await unitOfWork.SaveAsync();

            clientTypeDTO.ID = entity.ID;
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
                        colwrapClientDTO.RemoveAt(i);
                }

            }).ConfigureAwait(false);
        }

        public async Task UpdateClientTypeAsync(ClientTypeDTO clientTypeDTO)
        {
            ClientsTypes entity = await clientsTypesRepository.FindByIDAsync(clientTypeDTO.ID).ConfigureAwait(false);
            entity.Name = clientTypeDTO.Name;

            clientsTypesRepository.Update(entity);

            await unitOfWork.SaveAsync().ConfigureAwait(false);
        }
    }

    static class ClientsModelHelper
    {
        public static async Task ClientDTOToClientEntity(Clients clientEntity, ClientDTO clientDTO, IRepository<ClientsTypes> repository)
        {
            clientEntity.INN = clientDTO.INN;
            clientEntity.Name = clientDTO.Name;
            clientEntity.DateContract = clientDTO.DateContract;
            clientEntity.Contacts = clientDTO.Contacts;
            clientEntity.ClientsTypes = await repository.FindByIDAsync(clientDTO.Type.ID).ConfigureAwait(false);
        }
    }
}
