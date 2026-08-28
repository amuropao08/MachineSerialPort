using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MachineSerialPort.Helpers
{
    public static class ConfigHelper
    {
        public static IConfiguration Config { get; private set; }

        static ConfigHelper()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Config = builder.Build();
            Reload();
        }

        public static List<string> GetFriendlyPortNames()
        {
            List<string> portList = new List<string>();

            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%(COM%)'"))
                {
                    foreach (var device in searcher.Get())
                    {
                        var caption = device["Caption"]?.ToString();
                        if (!string.IsNullOrEmpty(caption))
                        {
                            portList.Add(caption);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Fallback to standard names if WMI fails
                return System.IO.Ports.SerialPort.GetPortNames().ToList();
            }

            return portList;
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }

        // This uses Generics to return the type you ask for
        public static T GetValue<T>(string key, T defaultValue = default!)
        {
            return Config.GetValue<T>(key, defaultValue)!;
        }

        public static void Reload()
        {
            // Re-run the builder to refresh the internal memory from the disk
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Config = builder.Build();
        }

        public static async Task<string?> ShowInputDialog(string templateName, string defaultMessage)
        {
            var template = Application.Current.TryFindResource(templateName) as DataTemplate;

            // We use your DialogData class, but put the default name in the 'Message' property
            var context = new DialogData
            {
                Message = defaultMessage
            };

            var content = new ContentControl { ContentTemplate = template, Content = context };

            try
            {
                var result = await DialogHost.Show(content, "RootDialog");
                return result as string;
            }
            catch (InvalidOperationException)
            {
                return Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter Job Name:",
                    "Manual Save (Fallback Mode)",
                    defaultMessage
                );
            }
        }

        public static async Task<bool> ShowSmartDialog(string templateName, string message, PackIconKind icon, System.Windows.Media.Brush color, string yesMessage = "OK", string noMessage = "CANCEL")
        {
            var template = Application.Current.TryFindResource(templateName) as DataTemplate;

            var context = new DialogData
            {
                Message = message,
                Icon = icon,
                IconColor = color,
                YesMessage = yesMessage,
                NoMessage = noMessage
            };

            var content = new ContentControl { ContentTemplate = template, Content = context };

            try
            {
                var result = await DialogHost.Show(content, "RootDialog");
                return result is bool isConfirmed && isConfirmed;
            }
            catch (InvalidOperationException)
            {
                // Fallback if DialogHost isn't ready: use standard MessageBox
                return System.Windows.MessageBox.Show(message, "Confirm", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            }
        }
    }
}
