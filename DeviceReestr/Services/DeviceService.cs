using System.Collections.Generic;
using System.Linq;
using DeviceReestr.Core;
using DeviceReestr.Core.Entities;

namespace DeviceReestr.Services
{
    public interface IDeviceService
    {
        Device CreateDevice(User user, string serialNo, string type, string description);
        List<Device> GetUserDevices(User user);
        List<Device> GetAllDevices();
    }
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceReestrService _deviceReestrService;
        private readonly IDialogService _dialogService;

        public DeviceService(IDeviceReestrService deviceReestrService, IDialogService dialogService)
        {
            _deviceReestrService = deviceReestrService;
            _dialogService = dialogService;
        }

        public Device CreateDevice(User user, string serialNo, string type, string description)
        {
            var pocoDevice = new Device
            {
                SerialNo = serialNo,
                Description = description,
                Type = type
            };

            var device = _deviceReestrService.AddDevice(user, pocoDevice);
            if (device == null)
            {
                _dialogService.Show(DialogIcon.Error, "Ошибка создания устройства", $"Не удалось создать устройство {pocoDevice}");
                return null;
            }

            return device;
        }

        public List<Device> GetUserDevices(User user)
        {
            return _deviceReestrService.GetUserDevices(user).ToList();
        }

        public List<Device> GetAllDevices()
        {
            return _deviceReestrService.GetAllDevices().ToList();
        }
    }
}