using System;
using System.Windows;
using ClientsManagement.Models;
using ClientsManagement.Util;
using ClientsManagement.ViewModels;
using MugenMvvmToolkit;
using MugenMvvmToolkit.WPF.Infrastructure;
using Ninject;

namespace ClientsManagement
{
    public partial class App : Application
    {
        public App()
        {
            IKernel kernel = new StandardKernel();
            kernel.Bind<ICollectionWrapperFactory>().To<CollectionWrapperFactory>();
            kernel.Bind<IClientsUnitOfWork>().To<ClientsUnitOfWork>().InSingletonScope();
            kernel.Bind<MainViewModel>().ToSelf();

            new Bootstrapper<MugenApplication>(this, new NinjectContainer(kernel));
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