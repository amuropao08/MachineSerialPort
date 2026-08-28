using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MachineSerialPort
{
    /// <summary>
    /// Interaction logic for OTPWindow.xaml
    /// </summary>
    public partial class OTPWindow : Window
    {
        private string _correctOtp;
        private string _email;
        private DispatcherTimer _timer;
        private TimeSpan _timeRemaining;

        public OTPWindow(string initialOtp, string email)
        {
            InitializeComponent();
            _correctOtp = initialOtp;
            _email = email;
            StartTimer();
        }

        private void StartTimer()
        {
            _timeRemaining = TimeSpan.FromMinutes(5);
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            btnVerify.IsEnabled = true;
            btnResend.Visibility = Visibility.Collapsed;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timeRemaining = _timeRemaining.Subtract(TimeSpan.FromSeconds(1));
            lblTimer.Text = $"Time remaining: {_timeRemaining:mm\\:ss}";

            if (_timeRemaining <= TimeSpan.Zero)
            {
                _timer.Stop();
                lblTimer.Text = "Code Expired!";
                btnVerify.IsEnabled = false;
                btnResend.Visibility = Visibility.Visible; // Show resend button
            }
        }

        private void BtnVerify_Click(object sender, RoutedEventArgs e)
        {
            if (txtOtpInput.Text == _correctOtp)
            {
                this.DialogResult = true; // Signals success to the parent window
                this.Close();
            }
            else
            {
                MessageBox.Show("Incorrect code. Please try again.");
            }
        }

        private void BtnResend_Click(object sender, RoutedEventArgs e)
        {
            // Re-use your existing generation/send logic
            _correctOtp = new Random().Next(100000, 999999).ToString();

            // Call your email sending method here
            //SendOTPEmail(_email, _correctOtp);

            MessageBox.Show("A new code has been sent.");
            StartTimer(); // Reset the clock
        }
    }
}
