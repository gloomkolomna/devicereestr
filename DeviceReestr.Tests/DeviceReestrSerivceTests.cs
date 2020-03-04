using System;
using DeviceReestr.Core;
using DeviceReestr.Core.Entities;
using DeviceReestr.Db;
using NSubstitute;
using NUnit.Framework;

namespace DeviceReestr.Tests
{
    public class Mock
    {
        public IDeviceReestrService DeviceReestrService { get; }
        public User User { get; }
        public Device Device { get; }

        public Mock()
        {
            var repository = new DeviceReestrRepository();
            var file = repository.DbFile;
            DeviceReestrService = Substitute.For<DeviceReestrService>(repository);

            User = CreateUser();
            Device = CreateDevice(User);
        }

        private User CreateUser()
        {
            var user = Substitute.For<User>();
            user.Login = "test";
            user.Password = "admin";
            return user;
        }

        private Device CreateDevice(User user)
        {
            var device = Substitute.For<Device>();
            device.SerialNo = "0112345678910";
            device.Type = "01type";
            device.Description = "10description";
            device.Owner = user;

            return device;
        }
    }

    [SetUpFixture]
    public class DeviceReestrFixture
    {
        public Mock Mock { get; private set; }

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Mock = new Mock();
        }
    }

    [TestFixture]
    public class DeviceReestrSerivceTests : DeviceReestrFixture
    {
        [Test, Order(0)]
        public void AddUserTest()
        {
            var newUser = Mock.DeviceReestrService.AddUser(Mock.User);
            Assert.AreEqual(Mock.User.Login, newUser.Login);
        }

        [Test, Order(1)]
        public void AddDeviceTest()
        {
            var user = Mock.DeviceReestrService.GetUser(Mock.User.Login, Mock.User.Password);
            var newDevice = Mock.DeviceReestrService.AddDevice(user, Mock.Device);
            Assert.AreEqual(Mock.Device.SerialNo, newDevice.SerialNo);
        }
    }
}