using System;
using System.Windows;
using DeviceReestr.ViewModel;

namespace DeviceReestr.Services
{
    public enum DialogIcon
    {
        None = 0,
        Error = 16,
        Warning = 48,
        Information = 64
    }

    public interface IDialogService
    {
        void Show(DialogIcon icon, string header, string content);
    }

    public class DialogService : IDialogService
    {
        private readonly IServiceProvider _serviceProvider;

        public DialogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Show(DialogIcon icon, string header, string content)
        {
            if (_serviceProvider.GetService(typeof(MainVm)) is MainVm mainVm &&
                _serviceProvider.GetService(typeof(MainWindow)) is MainWindow mainWindow)
            {
                var context = mainVm.GetContext();
                context.Post(post =>
                {
                    var taskDialogIcon = (MessageBoxImage)icon;
                    MessageBox.Show(mainWindow, content, header, MessageBoxButton.OK, taskDialogIcon);
                }, null);
            }
            else
            {
                Console.WriteLine($@"{icon}: Header: {header}; Content: {content}");
            }
        }
    }
}