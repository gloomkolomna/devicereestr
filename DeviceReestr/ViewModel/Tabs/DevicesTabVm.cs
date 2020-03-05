using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DeviceReestr.Core.Entities;
using DeviceReestr.Services;

namespace DeviceReestr.ViewModel
{
    public interface IDevicesTabVm : ITabVm
    {
        ObservableCollection<Device> UserDevices { get; set; }
        ICommand ReloadCommand { get; }
        Task LoadAsync();
    }
    public class DevicesTabVm : ObservableObject, IDevicesTabVm
    {
        private readonly ILongOperationService _longOperationService;
        private readonly IDeviceService _deviceService;
        private readonly IUserService _userService;
        private readonly MainVm _mainVm;
        private ObservableCollection<Device> _userDevices;
        private ICommand _reloadCommand;

        public DevicesTabVm(ILongOperationService longOperationService, IDeviceService deviceService, IUserService userService, MainVm mainVm)
        {
            _longOperationService = longOperationService;
            _deviceService = deviceService;
            _userService = userService;
            _mainVm = mainVm;
        }

        public ObservableCollection<Device> UserDevices
        {
            get => _userDevices;
            set
            {
                _userDevices = value;
                OnPropertyChanged();
            }
        }

        public ICommand ReloadCommand
        {
            get
            {
                return _reloadCommand ?? (_reloadCommand = new RelayCommand(
                           param => LoadAsync())
                {
                    MouseGesture = MouseAction.LeftClick
                });
            }
        }

        public async Task LoadAsync()
        {
            await _longOperationService.ExecuteAsync(() =>
            {
                var devices = _deviceService.GetUserDevices(_userService.CurrentUser);
                _mainVm.GetContext().Post(post =>
                {
                    var orderDevices = devices.OrderByDescending(x => x.CreatedAt);
                    UserDevices = new ObservableCollection<Device>(orderDevices);
                }, null);

                Thread.Sleep(1000);
            }, "Загрузка устройств пользователя");
        }
    }
}