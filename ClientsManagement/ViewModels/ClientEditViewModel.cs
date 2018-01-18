using System;
using System.Collections.ObjectModel;
using ClientsManagement.DTO;
using ClientsManagement.Models;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.Interfaces.ViewModels;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.ViewModels;

namespace ClientsManagement.ViewModels
{
    public class ClientEditViewModel : ViewModelBase
    {
        enum EditType { Add, Change };
        EditType editType;
        ClientDTO client;
        ClientsModel clientsModel;

        #region ~Свойства~
        public RelayCommand CommandSave { get; }

        public string INN
        {
            get { return client.INN; }
            set { client.INN = value; }
        }

        public string Name
        {
            get { return client.Name; }
            set { client.Name = value; }
        }

        public DateTime DateContract
        {
            get { return client.DateContract; }
            set { client.DateContract = value; }
        }

        public ClientTypeDTO ClientType
        {
            get { return client.Type; }
            set
            {
                OnPropertyChanged(nameof(ClientType));
                client.Type = value;
            }
        }

        public string Contacts
        {
            get { return client.Contacts; }
            set { client.Contacts = value; }
        }

        public int PartnershipDuration => client.PartnershipDuration;

        public ObservableCollection<ClientTypeDTO> ClientsTypes => clientsModel.ClientsTypesList;

        public bool DateContractEnable { get; set; }
        #endregion

        public ClientEditViewModel()
        {
            CommandSave = new RelayCommand(SaveHandler);
        }

        async void SaveHandler()
        {
            IBusyToken token = null;

            try
            {
                token = BeginBusy();

                if (editType == EditType.Add)
                {
                    await clientsModel.AddClientAsync(client);
                }
                else
                {
                    await clientsModel.ChangeClientAsync(client);
                }
            }
            catch(Exception ex)
            {

            }
            finally
            {
                token.Dispose();
            }
            
            await ((IViewModel)this).CloseAsync();
        }

        public void Initialize(ClientsModel clientsModel)
        {
            client = new ClientDTO();
            editType = EditType.Add;
            DateContractEnable = true;
            this.clientsModel = clientsModel;

            DateContract = DateTime.Now;
        }

        public void Initialize(ClientDTO client, ClientsModel clientsModel)
        {
            editType = EditType.Change;
            this.client = client;
            this.clientsModel = clientsModel;
        }
    }
}
