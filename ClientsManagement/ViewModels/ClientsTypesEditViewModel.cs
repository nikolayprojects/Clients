using System;
using System.Threading.Tasks;
using System.Windows;
using ClientsManagement.DTO;
using ClientsManagement.Models;
using ClientsManagement.Util;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.ViewModels;

namespace ClientsManagement.ViewModels
{
    public class ClientsTypesEditViewModel : ViewModelBase
    {
        ClientsModel clientsModel;
        CustomBindingList<ClientTypeDTO> bindingList;

        public CustomBindingList<ClientTypeDTO> ClientsTypes => bindingList;

        public void Initialize(ClientsModel clientsModel)
        {
            bindingList = new CustomBindingList<ClientTypeDTO>(clientsModel.ClientsTypesList);
            bindingList.ChangeItems += BindingList_ChangeItems;

            this.clientsModel = clientsModel;
        }

        private async Task BindingList_ChangeItems(CustomBindingListEventArgs<ClientTypeDTO> e)
        {
            IBusyToken token = null;

            try
            {
                token = BeginBusy();

                switch (e.ItemsChangedType)
                {
                    case ItemsChangedType.Added:
                        await clientsModel.AddClientTypeAsync(e.Item);
                        break;
                    case ItemsChangedType.Changed:
                        await clientsModel.ChangedClientTypeAsync(e.Item);
                        break;
                    case ItemsChangedType.Deleted:
                        await clientsModel.RemoveClientTypeAsync(e.Item);
                        break;
                }
            }
            catch (Exception ex)
            {
                e.Cancel = true;

                MessageBox.Show($"Не удалось обновить список типов клиентов.\r\n\r\n{ex.Message}", "Ошибка!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                token.Dispose();
            }
        }
    }
}