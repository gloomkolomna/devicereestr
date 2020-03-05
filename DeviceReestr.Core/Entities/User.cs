using System;

namespace DeviceReestr.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            return $"Пользователь с логином {Login}";
        }
    }
}