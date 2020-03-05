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
        ITabVm SelectedTab { get; set; }
    }

    public class WorkAreaVm : ObservableObject, IWorkAreaVm
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserService _userService;
        private Tab _currentTab;
        private ICommand _closeUserSessionCommand;
        private ITabVm _selectedTab;

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

        public ICommand CloseUserSessionCommand
        {
            get
            {
                return _closeUserSessionCommand ?? (_closeUserSessionCommand = new RelayCommand(param => CloseUserSession())
                {
                    MouseGesture = MouseAction.LeftClick
                });
            }
        }

        public ITabVm SelectedTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                OnPropertyChanged();
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
                    if (_serviceProvider.GetService(typeof(IAddDeviceTabVm)) is AddDeviceTabVm addDeviceTabVm)
                    {
                        SelectedTab = addDeviceTabVm;
                    }
                    break;
                case Tab.Devices:
                    if (_serviceProvider.GetService(typeof(IDevicesTabVm)) is DevicesTabVm devicesTabVm)
                    {
                        devicesTabVm.LoadAsync().ConfigureAwait(false);
                        SelectedTab = devicesTabVm;
                    }
                    break;
                case Tab.Statistics:
                    if (_serviceProvider.GetService(typeof(IStatisticsTabVm)) is StatisticsTabVm statisticsTabVm)
                    {
                        statisticsTabVm.LoadAsync().ConfigureAwait(false);
                        SelectedTab = statisticsTabVm;
                    }
                    break;
                case Tab.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tab), tab, null);
            }
        }

    }
}