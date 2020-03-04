using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Dapper;
using DeviceReestr.Core;
using DeviceReestr.Core.Entities;

namespace DeviceReestr.Db
{
    public class DeviceReestrService : DbOperation, IDeviceReestrService
    {
        public DeviceReestrService(DeviceReestrRepository deviceReestrRepository) : base(deviceReestrRepository)
        {
            if (!File.Exists(deviceReestrRepository.DbFile))
            {
                CreateDatabase();
            }
        }

        public User GetUser(string login, string password)
        {
            var user = Execute<User>($@"select * from Users where login = '{login}' and password = '{password}'");
            return user;
        }

        public Device GetUserDevice(User user, string serialNo, string type, string description)
        {
            var userDevices = GetUserDevices(user);
            return userDevices.FirstOrDefault(item => item.SerialNo == serialNo && item.Type == type && item.Description == description);
        }

        public IEnumerable<Device> GetUserDevices(User user)
        {
            var devices = GetAllDevices().Where(item => item.OwnerId == user.Id);
            return devices;
        }

        public IEnumerable<Device> GetAllDevices()
        {
            var devices = ExecuteEnumerable<Device>(@"select * from Devices");
            return devices;
        }

        public User AddUser(User user)
        {
            if (!ExistUser(user))
            {
                Execute($"INSERT INTO Users (Id, Login, Password) VALUES ('{Guid.NewGuid()}', '{user.Login}', '{user.Password}')");
            }

            return GetUser(user.Login, user.Password);
        }

        public Device AddDevice(User user, Device device)
        {
            var currentDateTime = DateTime.Now.ToString("s");
            Execute($"INSERT INTO Devices (Id, SerialNo, Type, Description, OwnerId, CreatedAt) " +
                                   $"VALUES ('{Guid.NewGuid()}', '{device.SerialNo}', '{device.Type}', '{device.Description}', " +
                                   $"'{user.Id}', '{currentDateTime}')");

            var returnDevice = GetUserDevice(user, device.SerialNo, device.Type, device.Description);
            return returnDevice;
        }

        private bool ExistUser(User user)
        {
            var userCount = Execute<int>($"select count(1) as userCount from Users " +
                                               $"where login = '{user.Login}' and password = '{user.Password}'");

            return userCount > 0;
        }
    }

    public abstract class DbOperation
    {
        private readonly DeviceReestrRepository _deviceReestrRepository;

        protected DbOperation(DeviceReestrRepository deviceReestrRepository)
        {
            _deviceReestrRepository = deviceReestrRepository;
        }

        protected void Execute(string sql)
        {
            try
            {
                using (var connection = _deviceReestrRepository.DbConnection())
                {
                    connection.Open();
                    connection.Execute(sql);
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected IEnumerable<T> ExecuteEnumerable<T>(string sql)
        {
            try
            {
                using (var connection = _deviceReestrRepository.DbConnection())
                {
                    var retValues = connection.Query<T>(sql);
                    return retValues;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected T Execute<T>(string sql)
        {
            var enumerables = ExecuteEnumerable<T>(sql);
            return enumerables.FirstOrDefault();
        }

        protected void CreateDatabase()
        {
            SQLiteConnection.CreateFile(_deviceReestrRepository.DbFile);
            Execute(@"CREATE TABLE Users (
                            Id Guid NOT NULL UNIQUE,
                            Login NVarChar(255),
                            Password NVarChar(255),
                            PRIMARY KEY(Id)
                        );
                        CREATE TABLE Devices (
                            Id Guid NOT NULL UNIQUE,
                            SerialNo NVarChar(255),
                            Type NVarChar(255),
                            Description NVarChar,
                            OwnerId Guid NOT NULL,
                            CreatedAt DateTime2,
                            PRIMARY KEY(Id) 
                            FOREIGN KEY(OwnerId) REFERENCES User(Id)
                        );");
        }
    }
}