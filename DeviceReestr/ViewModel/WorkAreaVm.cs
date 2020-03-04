using System;
using System.Windows.Input;
using DeviceReestr.Services;

namespace DeviceReestr.ViewModel
{
    public enum Tab
    {
        AddDevice,
        Devices,
        Statistics,
        None
    }

    public interface IWorkAreaVm : IBaseVm
    {
        Tab CurrentTab { get; set; }
        ICommand CloseUserSessionCommand { get; }
    }

    public class WorkAreaVm : ObservableObject, IWorkAreaVm
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserService _userService;
        private Tab _currentTab;
        private ICommand _closeUserSessionCommand;

        public WorkAreaVm(IServiceProvider serviceProvider, IUserService userService)
        {
            _serviceProvider = serviceProvider;
            _userService = userService;
            CurrentTab = Tab.None;
        }

        public void Close()
        {
            _userService.CurrentUser = null;
        }

        public Tab CurrentTab
        {
            get => _currentTab;
            set
            {
                _currentTab = value;
                TabFactory(value);
                OnPropertyChanged();
            }
        }

        public ICommand CloseUserSessionCommand {
            get
            {
                return _closeUserSessionCommand ?? (_closeUserSessionCommand = new RelayCommand(param => CloseUserSession())
                {
                    MouseGesture = MouseAction.LeftClick
                });
            }
        }

        private void CloseUserSession()
        {
            if (_serviceProvider.GetService(typeof(MainVm)) is MainVm mainVm)
            {
                Close();
                mainVm.HasAutorizationUser = false;
                mainVm.SelectedView = null;
            }
        }

        private void TabFactory(Tab tab)
        {
            switch (tab)
            {
                case Tab.AddDevice:
                    break;
                case Tab.Devices:
                    break;
                case Tab.Statistics:
                    break;
                case Tab.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tab), tab, null);
            }
        }

    }
}