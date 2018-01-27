using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ClientsManagement.DTO;
using ClientsManagement.Models;
using MugenMvvmToolkit.Infrastructure.Validation;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.Interfaces.ViewModels;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.ViewModels;

namespace ClientsManagement.ViewModels
{
    public class ClientEditViewModel : EditableViewModel<ClientDTO>
    {
        enum EditType { Add, Update };
        EditType editType;
        ClientsModel clientsModel;

        #region ~Свойства~
        public RelayCommand CommandSave { get; }

        public string INN
        {
            get { return Entity.INN; }
            set { Entity.INN = value; }
        }

        public string Name
        {
            get { return Entity.Name; }
            set { Entity.Name = value; }
        }

        public DateTime DateContract
        {
            get { return Entity.DateContract; }
            set { Entity.DateContract = value; }
        }

        public ClientTypeDTO ClientType
        {
            get { return Entity.Type; }
            set { Entity.Type = value; }
        }

        public string Contacts
        {
            get { return Entity.Contacts; }
            set { Entity.Contacts = value; }
        }

        public int PartnershipDuration => Entity.PartnershipDuration;

        public IList<ClientTypeDTO> ClientsTypes => clientsModel.ClientsTypesList;

        public bool DateContractEnable { get; set; }
        #endregion

        public ClientEditViewModel()
        {
            CommandSave = new RelayCommand(SaveHandler, () => IsValid, this);
        }

        async void SaveHandler()
        {
            IBusyToken token = null;

            try
            {
                token = BeginBusy();

                if (editType == EditType.Add)
                {
                    await clientsModel.AddClientAsync(Entity);
                }
                else
                {
                    if (!await clientsModel.UpdateClientAsync(Entity))
                    {
                        MessageBox.Show("Изменения отсутствуют!", "Ошибка!", MessageBoxButton.OK, 
                            MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении клиента!\r\n\r\n{ex.Message}", "Ошибка!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                token.Dispose();
            }

            await ((IViewModel)this).CloseAsync();
        }

        public void Initialize(ClientsModel clientsModel)
        {
            editType = EditType.Add;
            DateContractEnable = true;
            this.clientsModel = clientsModel;

            InitializeEntity(new ClientDTO(), true);

            DateContract = DateTime.Now;

            AddValidator<ClientEditViewModelValidator>(Entity);
        }

        public void Initialize(ClientDTO clientDTO, ClientsModel clientsModel)
        {
            editType = EditType.Update;
            this.clientsModel = clientsModel;

            InitializeEntity(clientDTO, true);

            AddValidator<ClientEditViewModelValidator>(Entity);
        }
    }

    class ClientEditViewModelValidator : ValidatorBase<ClientDTO>
    {
        const byte NAME_LENGTH_MAX = 150;
        const byte NAME_LENGTH_MIN = 1;
        const byte INN_LENGTH = 12;

        readonly Regex regex;

        public ClientEditViewModelValidator()
        {
            regex = new Regex(@"^\d+$");
        }

        protected override Task<IDictionary<string, IEnumerable>> ValidateInternalAsync(string propertyName, CancellationToken token)
        {
            IDictionary<string, IEnumerable> dictionary = new Dictionary<string, IEnumerable>();

            if (EmptyStringCheck())
            {
                dictionary.Add(propertyName, "Значение не указано.");
                return Task.FromResult(dictionary);
            }

            if (propertyName == nameof(Instance.INN))
            {
                if (!regex.IsMatch(Instance.INN))
                {
                    dictionary.Add(propertyName, "При вводе значения разрешены только цифры.");
                }
                else if (Instance.INN.Length != INN_LENGTH)
                {
                    dictionary.Add(propertyName, "Значение имеет недопустимую длину.");
                }

                return Task.FromResult(dictionary);
            }

            if (propertyName == nameof(Instance.Name))
            {
                if (!(Instance.Name.Length >= NAME_LENGTH_MIN && Instance.Name.Length <= NAME_LENGTH_MAX))
                {
                    dictionary.Add(propertyName, "Значение имеет недопустимую длину.");
                }
            }

            bool EmptyStringCheck()
            {
                switch (propertyName)
                {
                    case nameof(Instance.INN):
                        return string.IsNullOrWhiteSpace(Instance.INN);

                    case nameof(Instance.Name):
                        return string.IsNullOrWhiteSpace(Instance.Name);

                    case nameof(Instance.Contacts):
                        return string.IsNullOrWhiteSpace(Instance.Contacts);

                    default:
                        return false;
                }
            }

            return Task.FromResult(dictionary);
        }

        protected override Task<IDictionary<string, IEnumerable>> ValidateInternalAsync(CancellationToken token)
        {
            return EmptyResult;
        }
    }
}