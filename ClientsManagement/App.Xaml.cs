using System;
using System.Windows;
using ClientsManagement.ViewModels;
using MugenMvvmToolkit;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.ViewModels;
using MugenMvvmToolkit.WPF.Infrastructure;

namespace ClientsManagement
{
    public partial class App : Application
    {
        public App()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Bootstrapper<MugenApplication>(this, new MugenContainer());
        }

        [STAThread]
        public static void Main()
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }

    class MugenApplication : MvvmApplication
    {
        public override Type GetStartViewModelType()
        {
            return typeof(MainViewModel);
        }
    }
}