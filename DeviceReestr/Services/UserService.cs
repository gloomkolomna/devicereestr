using DeviceReestr.Core;
using DeviceReestr.Core.Entities;

namespace DeviceReestr.Services
{
    public interface IUserService
    {
        User CreateUser(string login, string password);
        User GetUser(string login, string password);
        User CurrentUser { get; set; }
    }
    public class UserService : IUserService
    {
        private readonly IDeviceReestrService _deviceReestrService;
        private readonly IDialogService _dialogService;

        public UserService(IDeviceReestrService deviceReestrService, IDialogService dialogService)
        {
            _deviceReestrService = deviceReestrService;
            _dialogService = dialogService;
        }

        public User CreateUser(string login, string password)
        {
            var pocoUser = new User {Login = login, Password = password};
            var user = _deviceReestrService.AddUser(pocoUser);
            if (user == null)
            {
                _dialogService.Show(DialogIcon.Error, "Ошибка создания пользователя", $"Не удалось создать пользователя {pocoUser}");
                return null;
            }

            return user;
        }

        public User GetUser(string login, string password)
        {
            var user = _deviceReestrService.GetUser(login, password);
            if (user == null)
            {
                _dialogService.Show(DialogIcon.Error, "Ошибка получения пользователя", $"Не удалось получить пользователя {login}");
                return null;
            }

            return user;
        }

        public User CurrentUser { get; set; }
    }
}