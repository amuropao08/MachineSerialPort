using MachineSerialPort.Databases;
using MachineSerialPort.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace MachineSerialPort
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            // เปลี่ยนจาก AddDbContext เป็น AddDbContextFactory ทุกตัว
            services.AddDbContextFactory<HuefulDBDbContext>(options =>
                options.UseSqlServer(ConfigHelper.Config.GetConnectionString("HuefulDB")));

            services.AddDbContextFactory<MaterialInfoDbContext>(options =>
                options.UseSqlServer(ConfigHelper.Config.GetConnectionString("MaterialInfo")));

            services.AddDbContextFactory<MeasurementLeatherContext>(options =>
                options
                    .UseSqlServer(ConfigHelper.Config.GetConnectionString("MeasurementLeather"))
                    .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
                    .EnableSensitiveDataLogging()
            );

            services.AddDbContextFactory<SapDbContext>(options =>
                options.UseSqlServer(ConfigHelper.Config.GetConnectionString("SAP")));

            services.AddSingleton<DataService>(); // ตอนนี้ปลอดภัยที่จะเป็น Singleton เพราะไม่ถือ DbContext ค้างแล้ว
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();

            ServiceProvider = services.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }
    }

}
