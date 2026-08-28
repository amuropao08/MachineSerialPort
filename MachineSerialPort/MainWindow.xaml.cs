using MachineSerialPort.Connected_Services;
using MachineSerialPort.Databases;
using MachineSerialPort.Databases.Tables;
using MachineSerialPort.Helpers;
using MailKit.Net.Smtp;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using SapGtMtClAt;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MachineSerialPort
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = _viewModel;

            _viewModel.RequestFocus = () =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    AddMeasureDataText.Focus();
                    Keyboard.Focus(AddMeasureDataText);
                }), System.Windows.Threading.DispatcherPriority.Input);
            };

            //AddMeasureDataText.IsEnabled = false;
            //AddDataButton.IsEnabled = false;

            //if (ConfigHelper.GetValue<bool>("AppSettings:ShowTestSendDataButton", false))
            //{
            //    TestSendDataButton.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    TestSendDataButton.Visibility = Visibility.Hidden;
            //}

            leatherDataGrid.AutoGenerateColumns = false;
            leatherDataGrid.CanUserAddRows = false;
            leatherDataGrid.CanUserDeleteRows = false;
            leatherDataGrid.IsReadOnly = true;

            packingDataGrid.AutoGenerateColumns = false;
            packingDataGrid.CanUserAddRows = false;
            packingDataGrid.CanUserDeleteRows = false;
            packingDataGrid.IsReadOnly = true;
        }

        private void DecimalTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = (string)e.DataObject.GetData(DataFormats.Text);
                Regex regex = new Regex(@"^[0-9]*(\.[0-9]{0,3})?$");
                if (!regex.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void DecimalTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void DecimalTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is not TextBox textBox) return;

            // Get the text as it WILL look after the change
            // This correctly handles if the user has highlighted text to replace it
            string newText = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength)
                                 .Insert(textBox.SelectionStart, e.Text);

            Regex regex = new Regex(@"^[0-9]*(\.[0-9]{0,3})?$");

            // If it doesn't match, we set Handled = true to BLOCK the input
            e.Handled = !regex.IsMatch(newText);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            // 'this' refers to the current Window (MainWindow)
            this.Close();
        }

        private async void GeneralSettings_Click(object sender, RoutedEventArgs e)
        {
            GeneralSettings win = new GeneralSettings();
            win.Owner = this;
            win.OriClient = ConfigHelper.GetValue<string>("AppSettings:SAPClient", "PRD");
            win.OriIsPacking = ConfigHelper.GetValue<bool>("AppSettings:PackingEnable", true);
            win.ShowDialog();

            ConfigHelper.Reload();

            _viewModel.CurrentClient = ConfigHelper.GetValue<string>("AppSettings:SAPClient", "PRD");
            _viewModel.IsPacking = ConfigHelper.GetValue<bool>("AppSettings:PackingEnable", true);
            _viewModel.ShowTestSendDataButton = ConfigHelper.GetValue<bool>("AppSettings:ShowTestSendDataButton", false);

            var result = _viewModel._dataService.GetRfcPwd();
            if (result.GetValue().IsSuccess)
            {
                _viewModel.sapPass = result.GetValue().Message;
            }
            else
            {
                await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        "Cannot get password for SAP user log in",
                        PackIconKind.WarningCircle,
                        Brushes.DarkOrange
                    );
            }

            _viewModel._dataService.SetSapEndpoint(_viewModel.CurrentClient);

            if (win.OriClient != _viewModel.CurrentClient || win.OriIsPacking != _viewModel.IsPacking)
            {
                _viewModel.ResetData();
            }
        }

        public string GenerateOTP()
        {
            Random res = new Random();
            // Generates a number between 100000 and 999999
            return res.Next(100000, 999999).ToString();
        }

        private async void LoadSession_Click(object sender, RoutedEventArgs e)
        {
            var win = new LoadSessionWindow
            {
                Owner = this
            };

            if (win.ShowDialog() == true && win.SelectedSession != null)
            {
                // Tell the ViewModel to update the UI with the loaded data
                _viewModel.LoadManualJob(win.SelectedSession);
                await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        $"Loaded {win.SelectedSession.CustomSessionName} successfully.",
                        PackIconKind.SuccessCircle,
                        Brushes.ForestGreen
                    );
            }
        }

        private async void ManualSave_Click(object sender, RoutedEventArgs e)
        {
            // You can use a simple MaterialDesign Dialog with a TextBox 
            // or just a standard InputBox for now
            string jobName = Microsoft.VisualBasic.Interaction.InputBox("Enter Job Name (e.g. Batch_001):", "Manual Save", "");

            if (!string.IsNullOrWhiteSpace(jobName))
            {
                await _viewModel.SaveManualJob(jobName);
            }
        }

        private void Reprint_Click(object sender, RoutedEventArgs e)
        {
            Reprint win = new Reprint();
            win.DataContext = this.DataContext;
            win.Owner = this;

            // The execution of this method pauses here until the window is closed
            win.ShowDialog();

            // Once the user closes the window (either by X or a button)
            //if (this.DataContext is MainViewModel vm)
            //{
            //    vm.ClearReprintData();
            //}
        }

        private async void Request_Click(object sender, RoutedEventArgs e)
        {
            string email = ConfigHelper.GetValue<string>("AppSettings:RequestOTPEmail");

            if (!string.IsNullOrWhiteSpace(email))
            {
                string generatedOtp = GenerateOTP(); // Your method from before

                // 1. Send the email (Async so the UI doesn't lag)
                await Task.Run(() => SendOTPEmail(email, generatedOtp));

                // 2. Open the OTP Window
                OTPWindow otpPopup = new OTPWindow(generatedOtp, email);
                otpPopup.Owner = this; // Centers it relative to MainWindow

                if (otpPopup.ShowDialog() == true)
                {
                    // This block runs ONLY if DialogResult was set to true (correct OTP)
                    //AddMeasureDataText.IsEnabled = true;
                    //AddDataButton.IsEnabled = true;
                    _viewModel.IsReadyToAdd = true;
                    await ConfigHelper.ShowSmartDialog(
                            "InfoTemplate",
                            "Add Measure Data Granted.",
                            PackIconKind.SuccessCircle,
                            Brushes.ForestGreen
                        );
                }
            }
            else
            {
                await ConfigHelper.ShowSmartDialog(
                            "InfoTemplate",
                            "You have not set email for received OTP",
                            PackIconKind.WarningCircle,
                            Brushes.DarkOrange
                        );
            }
        }

        public void SendOTPEmail(string userEmail, string otp)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Leather Measurement App", "notification@interhides.com"));
            message.To.Add(new MailboxAddress("Add Measure Approver", userEmail));
            message.Subject = "Your Verification Code for Approve Add Measure";

            message.Body = new TextPart("plain")
            {
                Text = $"Your OTP code is: {otp}. It will expire in 5 minutes."
            };

            using (var client = new SmtpClient())
            {
                // For Gmail: Use smtp.gmail.com, Port 587, SecureSocketOptions.StartTls
                client.Connect("smtp.office365.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                // Note: For Gmail/Outlook, use an "App Password," not your regular password
                client.Authenticate("notification@interhides.com", "y&UCM4Av^jxW");

                client.Send(message);
                client.Disconnect(true);
            }
        }

        private bool _isClosingConfirmed = false;

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // If we already confirmed, just let the window close
            if (_isClosingConfirmed) return;

            if (_viewModel.Leather.Count > 0 && !_viewModel.IsManualSaved)
            {
                // 1. STOP the window from closing right now
                e.Cancel = true;

                // 2. Ask the user (this doesn't block the UI thread, but pauses this method)
                var result = await ConfigHelper.ShowSmartDialog(
                        "ConfirmTemplate",
                        "You have unsaved measurements. Are you sure you want to exit?",
                        PackIconKind.AlertCircle,
                        Brushes.Goldenrod
                    );

                // 3. If they clicked CONFIRM, set our flag and close the window again
                if (result)
                {
                    _isClosingConfirmed = true;
                    this.Close(); // This triggers Window_Closing again, but hits the 'return' at the top
                }
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);

            await _viewModel.CheckForRecovery();
        }

        //private async Task Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    // 1. Get a reference to your ViewModel
        //    var viewModel = (MainViewModel)this.DataContext;

        //    // 2. Define what "Unsaved" looks like
        //    // For example: If we are Measuring, or Finished but not yet Saved
        //    bool isDataUnsaved = viewModel.CurrentState == DeviceState.StartMeasuring ||
        //                         viewModel.CurrentState == DeviceState.StopMeasuring ||
        //                         viewModel.Leather.Count > 0; // Or check if the Leather list isn't empty

        //    if (isDataUnsaved)
        //    {
        //        var result = await ConfigHelper.ShowSmartDialog(
        //                "ConfirmTemplate",
        //                "You have unsaved measurements. Are you sure you want to exit without saving?",
        //                PackIconKind.QuestionMarkCircle,
        //                Brushes.Goldenrod,
        //                "CONFIRM",
        //                "CANCEL"
        //            );

        //        if (!result)
        //        {
        //            // 3. This stops the application from closing
        //            e.Cancel = true;
        //        }
        //    }
        //}
    }
}