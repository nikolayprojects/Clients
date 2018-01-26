using System;
using System.Collections.ObjectModel;
using System.Windows;
using ClientsManagement.DTO;
using ClientsManagement.Models;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.ViewModels;

namespace ClientsManagement.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        readonly ClientsModel clientsModel;

        public RelayCommand CommandAppLoad { get; }
        public RelayCommand<string> CommandToolBarAction { get; }
        public RelayCommand CommandMenuAction { get; }
        public ObservableCollection<ClientDTO> Clients => clientsModel.ClientsList;
        public ClientDTO SelectedClient { get; set; }

        public MainViewModel(IClientsUnitOfWork unitOfWork)
        {
            CommandAppLoad = new RelayCommand(AppLoadHandler);
            CommandToolBarAction = new RelayCommand<string>(ToolBarActionHandler);
            CommandMenuAction = new RelayCommand(MenuActionHandler);
            clientsModel = new ClientsModel(unitOfWork);
        }

        async void AppLoadHandler()
        {
            IBusyToken token = null;

            try
            {
                token = BeginBusy();

                await clientsModel.LoadAllAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить данные.\r\n\r\n{ex.Message}", "Ошибка!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                token.Dispose();
            }
        }

        async void ToolBarActionHandler(string type)
        {
            if (type == "Add" || type == "Change")
            {
                using (var viewModel = GetViewModel<ClientEditViewModel>())
                {
                    if (type == "Add")
                    {
                        viewModel.Initialize(clientsModel);

                        viewModel.ShowAsync();
                    }
                    else
                    {
                        if (Check())
                        {
                            viewModel.Initialize(SelectedClient, clientsModel);

                            viewModel.ShowAsync();

                            SelectedClient = null;
                        }
                    }
                }
            }
            else if (type == "Remove" && Check())
            {
                try
                {
                    await clientsModel.RemoveClientAsync(SelectedClient);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении клиента!\r\n\r\n{ex.Message}", "Ошибка!", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }

                SelectedClient = null;
            }

            bool Check()
            {
                if (Clients.Count == 1)
                {
                    SelectedClient = Clients[0];
                    return true;
                }
                else if (SelectedClient == null)
                {
                    MessageBox.Show("Не выбран клиент!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                
                return true;
            }
        }

        void MenuActionHandler()
        {
            using (var viewModel = GetViewModel<ClientsTypesEditViewModel>())
            {
                viewModel.Initialize(clientsModel);
                viewModel.ShowAsync();
            }
        }
    }
}