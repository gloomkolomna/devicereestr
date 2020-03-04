using System.Collections.Generic;
using DeviceReestr.Core.Entities;

namespace DeviceReestr.Core
{
    public interface IDeviceReestrService
    {
        User GetUser(string login, string password);
        Device GetUserDevice(User user, string serialNo, string type, string description);
        IEnumerable<Device> GetUserDevices(User user);
        IEnumerable<Device> GetAllDevices();

        User AddUser(User user);
        Device AddDevice(User user, Device device);
    }
}