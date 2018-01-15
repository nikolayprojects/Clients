using ClientsManagement.ViewModels;
using MugenMvvmToolkit.Attributes;
using System.Windows;

namespace ClientsManagement.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    [ViewModel(typeof(MainViewModel))]
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }
    }
}