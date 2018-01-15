using ClientsManagement.DTO;
using ClientsManagement.Models;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.ViewModels;
using System.Collections.ObjectModel;

namespace ClientsManagement.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        readonly ClientsModel clientsModel;

        public RelayCommand CommandAppLoad { get; }
        public RelayCommand<string> CommandMenuAction { get; }
        public ObservableCollection<ClientDTO> Clients => clientsModel.ClientsList;
        public ClientDTO SelectedClient { get; set; }

        public MainViewModel()
        {
            CommandAppLoad = new RelayCommand(AppLoadHandler);
            CommandMenuAction = new RelayCommand<string>(MenuActionHandler);
            clientsModel = new ClientsModel(new ClientsUnitOfWork());
        }

        async void AppLoadHandler()
        {
            await clientsModel.LoadAllAsync();
        }

        async void MenuActionHandler(string type)
        {
            if (type == "Add" || type == "Change")
            {
                using (var viewModel = GetViewModel<ClientEditViewModel>())
                {
                    if (type == "Add")
                        viewModel.Initialize(clientsModel);
                    else
                        viewModel.Initialize(SelectedClient, clientsModel);

                    viewModel.ShowAsync();
                }
            }
            else if (type == "Remove")
            {
                await clientsModel.RemoveClientAsync(SelectedClient);
            }
        }
    }
}