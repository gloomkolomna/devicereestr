using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DeviceReestr.Services;

namespace DeviceReestr.ViewModel
{
    public interface IAddDeviceTabVm : ITabVm
    {
        string SerialNo { get; set; }
        string Type { get; set; }
        string Description { get; set; }
        ICommand AddCommand { get; }
    }
    public class AddDeviceTabVm : ObservableObject, IAddDeviceTabVm
    {
        private readonly ILongOperationService _longOperationService;
        private readonly IUserService _userService;
        private readonly IDeviceService _deviceService;
        private string _serialNo;
        private string _type;
        private string _description;
        private ICommand _addCommand;

        public AddDeviceTabVm(ILongOperationService longOperationService, IUserService userService, IDeviceService deviceService)
        {
            _longOperationService = longOperationService;
            _userService = userService;
            _deviceService = deviceService;
        }

        public string SerialNo
        {
            get => _serialNo;
            set
            {
                _serialNo = value;
                OnPropertyChanged();
            }
        }

        public string Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new RelayCommand(
                           param => Add())
                {
                    MouseGesture = MouseAction.LeftClick
                });
            }
        }

        private async Task Add()
        {
            await _longOperationService.ExecuteAsync(() =>
            {
                _deviceService.CreateDevice(_userService.CurrentUser, SerialNo, Type, Description);
                Thread.Sleep(1000);
            }, "Добавление нового устройства");
        }
    }
}