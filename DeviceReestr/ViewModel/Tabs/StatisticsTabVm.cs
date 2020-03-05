using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DeviceReestr.Core.Entities;
using DeviceReestr.Services;

namespace DeviceReestr.ViewModel
{
    public interface IStatisticsTabVm : ITabVm
    {
        ObservableCollection<Device> Devices { get; set; }
        ICommand ReloadCommand { get; }
        Task LoadAsync();
    }

    public class StatisticsTabVm : ObservableObject, IStatisticsTabVm
    {
        private readonly ILongOperationService _longOperationService;
        private readonly IDeviceService _deviceService;
        private readonly MainVm _mainVm;
        private ObservableCollection<Device> _devices;
        private ICommand _reloadCommand;

        public StatisticsTabVm(ILongOperationService longOperationService, IDeviceService deviceService, MainVm mainVm)
        {
            _longOperationService = longOperationService;
            _deviceService = deviceService;
            _mainVm = mainVm;
        }

        public ObservableCollection<Device> Devices
        {
            get => _devices;
            set
            {
                _devices = value;
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
                var devices = _deviceService.GetAllDevices();
                _mainVm.GetContext().Post(post =>
                {
                    var lastFive = devices.OrderByDescending(x => x.CreatedAt).Take(5);
                    Devices = new ObservableCollection<Device>(lastFive);
                }, null);

                Thread.Sleep(1000);
            }, "Загрузка последних устройств");
        }
    }
}