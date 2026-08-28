using MachineSerialPort.Helpers;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MachineSerialPort
{
    /// <summary>
    /// Interaction logic for SetClient.xaml
    /// </summary>
    public partial class GeneralSettings : Window, INotifyPropertyChanged
    {
        public string OriClient = string.Empty;
        public bool OriIsPacking;

        private bool _isPackingEnable;
        public bool IsPackingEnable
        {
            get { return _isPackingEnable; }
            set
            {
                if (_isPackingEnable != value)
                {
                    _isPackingEnable = value;
                    OnPropertyChanged(nameof(IsPackingEnable));
                }
            }
        }

        private bool _isShowTestSendDataButton;
        public bool IsShowTestSendDataButton
        {
            get { return _isShowTestSendDataButton; }
            set
            {
                if (_isShowTestSendDataButton != value)
                {
                    _isShowTestSendDataButton = value;
                    OnPropertyChanged(nameof(IsShowTestSendDataButton));
                }
            }
        }

        private string _oTPRequestEmail = string.Empty;
        public string OTPRequestEmail
        {
            get { return _oTPRequestEmail; }
            set
            {
                if (_oTPRequestEmail != value)
                {
                    _oTPRequestEmail = value;
                    OnPropertyChanged(nameof(OTPRequestEmail));
                }
            }
        }

        private string _selectedClient = string.Empty;

        public string SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                if (_selectedClient != value)
                {
                    _selectedClient = value;
                    OnPropertyChanged(nameof(SelectedClient));
                }
            }
        }

        public ObservableCollection<string> SelectedClients { get; set; } = new ObservableCollection<string>();

        public ICommand SaveCommand { get; private set; }

        public GeneralSettings()
        {
            InitializeComponent();
            DataContext = this;

            IsPackingEnable = ConfigHelper.GetValue<bool>("AppSettings:PackingEnable", true);
            IsShowTestSendDataButton = ConfigHelper.GetValue<bool>("AppSettings:ShowTestSendDataButton", false);
            OTPRequestEmail = ConfigHelper.GetValue<string>("AppSettings:RequestOTPEmail") ?? string.Empty;

            AvailableClients();

            SaveCommand = new RelayCommand(SaveData);
        }

        private void AvailableClients()
        {
            SelectedClients.Clear();
            SelectedClients.Add("QAS");
            SelectedClients.Add("PRD");

            string currentClient = ConfigHelper.GetValue<string>("AppSettings:SAPClient") ?? "QAS";
            SelectedClient = currentClient == "QAS" ? SelectedClients[0] : SelectedClients[1];
        }

        private async Task SaveData()
        {
            if (OriClient == SelectedClient && OriIsPacking == IsPackingEnable)
            {
                UpdateSetting("AppSettings", "PackingEnable", IsPackingEnable);
                UpdateSetting("AppSettings", "ShowTestSendDataButton", IsShowTestSendDataButton);
                UpdateSetting("AppSettings", "RequestOTPEmail", OTPRequestEmail);
                UpdateSetting("AppSettings", "SAPClient", SelectedClient);

                await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        "Settings saved successfully.",
                        PackIconKind.SuccessCircle,
                        Brushes.ForestGreen
                    );

                this.Close();
            }
            else if(OriClient != SelectedClient || OriIsPacking != IsPackingEnable)
            {
                var result = await ConfigHelper.ShowSmartDialog(
                        "ConfirmTemplate",
                        $"You have change Client or Packing setting.\nProgram need to reset current state to Startup.\nAll current measuring data will lost.\nAre you confirm to go on?",
                        PackIconKind.QuestionMarkCircle,
                        Brushes.Goldenrod,
                        "CONFIRM",
                        "CANCEL"
                    );

                if (result)
                {
                    UpdateSetting("AppSettings", "PackingEnable", IsPackingEnable);
                    UpdateSetting("AppSettings", "ShowTestSendDataButton", IsShowTestSendDataButton);
                    UpdateSetting("AppSettings", "RequestOTPEmail", OTPRequestEmail);
                    UpdateSetting("AppSettings", "SAPClient", SelectedClient);

                    this.Close();
                }
            }
        }

        public void UpdateSetting(string section, string key, object value)
        {
            string configFilePath = "appsettings.json";
            if (!File.Exists(configFilePath)) return;

            string json = File.ReadAllText(configFilePath);
            var jsonObj = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(json);

            if (jsonObj != null && jsonObj.ContainsKey(section))
            {
                jsonObj[section][key] = value;
                var options = new JsonSerializerOptions { WriteIndented = true };
                string output = JsonSerializer.Serialize(jsonObj, options);
                File.WriteAllText(configFilePath, output);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
