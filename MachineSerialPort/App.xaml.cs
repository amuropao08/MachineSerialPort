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
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            // In your application startup method
            var services = new ServiceCollection();

            // Register the first DbContext with its connection string
            services.AddDbContext<HuefulDBDbContext>(options =>
                options.UseSqlServer(ConfigHelper.Config.GetConnectionString("HuefulDB")));

            services.AddDbContext<MaterialInfoDbContext>(options =>
                options.UseSqlServer(ConfigHelper.Config.GetConnectionString("MaterialInfo")));

            services.AddDbContext<MeasurementLeatherContext>(options =>
                options
                    .UseSqlServer(ConfigHelper.Config.GetConnectionString("MeasurementLeather"))
                    .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information) // Log to Output window
                    .EnableSensitiveDataLogging() // This lets you see the Parameter values (like OrderNo));
            );

            services.AddDbContext<SapDbContext>(options =>
                options.UseSqlServer(ConfigHelper.Config.GetConnectionString("SAP")));

            services.AddTransient<DataService>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();

            // Build the service provider
            ServiceProvider = services.BuildServiceProvider();

            // Create and show your main window
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }
    }

}
