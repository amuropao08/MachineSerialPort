using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Input;
using MachineSerialPort.Helpers;

namespace MachineSerialPort.ViewModels
{
    public class GeneralSettingsViewModel : INotifyPropertyChanged
    {
        private string _printerIp = "192.168.1.200";
        private string _ipErrorMessage = string.Empty;

        // --- Your existing properties ---
        public string SelectedClient { get; set; }
        public string OTPRequestEmail { get; set; }
        public bool IsPackingEnable { get; set; }
        public bool IsShowTestSendDataButton { get; set; }

        // --- Printer IP Properties ---
        public string PrinterIp
        {
            get => _printerIp;
            set
            {
                if (_printerIp != value)
                {
                    _printerIp = value;
                    OnPropertyChanged();
                    ValidatePrinterIp();
                }
            }
        }

        public string IpErrorMessage
        {
            get => _ipErrorMessage;
            set
            {
                if (_ipErrorMessage != value)
                {
                    _ipErrorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }

        public GeneralSettingsViewModel()
        {
            LoadSettings();
            SaveCommand = new RelayCommand(ExecuteSave, CanSave);
        }

        private bool ValidatePrinterIp()
        {
            if (string.IsNullOrWhiteSpace(PrinterIp))
            {
                IpErrorMessage = "IP Address cannot be empty.";
                return false;
            }

            if (!IpValidationHelper.IsValidIp(PrinterIp))
            {
                IpErrorMessage = "Invalid IP format (e.g., 192.168.1.200).";
                return false;
            }

            IpErrorMessage = string.Empty;
            return true;
        }

        private bool CanSave()
        {
            return string.IsNullOrEmpty(IpErrorMessage) && !string.IsNullOrWhiteSpace(PrinterIp);
        }

        private void LoadSettings()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    var jsonNode = JsonNode.Parse(json);
                    PrinterIp = jsonNode?["PrinterSettings"]?["PrinterIp"]?.ToString() ?? "192.168.1.200";
                }
            }
            catch
            {
                PrinterIp = "192.168.1.200";
            }
        }

        private void ExecuteSave()
        {
            if (!ValidatePrinterIp()) return;

            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

                JsonNode rootNode = File.Exists(configPath)
                    ? JsonNode.Parse(File.ReadAllText(configPath)) ?? new JsonObject()
                    : new JsonObject();

                if (rootNode["PrinterSettings"] == null)
                {
                    rootNode["PrinterSettings"] = new JsonObject();
                }

                rootNode["PrinterSettings"]["PrinterIp"] = PrinterIp.Trim();

                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(configPath, rootNode.ToJsonString(options));

                MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}