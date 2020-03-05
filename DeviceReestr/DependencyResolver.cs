using System;
using DeviceReestr.Core;
using DeviceReestr.Db;
using DeviceReestr.Services;
using DeviceReestr.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceReestr
{
    public class DependencyResolver
    {
        public IServiceProvider ServiceProvider { get; }

        public DependencyResolver()
        {
            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainVm>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<DeviceReestrRepository>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IDeviceService, DeviceService>();

            services.AddSingleton<IWorkAreaVm, WorkAreaVm>();

            services.AddScoped<IDeviceReestrService, DeviceReestrService>();
            services.AddScoped<ILongOperationService, LongOperationService>();
            services.AddScoped<IDialogService, DialogService>();

            services.AddTransient<IAddDeviceTabVm, AddDeviceTabVm>();
            services.AddTransient<IDevicesTabVm, DevicesTabVm>();
            services.AddTransient<IStatisticsTabVm, StatisticsTabVm>();
        }
    }
}