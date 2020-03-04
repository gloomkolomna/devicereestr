using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DeviceReestr.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceReestr
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly DependencyResolver _dependencyResolver;

        public App()
        {
            _dependencyResolver = new DependencyResolver();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var mainVm = _dependencyResolver.ServiceProvider.GetService<MainVm>();
                var mainWindow = _dependencyResolver.ServiceProvider.GetService<MainWindow>();

                mainVm.Init();
                mainWindow.DataContext = mainVm;
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Реестр", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
        }
    }
}
