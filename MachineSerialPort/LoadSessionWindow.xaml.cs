using MachineSerialPort.Databases.Tables;
using MachineSerialPort.Helpers;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
using Path = System.IO.Path;

namespace MachineSerialPort
{
    /// <summary>
    /// Interaction logic for LoadSessionWindow.xaml
    /// </summary>
    public partial class LoadSessionWindow : Window
    {
        public RecoverySession? SelectedSession { get; private set; }
        public ObservableCollection<RecoverySession> Sessions { get; set; } = new ObservableCollection<RecoverySession>();

        public LoadSessionWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadFilesFromDisk();
        }

        private async void DeleteSession_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var session = button?.Tag as RecoverySession;

            if (session == null) return;

            string sessionName = session.CustomSessionName;

            var result = await ConfigHelper.ShowSmartDialog(
                "ConfirmTemplate",
                $"Are you sure you want to permanently delete '{sessionName}'?",
                PackIconKind.AlertCircle,
                Brushes.Crimson
            );

            if (result)
            {
                try
                {
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedSessions", $"{sessionName}.json");

                    // Standard Delete
                    if (File.Exists(path)) File.Delete(path);

                    Sessions.Remove(session);

                    await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        $"{sessionName} delete succeed.",
                        PackIconKind.SuccessCircle,
                        Brushes.ForestGreen);
                }
                catch (IOException ioEx)
                {
                    System.Diagnostics.Debug.WriteLine($"File deletion lock error for {sessionName}: {ioEx.Message}");

                    await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        "File is currently locked by the system. Try again in a moment.",
                        PackIconKind.WarningCircle,
                        Brushes.DarkOrange);
                }
                finally
                {
                    // Ensure the operator doesn't have to click the window again to use it
                    Keyboard.Focus(SessionListBox);
                }
            }
            else
            {
                Keyboard.Focus(SessionListBox);
            }
        }

        private void LoadFilesFromDisk()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedSessions");
            if (!Directory.Exists(path)) return;

            // Get all json files
            var files = Directory.GetFiles(path, "*.json");

            foreach (string filePath in files)
            {
                try
                {
                    // Open for reading, but allow other processes to read/write (FileShare.ReadWrite)
                    using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        var session = JsonSerializer.Deserialize<RecoverySession>(stream);

                        if (session != null)
                        {
                            // If the user didn't provide a CustomSessionName, use the FileName
                            if (string.IsNullOrWhiteSpace(session.CustomSessionName))
                            {
                                session.CustomSessionName = Path.GetFileNameWithoutExtension(filePath);
                            }

                            Sessions.Add(session);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the error but don't crash the window
                    System.Diagnostics.Debug.WriteLine($"Error loading {filePath}: {ex.Message}");
                }
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedSession = (RecoverySession)SessionListBox.SelectedItem;
            if (SelectedSession != null)
            {
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
