using Azure;
using MachineSerialPort.Connected_Services;
using MachineSerialPort.Databases;
using MachineSerialPort.Databases.Tables;
using MachineSerialPort.Helpers;
using MachineSerialPort.Models;
using MachineSerialPort.PDF;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using MimeKit;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Quality;
using SapChkMsOp;
using SapGtHuIf;
using SapGtMtClAt;
using SapMigo;
using SapOdGrdTt;
using SapRtAct;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // For ICommand
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Xml;
using static MachineSerialPort.Constants;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;
using BAPI_CORU_RETURN = SapMigo.BAPI_CORU_RETURN;
//using BAPI_PP_TIMETICKET = SapOdCf.BAPI_PP_TIMETICKET;
using BAPI_PP_TIMETICKET = SapMigo.BAPI_PP_TIMETICKET;
using BAPI2017_GM_ITEM_CREATE = SapMigo.BAPI2017_GM_ITEM_CREATE;
using ZSTPDITEM = SapChkMsOp.ZSTPDITEM;

namespace MachineSerialPort
{
    public class MainViewModel : INotifyPropertyChanged
    {
        Dictionary<string, string> arraySapEndpoint = new Dictionary<string, string>();

        //private bool _hasSavedSuccessfully = false;
        //bool _isPackingPrinted = false;
        //private bool _isStopped = false;

        public int CurrentGroup = 1; // Start at Group 1

        public readonly DataService _dataService;

        private string _serialBuffer = string.Empty;

        private BAPI2017_GM_CODE Gc     = new BAPI2017_GM_CODE();
        private BAPI2017_GM_HEAD_01 Gh = new BAPI2017_GM_HEAD_01();
        private ZSTPDITEM Oi            = new ZSTPDITEM();

        private decimal PackingTotalArea;
        private decimal TotalArea;

        private string _cnfCnt = string.Empty;
        public string CnfCnt { get => _cnfCnt; set { _cnfCnt = value; OnPropertyChanged(); } }

        private string _cnfNo = string.Empty;
        public string CnfNo { get => _cnfNo; set { _cnfNo = value; OnPropertyChanged(); } }

        private string _endDate = string.Empty;
        public string EndDate { get => _endDate; set { _endDate = value; OnPropertyChanged(); } }

        private string _endTime = string.Empty;
        public string EndTime { get => _endTime; set { _endTime = value; OnPropertyChanged(); } }

        private string _errMsg = string.Empty;
        public string ErrMsg { get => _errMsg; set { _errMsg = value; OnPropertyChanged(); } }

        private string _matDocNo = string.Empty;
        public string MatDocNo { get => _matDocNo; set { _matDocNo = value; OnPropertyChanged(); } }

        private string _startDate = string.Empty;
        public string StartDate { get => _startDate; set { _startDate = value; OnPropertyChanged(); } }

        private string _startTime = string.Empty;
        public string StartTime { get => _startTime; set { _startTime = value; OnPropertyChanged(); } }

        //private string returnResult;
        public string? sapPass;
        //private string sapUser = "IHL_RFC";

        //public DbSet<Rfc_pass> Rfc_pass { get; set; }

        public ReturnResult rtnRst = new();

        private ObservableCollection<BAPIPAREX> Ex                         = new ObservableCollection<BAPIPAREX>();
        private ObservableCollection<BAPI2017_GM_ITEM_CREATE> Gi           = new ObservableCollection<BAPI2017_GM_ITEM_CREATE>();
        private ObservableCollection<_CWM_BAPI2017_GM_ITEM_CREATE> Gic     = new ObservableCollection<_CWM_BAPI2017_GM_ITEM_CREATE>();
        private ObservableCollection<RSIS_S_RANGE> md = new ObservableCollection<RSIS_S_RANGE>();
        private ObservableCollection<BAPI_PP_TIMETICKET> Tt                = new ObservableCollection<BAPI_PP_TIMETICKET>();


        private string _addMeasureData = string.Empty;
        public string AddMeasureData
        {
            get => _addMeasureData;
            set => SetProperty(ref _addMeasureData, value);
        }

        private string _colorArticle = string.Empty;
        public string ColorArticle
        {
            get => _colorArticle;
            set => SetProperty(ref _colorArticle, value);
        }

        private string _connectionButtonIcon = "LanConnect";
        public string ConnectionButtonIcon
        {
            get => _connectionButtonIcon;
            set => SetProperty(ref _connectionButtonIcon, value);
        }

        private string _connectionButtonText = "Connect";
        public string ConnectionButtonText
        {
            get => _connectionButtonText;
            set => SetProperty(ref _connectionButtonText, value);
        }

        private string _currentClient = "PRD";
        public string CurrentClient
        {
            get => _currentClient;
            set => SetProperty(ref _currentClient, value);
        }

        public string DocYear { get; set; } = DateTime.Now.ToString("yyyy");
        public string ReprintDocYear { get; set; } = string.Empty;

        private string _matDesc = string.Empty;
        public string MatDesc
        {
            get => _matDesc;
            set => SetProperty(ref _matDesc, value);
        }

        private string _matDocFrom = string.Empty;
        public string MatDocFrom
        {
            get => _matDocFrom;
            set => SetProperty(ref _matDocFrom, value);
        }

        private string _matDocTo = string.Empty;
        public string MatDocTo
        {
            get => _matDocTo;
            set => SetProperty(ref _matDocTo, value);
        }

        private string _matNo = string.Empty;
        public string MatNo
        {
            get => _matNo;
            set => SetProperty(ref _matNo, value);
        }

        private string _operNo = string.Empty;
        public string OperNo
        {
            get => _operNo;
            set => SetProperty(ref _operNo, value);
        }

        private string _orderNo = string.Empty;
        public string OrderNo
        {
            get => _orderNo;
            set => SetProperty(ref _orderNo, value);
        }

        private string _reprintOrderNo = string.Empty;
        public string ReprintOrderNo
        {
            get => _reprintOrderNo;
            set => SetProperty(ref _reprintOrderNo, value);
        }

        private bool _isAllSelected;
        public bool IsAllSelected
        {
            get => _isAllSelected;
            set
            {
                if (_isAllSelected != value)
                {
                    _isAllSelected = value;
                    OnPropertyChanged(nameof(IsAllSelected));

                    // Set all items to match the header checkbox
                    if (Leather != null)
                    {
                        foreach (var item in Leather)
                        {
                            item.IsSelected = _isAllSelected;
                        }
                    }
                }
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    OnPropertyChanged(nameof(CanProcessData));
                    OnPropertyChanged(nameof(CanAddData));
                }
            }
        }

        private bool _isFocused;
        public bool IsFocused
        {
            get => _isFocused;
            set
            {
                _isFocused = value;
                OnPropertyChanged(nameof(IsFocused));
            }
        }

        private bool _isPacking;
        public bool IsPacking
        {
            get => _isPacking;
            set
            {
                if (_isPacking == value) return;

                if (_isPacking && !value)
                {
                    ConfirmUncheckAsync();
                    return;
                }

                _isPacking = value;

                // Notify all related properties
                OnPropertyChanged(nameof(IsPacking));
                OnPropertyChanged(nameof(TextPackingStatus));
                OnPropertyChanged(nameof(TextPackingColor));
            }
        }

        private OrderQuantity _orderQty = new();
        public OrderQuantity OrderQty
        {
            get => _orderQty;
            set
            {
                _orderQty = value;
                OnPropertyChanged(nameof(OrderQty));
            }
        }

        private async void ConfirmUncheckAsync()
        {
            var result = await ConfigHelper.ShowSmartDialog(
                        "ConfirmTemplate",
                        "Unchecking this will clear all current rows. Are you sure?",
                        PackIconKind.QuestionMarkCircle,
                        Brushes.Goldenrod,
                        "CONFIRM",
                        "CANCEL"
                    );

            if (result)
            {
                // User confirmed: Update the value and clear data
                _isPacking = false;
                PackingLeather?.Clear();
                TxtPackingTotalArea = "";

                OnPropertyChanged(nameof(IsPacking));
                OnPropertyChanged(nameof(TextPackingColor));
                OnPropertyChanged(nameof(TextPackingStatus));
            }
            else
            {
                // User cancelled: Force the UI to stay "Checked"
                OnPropertyChanged(nameof(IsPacking));
            }
        }
        //public bool IsPacking
        //{
        //    get => _isPacking;
        //    set
        //    {
        //        // 1. If the value is the same, do nothing
        //        if (_isPacking == value) return;

        //        // 2. If the user is UNCHECKING (changing from true to false)
        //        if (_isPacking == true && value == false && PackingLeather?.Count > 0 && !IsBusy)
        //        {
        //            var result = MessageBox.Show(
        //                "Unchecking this will clear all current rows. Are you sure?",
        //                "Confirm Clear",
        //                MessageBoxButton.YesNo,
        //                MessageBoxImage.Warning);

        //            if (result == MessageBoxResult.No)
        //            {
        //                // Revert the value and exit (don't clear data)
        //                OnPropertyChanged(nameof(IsPacking));
        //                return;
        //            }
        //        }

        //        // 3. Proceed with the change
        //        _isPacking = value;
        //        OnPropertyChanged(nameof(IsPacking));

        //        // 4. Clear data only if it ended up being false
        //        if (!_isPacking)
        //        {
        //            PackingLeather?.Clear();
        //            // Reset any total strings or calculations here
        //            TxtPackingTotalArea = "";
        //        }

        //        OnPropertyChanged(nameof(TextPackingColor));
        //        OnPropertyChanged(nameof(TextPackingStatus));
        //    }
        //}

        private bool _isReadyToAdd;
        public bool IsReadyToAdd
        {
            get => _isReadyToAdd;
            set
            {
                if (SetProperty(ref _isReadyToAdd, value))
                {
                    // Only update the Add helper
                    OnPropertyChanged(nameof(CanAddData));
                }
            }
        }

        //public bool IsReadyToEditSettings => !IsStarted;
        private bool _isStarted = false;
        public bool IsStarted
        {
            get => _isStarted;
            set
            {
                if (SetProperty(ref _isStarted, value))
                {
                    OnPropertyChanged(nameof(IsReadyToEditSettings));
                    OnPropertyChanged(nameof(CanProcessData));

                    // This ensures buttons update their Enabled/Disabled state immediately
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        //private ObservableCollection<LeatherArea> _leather = new();
        //public ObservableCollection<LeatherArea> Leather // Public property
        //{
        //    get => _leather;
        //    set => SetProperty(ref _leather, value);
        //}

        //private ObservableCollection<LeatherArea> _packingBatch = new();
        //public ObservableCollection<LeatherArea> PackingBatch // Public property
        //{
        //    get => _packingBatch;
        //    set => SetProperty(ref _packingBatch, value);
        //}

        //private ObservableCollection<LeatherArea> _packingLeather = new();
        //public ObservableCollection<LeatherArea> PackingLeather // Public property
        //{
        //    get => _packingLeather;
        //    set => SetProperty(ref _packingLeather, value);
        //}

        public ObservableCollection<LeatherArea> Leather { get; } = new();
        public ObservableCollection<LeatherArea> PackingBatch { get; } = new();
        public ObservableCollection<LeatherArea> PackingLeather { get; } = new();

        private string _receivedData = string.Empty;
        public string ReceivedData
        {
            get => _receivedData;
            set => SetProperty(ref _receivedData, value);
        }

        private LeatherArea? _selectedLeather = new();
        public LeatherArea? SelectedLeather
        {
            get => _selectedLeather;
            set => SetProperty(ref _selectedLeather, value);
        }

        private LeatherArea? _selectedPackingLeather = new();
        public LeatherArea? SelectedPackingLeather
        {
            get => _selectedPackingLeather;
            set => SetProperty(ref _selectedPackingLeather, value);
        }

        private bool _showTestSendDataButton;
        public bool ShowTestSendDataButton
        {
            get => _showTestSendDataButton;
            set => SetProperty(ref _showTestSendDataButton, value);
        }

        public string TextPackingColor => IsPacking ? "SeaGreen" : "Red";
        public string TextPackingStatus => IsPacking ? "PACKING" : "NOT PACKING";

        private string _txtPackingTotalArea = string.Empty;
        public string TxtPackingTotalArea
        {
            get => _txtPackingTotalArea;
            set => SetProperty(ref _txtPackingTotalArea, value);
        }

        private string _txtTotalArea = string.Empty;
        public string TxtTotalArea
        {
            get => _txtTotalArea;
            set => SetProperty(ref _txtTotalArea, value);
        }

        private string _unit = string.Empty;
        public string Unit
        {
            get => _unit;
            set => SetProperty(ref _unit, value);
        }

        private string _packaging = string.Empty;
        public string Packaging
        {
            get => _packaging;
            set => SetProperty(ref _packaging, value);
        }

        public ICommand AddItemCommand { get; private set; }
        public ICommand DeleteItemCommand { get; }
        public ICommand GetOrderInfoCommand { get; private set; }
        public ICommand NextPackingCommand { get; }
        public ICommand PrintA4Command { get; }
        public ICommand PrintA5Command { get; private set; }
        public ICommand PrintPackingCommand { get; private set; }
        public ICommand RefreshPortsCommand { get; }
        public ICommand ReprintA4Command { get; }
        public ICommand ReprintA5Command { get; }
        public ICommand ResetCommand { get; }
        public ICommand SaveCommand { get; private set; }
        public ICommand StartCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand TestSendDataCommand { get; }
        public ICommand ToggleConnectionCommand { get; }
        public ICommand TogglePauseCommand { get; }
        public ICommand SaveManualCommand { get; }
        public ICommand LoadManualCommand { get; }

        private DeviceState _currentState = DeviceState.Disconnected;
        public DeviceState CurrentState
        {
            get => _currentState;
            set
            {
                if (_currentState != value)
                {
                    _currentState = value;
                    OnPropertyChanged(nameof(CurrentState));

                    // Notify all dependent UI properties
                    OnPropertyChanged(nameof(IsToggleConnected));
                    OnPropertyChanged(nameof(IsReadyToSearchOrder));
                    OnPropertyChanged(nameof(IsReadyToEditSettings));
                    OnPropertyChanged(nameof(IsNGReasonEnabled));
                    OnPropertyChanged(nameof(IsPackingAllowed));
                    OnPropertyChanged(nameof(IsReadyToStart));
                    OnPropertyChanged(nameof(IsMeasuring));
                    OnPropertyChanged(nameof(IsConfirmStopMeasuring));
                    OnPropertyChanged(nameof(IsReadyToSave));
                    OnPropertyChanged(nameof(CanProcessData));
                    OnPropertyChanged(nameof(CanAddData));

                    if (_currentState != DeviceState.StartMeasuring)
                    {
                        IsReadyToAdd = false;
                    }

                    OnPropertyChanged(nameof(IsReadyToAdd));

                    // Refresh button Enable/Disable status
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }
        public bool IsToggleConnected => CurrentState != DeviceState.StartMeasuring && CurrentState != DeviceState.ConfirmStopMeasuring && CurrentState != DeviceState.StopMeasuring;
        public bool IsReadyToSearchOrder => CurrentState == DeviceState.Connected || CurrentState == DeviceState.Ready;
        public bool IsReadyToEditSettings => CurrentState == DeviceState.Ready || CurrentState == DeviceState.Saved;
        public bool IsNGReasonEnabled => SelectedGrade == "NG";
        public bool IsPackingAllowed => CurrentState != DeviceState.Disconnected
            && CurrentState != DeviceState.StartMeasuring
            && CurrentState != DeviceState.StopMeasuring;
            //&& SelectedGrade != "NC" && SelectedGrade != "NG";
        public bool IsReadyToStart => (CurrentState == DeviceState.Ready || CurrentState == DeviceState.Saved) && CurrentState != DeviceState.ConfirmCancel;
        public bool IsMeasuring => CurrentState == DeviceState.StartMeasuring;
        public bool IsConfirmStopMeasuring => CurrentState == DeviceState.ConfirmStopMeasuring;
        public bool IsReadyToSave => CurrentState == DeviceState.StopMeasuring;

        public MainViewModel(DataService dataService)
        {
            CurrentClient = ConfigHelper.GetValue<string>("AppSettings:SAPClient", "PRD");
            arraySapEndpoint = dataService.GetSapEndPoint(CurrentClient);

            AvailablePorts = new ObservableCollection<string>();
            RefreshPorts();
            
            ConfirmTypes = new ObservableCollection<ComboItem>();
            GetConfirmTypes();

            IsPacking = ConfigHelper.GetValue<bool>("AppSettings:PackingEnable", false);
            ShowTestSendDataButton = ConfigHelper.GetValue<bool>("AppSettings:ShowTestSendDataButton", false);
            OrderQty = new OrderQuantity();

            //Leather          = new ObservableCollection<LeatherArea>();
            LeatherGrades    = new ObservableCollection<string>();
            NGReasons        = new ObservableCollection<ComboItem>();
            //PackingBatch     = new ObservableCollection<LeatherArea>();
            //PackingLeather   = new ObservableCollection<LeatherArea>();
            //Packagings       = new ObservableCollection<ComboItem>();
            ParallelUnits    = new ObservableCollection<ComboItem>();
            Personnels       = new ObservableCollection<ComboItem>();
            StorageLocations = new ObservableCollection<ComboItem>();
            WageGroups       = new ObservableCollection<ComboItem>();
            Packagings       = new ObservableCollection<ComboItem>();

            RefreshPortsCommand     = new RelayCommand(RefreshPorts);
            ToggleConnectionCommand = new RelayCommand(ToggleConnection);
            GetOrderInfoCommand     = new AsyncRelayCommand(GetOrderInfo, CheckInputOrderNo);
            ResetCommand            = new RelayCommandObj(ResetCommandExecute);
            StartCommand            = new RelayCommand(StartMeasure);
            CancelCommand           = new RelayCommand(BackToReady);
            StopCommand             = new RelayCommand(StopMeasure, CheckEnableStop);
            TogglePauseCommand      = new RelayCommand(TogglePause, CheckCanPause);
            SaveManualCommand       = new AsyncRelayCommand(ExecuteSaveManual, () => Leather.Count > 0);
            LoadManualCommand       = new RelayCommand(ExecuteLoadManual);
            //StartCommand = new RelayCommand(StartMeasure, CheckEnableStart);

            AddItemCommand = new RelayCommand(AddItem, CheckEnableAdd);
            DeleteItemCommand       = new RelayCommand(DeleteSelectedItems, () => CanProcessData);
            //DeleteItemCommand       = new RelayCommand(DeleteItem, CheckDataGridItem);
            NextPackingCommand      = new RelayCommand(ExecuteNextPacking, CheckEnableNextPacking);
            PrintA4Command          = new RelayCommand(PrintA4Form, CheckEnablePrintA4);
            PrintA5Command          = new RelayCommand(PrintA5Form, CheckEnablePrintA5);
            PrintPackingCommand     = new RelayCommand(PrintPackingForm, CheckEnablePrintPacking);
            ReprintA4Command        = new RelayCommand(PrintA4Form, CheckEnableReprint);
            ReprintA5Command        = new RelayCommand(PrintA5Form, CheckEnableReprint);
            SaveCommand             = new RelayCommand(SaveData, CheckEnableSave);

            TestSendDataCommand = new RelayCommandObj(TestSendData);

            _dataService = dataService;
            rtnRst = _dataService.GetRfcPwd();

            SessionDuration = "Duration: 00:00:00";
            TotalArea = 0.000m;
            TxtTotalArea = TotalArea.ToString("#,#.###");

            PackingTotalArea = 0.000m;
            TxtPackingTotalArea = PackingTotalArea.ToString("#,#.###");

            _ = InitializeAsync();

            //IsStarted = false;
        }

        private async Task InitializeAsync()
        {
            if (AvailablePorts.Count > 0)
            {
                await ConnectSerialPort();
            }

            if (!rtnRst.GetValue().IsSuccess)
            {
                await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        rtnRst.GetValue().Message,
                        PackIconKind.Error,
                        Brushes.Crimson
                    );
            }
            else
            {
                sapPass = rtnRst.GetValue().Message;
            }

            // 1. Load first
            var recovered = LoadWithFailover();
            if (recovered != null)
            {
                await LoadRecoveryData(recovered);
                IsPaused = true;
                //OrderNo = recovered.OrderNo;

                //// Now we can safely await the SAP data
                //await GetOrderInfo();

                //// Restore the rest of the fields after SAP has populated the ComboBoxes
                //try
                //{
                //    // This assumes MsDate is "yyyy-MM-dd" and MsTime is "HH:mm:ss"
                //    startDateTime = DateTime.Parse($"{recovered.MsDate} {recovered.MsTime}");
                //    _elapsedTime = recovered.ElapsedTime;
                //}
                //catch
                //{
                //    // Fallback if recovery strings are empty or corrupted
                //    startDateTime = DateTime.Now;
                //    _elapsedTime = TimeSpan.Zero;
                //}

                //StartDate = startDateTime.ToString("yyyy-MM-dd");
                //StartTime = startDateTime.ToString("HH:mm:ss");
                //SelectedPersonnel = Personnels.FirstOrDefault(p => p.Value == recovered.PersonnelNo);
                //SelectedWageGroup = WageGroups.FirstOrDefault(p => p.Value == recovered.WageGroup);
                //SelectedParallelUnit = ParallelUnits.FirstOrDefault(p => p.Value == recovered.PUom);
                //SelectedStorageLocation = StorageLocations.FirstOrDefault(p => p.Value == recovered.StoreLoc);
                //SelectedGrade = recovered.Grade;
                //SelectedNGReason = NGReasons.FirstOrDefault(p => p.Value == recovered.NgReason);

                //Leather.Clear();
                //foreach (var item in recovered.MeasureData) Leather.Add(item);
                //if (Leather.Count > 0)
                //{
                //    SelectedLeather = Leather.LastOrDefault();
                //}

                //PackingLeather.Clear();
                //foreach (var item in recovered.PackingData) PackingLeather.Add(item);
                //if (PackingLeather.Count > 0)
                //{
                //    SelectedPackingLeather = PackingLeather.LastOrDefault();
                //}

                //if (Leather.Any())
                //{
                //    PackingBatch.Clear();

                //    // Group all measured leather by their Group number
                //    var groups = Leather.GroupBy(x => x.Group)
                //                        .OrderBy(g => g.Key); // Keep them in order 1, 2, 3...

                //    foreach (var g in groups)
                //    {
                //        var batchSummary = new LeatherArea
                //        {
                //            Group = g.Key,
                //            Area = g.Sum(x => x.Area),   // Sum of all leather in this group
                //            Piece = g.Count()             // Total pieces in this group
                //        };

                //        PackingBatch.Add(batchSummary);
                //    }

                //    // Update the "Running Totals" for the UI
                //    TotalArea = Leather.Sum(x => x.Area);
                //    TxtTotalArea = TotalArea.ToString("#,##0.000");

                //    if (IsPacking)
                //    {
                //        PackingTotalArea = PackingLeather.Sum(x => x.Area);
                //        TxtPackingTotalArea = PackingTotalArea.ToString("#,##0.000");
                //    }

                //    if (recovered.PackingData.Any())
                //    {
                //        // Ensure CurrentGroup matches the bundle we are currently working on
                //        CurrentGroup = recovered.PackingData.First().Group;
                //    }
                //}

                //CurrentState = DeviceState.StartMeasuring;
            }

            // 2. Start the safety net
            _recoveryTimer.Interval = TimeSpan.FromSeconds(1);
            _recoveryTimer.Tick -= RecoveryTimer_Tick;
            _recoveryTimer.Tick += RecoveryTimer_Tick;
            _recoveryTimer.Start();
        }

        private string? _selectedPortName;
        public string? SelectedPortName
        {
            get { return _selectedPortName; }
            set
            {
                if (_selectedPortName != value)
                {
                    _selectedPortName = value;
                    OnPropertyChanged(nameof(SelectedPortName));
                }
            }
        }

        public ObservableCollection<string> AvailablePorts { get; set; }
        private void RefreshPorts()
        {
            AvailablePorts.Clear();

            // Get the list of friendly names from the helper we built
            var ports = ConfigHelper.GetFriendlyPortNames();

            foreach (string port in ports)
            {
                AvailablePorts.Add(port);

                // Check if the name starts with your specific ATEN bridge string
                if (port.StartsWith("ATEN USB to Serial Bridge", StringComparison.OrdinalIgnoreCase))
                {
                    SelectedPortName = port;
                }
            }

            // Fallback: If no ATEN port was found but there are other ports, 
            // and nothing is selected yet, you might want to pick the first one
            if (AvailablePorts.Count > 0 && string.IsNullOrEmpty(SelectedPortName))
            {
                SelectedPortName = AvailablePorts[0];
            }
            else if (AvailablePorts.Count == 0)
            {
                SelectedPortName = null;
            }
        }

        private SerialPort? _serialPort;
        private int _baudRate = 9600; // Default baud rate
        public int BaudRate
        {
            get { return _baudRate; }
            set
            {
                if (_baudRate != value)
                {
                    _baudRate = value;
                    OnPropertyChanged(nameof(BaudRate));
                }
            }
        }
        private bool _isConnected = false;
        private async Task ConnectSerialPort()
        {
            if (string.IsNullOrEmpty(SelectedPortName))
            {
                await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        "Please select a serial port.",
                        PackIconKind.WarningCircle,
                        Brushes.DarkOrange
                    );

                return;
            }

            string comName = SelectedPortName.Split('(', ')')[1];

            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
                _serialPort.Dispose();
            }

            _serialPort = new SerialPort(comName, BaudRate, Parity.None, 8, StopBits.One);
            _serialPort.Handshake = Handshake.None;
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;

            try
            {
                _serialPort.Open();
                _serialPort.DataReceived += SerialPort_DataReceived;
                _isConnected = true;
                CurrentState = DeviceState.Connected;
                //_isStarted = true;
                //IsStarted = true;
                ConnectionButtonIcon = "LanDisconnect";
                ConnectionButtonText = "Disconnect";
                ReceivedData = "Connected to " + comName + "\n";
            }
            catch (UnauthorizedAccessException ex)
            {
                await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        $"Error: Access to port {comName} is denied. It might be in use by another application. {ex.Message}",
                        PackIconKind.Error,
                        Brushes.Crimson
                    );

                _isConnected = false;
                CurrentState = DeviceState.Disconnected;
            }
            catch (InvalidOperationException ex)
            {
                await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        $"Error: The port {comName} is already open. {ex.Message}",
                        PackIconKind.Error,
                        Brushes.Crimson
                    );

                _isConnected = false;
                CurrentState = DeviceState.Disconnected;
            }
            catch (System.IO.IOException ex)
            {
                await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        $"Error: Port {comName} does not exist or is unplugged. {ex.Message}",
                        PackIconKind.Error,
                        Brushes.Crimson
                    );

                _isConnected = false;
                CurrentState = DeviceState.Disconnected;
            }
            catch (Exception ex)
            {
                await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        $"An unexpected error occurred while opening port {comName}: {ex.Message}",
                        PackIconKind.Error,
                        Brushes.Crimson
                    );

                _isConnected = false;
                CurrentState = DeviceState.Disconnected;
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;

            // 1. Read everything available on the background thread
            string incomingData = sp.ReadExisting();
            _serialBuffer += incomingData;

            // 2. Process ALL complete messages currently in the buffer
            // Use a while loop in case the machine sent 2-3 rows at once
            while (_serialBuffer.Contains("\r") || _serialBuffer.Contains("\n"))
            {
                int indexR = _serialBuffer.IndexOf('\r');
                int indexN = _serialBuffer.IndexOf('\n');

                // Find the first occurring terminator
                int tIndex = (indexR != -1 && indexN != -1) ? Math.Min(indexR, indexN) : Math.Max(indexR, indexN);

                string completeMessage = _serialBuffer.Substring(0, tIndex);
                _serialBuffer = _serialBuffer.Substring(tIndex + 1);

                string cleanedMessage = completeMessage.Trim();
                if (string.IsNullOrEmpty(cleanedMessage)) continue;

                // 3. Only switch to the UI thread once we have a valid number
                if (decimal.TryParse(cleanedMessage, out decimal leatherArea) && leatherArea > 0)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (CurrentState == DeviceState.StartMeasuring)
                        {
                            ProcessNewMeasurement(leatherArea);
                        }
                    });
                }
            }
        }

        // Move the logic to a separate method to keep the code clean
        private bool _isDataDirty = false;
        private void ProcessNewMeasurement(decimal leatherArea)
        {
            var newItem = new LeatherArea
            {
                Rank = Leather.Count + 1,
                PackingRank = PackingLeather.Count + 1,
                Area = leatherArea,
                Group = CurrentGroup
            };

            newItem.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(LeatherArea.IsSelected))
                {
                    // If any item is unchecked, the 'Select All' isn't true anymore
                    if (s is LeatherArea item && !item.IsSelected)
                    {
                        _isAllSelected = false;
                    }
                    OnPropertyChanged(nameof(IsAllSelected));
                }
            };

            Leather.Add(newItem);

            if (Leather.Count > 0)
            {
                SelectedLeather = Leather.LastOrDefault();
            }

            TotalArea += leatherArea;
            TxtTotalArea = TotalArea.ToString("#,##0.000");

            if (IsPacking)
            {
                PackingLeather.Add(newItem);

                if (PackingLeather.Count > 0)
                {
                    SelectedPackingLeather = PackingLeather.LastOrDefault();
                }

                PackingTotalArea += leatherArea;
                TxtPackingTotalArea = PackingTotalArea.ToString("#,##0.000");
            }

            _isDataDirty = true;
        }

        public bool CanProcessData => IsStarted && !IsPaused && !IsBusy;
        public bool CanAddData => !IsPaused && !IsBusy && IsReadyToAdd && CurrentState == DeviceState.StartMeasuring;
        private bool _isPaused = false;
        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                if (SetProperty(ref _isPaused, value))
                {
                    OnPropertyChanged(nameof(CanProcessData));
                    OnPropertyChanged(nameof(CanAddData));
                }
            }
        }
        private DispatcherTimer _recoveryTimer = new DispatcherTimer();
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private string? _sessionDuration;
        public string? SessionDuration
        {
            get => _sessionDuration;
            set { _sessionDuration = value; OnPropertyChanged(); }
        }
        private void UpdateTimerUI()
        {
            // Format: HH:mm:ss (e.g., 01:20:05)
            // We use :D2 to ensure 2 digits (e.g., "05" instead of "5")
            SessionDuration = string.Format("Duration: {0:D2}:{1:D2}:{2:D2}",
                (int)_elapsedTime.TotalHours,
                _elapsedTime.Minutes,
                _elapsedTime.Seconds);
        }
        private async void RecoveryTimer_Tick(object? sender, EventArgs e)
        {
            // 1. Auto-save if data changed (for recovery)
            if (_isDataDirty)
            {
                await SaveTempSession();
                _isDataDirty = false;
            }

            // 2. The Condition: Only count if we are Measuring AND not Paused
            // We remove the check for 'ConfirmCancel' or 'ConfirmStop' because 
            // usually, you don't want the timer running while a dialog is open.
            if (IsPaused || CurrentState != DeviceState.StartMeasuring)
                return;

            // 3. Increment the timer by 1 second (Assuming 1s Interval)
            _elapsedTime = _elapsedTime.Add(TimeSpan.FromSeconds(1));

            // 4. Update the UI String
            // Using TotalHours handles cases where a shift might exceed 24 hours.
            UpdateTimerUI();
        }

        //private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        try
        //        {
        //            SerialPort sp = (SerialPort)sender;

        //            // 1. Read data and append to the class-level buffer
        //            string incomingData = sp.ReadExisting();
        //            _serialBuffer += incomingData;

        //            // 2. Look for the message terminator (\r or \n).
        //            // Assuming your device sends a full message ending in \r or \n (or \r\n).
        //            int terminatorIndex = _serialBuffer.IndexOf('\r');
        //            if (terminatorIndex == -1)
        //            {
        //                terminatorIndex = _serialBuffer.IndexOf('\n');
        //            }

        //            // 3. Process only if a complete message is available
        //            if (terminatorIndex >= 0)
        //            {
        //                // Extract the complete message (the number string)
        //                string completeMessage = _serialBuffer.Substring(0, terminatorIndex);

        //                // Remove the message and the terminator from the buffer
        //                _serialBuffer = _serialBuffer.Substring(terminatorIndex + 1);

        //                // **CLEAN AND PARSE**
        //                // Clean up any remaining whitespace/line endings from the message
        //                string cleanedMessage = completeMessage.Trim();

        //                if (decimal.TryParse(cleanedMessage, out decimal leatherArea))
        //                {
        //                    if (leatherArea > 0)
        //                    {
        //                        if (CurrentState == DeviceState.StartMeasuring)
        //                        {
        //                            //Leather.Add(new LeatherArea { Rank = Leather.Count + 1, Area = leatherArea });
        //                            var newItem = new LeatherArea
        //                            {
        //                                Rank = Leather.Count + 1,
        //                                PackingRank = PackingLeather.Count + 1,
        //                                Area = leatherArea,
        //                                Group = CurrentGroup // Assign the current active group
        //                            };

        //                            // Subscribe to property changes of the individual item
        //                            newItem.PropertyChanged += (s, args) =>
        //                            {
        //                                if (args.PropertyName == nameof(LeatherArea.IsSelected))
        //                                {
        //                                    // If any item is unchecked, the 'Select All' isn't true anymore
        //                                    if (!((LeatherArea)s).IsSelected)
        //                                        _isAllSelected = false;
        //                                    OnPropertyChanged(nameof(IsAllSelected));
        //                                }
        //                            };

        //                            Leather.Add(newItem);

        //                            if (Leather.Count > 0)
        //                            {
        //                                SelectedLeather = Leather.LastOrDefault();
        //                            }

        //                            TotalArea += leatherArea;
        //                            TxtTotalArea = TotalArea.ToString("#,##0.000");

        //                            if (IsPacking)
        //                            {
        //                                PackingLeather.Add(newItem);

        //                                if (PackingLeather.Count > 0)
        //                                {
        //                                    SelectedPackingLeather = PackingLeather.LastOrDefault();
        //                                }

        //                                PackingTotalArea += leatherArea;
        //                                TxtPackingTotalArea = PackingTotalArea.ToString("#,##0.000");
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    // Log or handle the case where the data is corrupt/non-numeric
        //                    ReceivedData += $"Failed to parse: '{cleanedMessage}'\n";
        //                }
        //            }
        //            // If terminatorIndex is -1, the buffer is incomplete. Do nothing and wait for more data.
        //        }
        //        catch (TimeoutException) { }
        //        catch (Exception ex)
        //        {
        //            ReceivedData += $"Error reading: {ex.Message}\n";
        //        }
        //    });
        //}

        private void TestSendData(object? parameter)
        {
            if (CurrentState == DeviceState.StartMeasuring)
            {
                Random rnd = new Random();
                decimal max = 8.000m;
                decimal min = 4.000m;
                decimal randomNumberInRange = Math.Round((decimal)rnd.NextDouble() * (max - min) + min, 3);
                ProcessNewMeasurement(randomNumberInRange);
            }
        }

        private async Task DisconnectSerialPort()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                try
                {
                    _serialPort.DataReceived -= SerialPort_DataReceived;
                    _serialPort.Close();
                    _serialPort.Dispose();
                    _isConnected = false;
                    //_isStarted = false;
                    //IsStarted = false;
                    //ResetData();
                    ConnectionButtonIcon = "LanConnect";
                    ConnectionButtonText = "Connect";
                    string comName = SelectedPortName?.Split('(', ')')[1] ?? string.Empty;
                    ReceivedData += "Disconnected from " + comName + "\n";
                    CurrentState = DeviceState.Disconnected;
                }
                catch (Exception ex)
                {
                    await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        $"Error disconnecting from serial port: {ex.Message}",
                        PackIconKind.Error,
                        Brushes.Crimson
                    );
                }
            }
        }

        private async Task ToggleConnection()
        {
            CurrentState = _isConnected ? DeviceState.Connected : DeviceState.Disconnected;

            if (_isConnected)
            {
                await DisconnectSerialPort();
            }
            else
            {
                await ConnectSerialPort();
            }
        }

        private bool CheckInputOrderNo()
        {
            // Check for null or whitespace
            return !string.IsNullOrWhiteSpace(OrderNo);
        }

        private async Task GetOrderInfo()
        {
            if (CurrentState == DeviceState.Disconnected)
            {
                await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        "Please connected Serial Port",
                        PackIconKind.WarningCircle,
                        Brushes.DarkOrange
                    );

                IsFocused = true;
            }
            else if (OrderNo.Length < 12)
            {
                await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        "Please enter 12 digit",
                        //PackIconKind.QuestionMarkCircle,
                        //Brushes.Goldenrod
                        PackIconKind.WarningCircle,
                        Brushes.DarkOrange
                    );

                IsFocused = true;
            }
            else
            {
                IsBusy = true;

                try
                {
                    // Define your SAP user credentials
                    var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
                    binding.MaxReceivedMessageSize = 2147483647; // Set to 2GB (Max Int32)
                    binding.MaxBufferSize = 2147483647;

                    // Also update the ReaderQuotas if you have large strings or arrays
                    binding.ReaderQuotas.MaxStringContentLength = 2147483647;
                    binding.ReaderQuotas.MaxArrayLength = 2147483647;

                    binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                    var endpoint = new EndpointAddress(arraySapEndpoint["z_check_measure_op"]);

                    // Set parameter
                    var pdoParams = new Z_CHECK_MEASURE_OPRequest
                    {
                        I_PD_NO = OrderNo,
                        T_EMP = new PA0001[] { },
                        T_NGRS = new TBICCF[] { },
                        T_PU = new T006A[] { },
                        T_QAG = new TBICCF[] { },
                        T_SL = new T001L[] { },
                        T_WG = new T421T[] { }
                    };

                    var client = new Z_CHECK_MEASURE_OPClient(binding, endpoint);

                    // Set the SAP credentials
                    client.ClientCredentials.UserName.UserName = _dataService.SapUser;

                    if (sapPass != null)
                    {
                        client.ClientCredentials.UserName.Password = sapPass;
                    }
                    else
                    {
                        throw new System.NullReferenceException("Could not get RFC password");
                    }

                    // Call the web service method
                    var response = await client.Z_CHECK_MEASURE_OPAsync(pdoParams);
                    // Process the result
                    if (response.E_OD_INFO.GMEIN == "0")
                    {
                        throw new Exception($"Order No. {OrderNo} not found");
                    }
                    else
                    {
                        MapTtValue(response.E_PP_TT);

                        Oi = response.E_OD_INFO;

                        MatNo = Oi.PLNBEZ;
                        MatDesc = Oi.MAKTX;
                        OrderQty.Total = Oi.GAMNG;
                        OrderQty.Delivered = Oi.ISM04;
                        OrderQty.Remaining = Oi.ISM05;
                        OperNo = Tt[0].OPERATION;
                        Unit = Tt[0].CONF_QUAN_UNIT;

                        GetNGReasons(response.T_NGRS);
                        GetLeatherGrades(response.T_QAG);
                        GetParallelUnits(response.T_PU);
                        GetPersonnels(response.T_EMP);
                        GetStorageLocations(response.T_SL);
                        GetWageGroups(response.T_WG);

                        rtnRst = _dataService.GetMaterialDescription(MatNo);
                        if (rtnRst.GetValue().IsSuccess)
                        {
                            ColorArticle = rtnRst.GetValue().Message;
                        }
                        else
                        {
                            ColorArticle = await _dataService.GetCustomerColorArticle(MatNo);
                        }

                        endpoint = new EndpointAddress(arraySapEndpoint["z_get_hu_info"]);

                        var huParams = new Z_GET_HU_INFORequest
                        {
                            I_GET_PI = ""
                        };

                        var huClient = new Z_GET_HU_INFOClient(binding, endpoint);

                        // Set the SAP credentials
                        huClient.ClientCredentials.UserName.UserName = _dataService.SapUser;

                        if (sapPass != null)
                        {
                            huClient.ClientCredentials.UserName.Password = sapPass;
                        }
                        else
                        {
                            throw new System.NullReferenceException("Could not get RFC password");
                        }

                        // Call the web service method
                        var huResponse = await huClient.Z_GET_HU_INFOAsync(huParams);
                        GetPackagings(huResponse.E_PK.ToArray());

                        if (Leather.Count == 0)
                        {
                            CurrentState = DeviceState.Ready;
                        }
                    }
                }
                catch (System.ServiceModel.FaultException faultEx)
                {
                    if (faultEx.Message == "ORDER_NOT_FOUND")
                    {
                        ErrMsg = "Order Not Found";
                    }
                    else if (faultEx.Message == "OPERATION_NOT_FOUND")
                    {
                        ErrMsg = "Operation Not Found";
                    }
                    else if (faultEx.Message == "ONE_OP")
                    {
                        ErrMsg = "Order Has One Operation Only";
                    }
                    else if (faultEx.Message == "LAST_OP_NOT_MEA")
                    {
                        ErrMsg = "Last Operation Not Measure";
                    }
                    else if (faultEx.Message == "LAST_OP_STAT_CHECK")
                    {
                        ErrMsg = "Check Last Operation Order Status";
                    }
                    else if (faultEx.Message == "PREV_OP_NOT_CF")
                    {
                        ErrMsg = "Previous Operation Not Confirm";
                    }
                    else
                    {
                        ErrMsg = faultEx.Message;
                    }

                    await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        ErrMsg,
                        PackIconKind.WarningCircle,
                        Brushes.DarkOrange
                    );

                    ResetData();
                }
                catch (Exception ex)
                {
                    await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        ex.Message,
                        PackIconKind.WarningCircle,
                        Brushes.DarkOrange
                    );

                    ResetData();
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        public ObservableCollection<ComboItem> ConfirmTypes { get; set; }
        private void GetConfirmTypes()
        {
            ConfirmTypes.Clear();

            ConfirmTypes.Add(new ComboItem { Value = "", Text = "Partial confirmation" });
            //ConfirmTypes.Add(new ComboBox { Value = "X", Text = "Final confirmation" });
            //ConfirmTypes.Add(new ComboBox { Value = "1", Text = "Automatic final confirmation" });

            SelectedConfirmType = ConfirmTypes[0];
        }
        private ComboItem? _selectedConfirmType;
        public ComboItem? SelectedConfirmType
        {
            get => _selectedConfirmType;
            set
            {
                _selectedConfirmType = value;
                OnPropertyChanged(nameof(SelectedConfirmType));
            }
        }

        public ObservableCollection<ComboItem> Personnels { get; set; }
        private void GetPersonnels(PA0001[] T_Emp)
        {
            Personnels.Clear();

            Personnels.Add(new ComboItem { Value = "", Text = "-" });

            foreach (PA0001 item in T_Emp)
            {
                Personnels.Add(new ComboItem { Value = item.PERNR, Text = $"{item.PERNR.Substring(1)} {item.ENAME}" });
            }

            SelectedPersonnel = Personnels[0];
        }
        private ComboItem? _selectedPersonnel;
        public ComboItem? SelectedPersonnel
        {
            get => _selectedPersonnel;
            set
            {
                _selectedPersonnel = value;
                OnPropertyChanged(nameof(SelectedPersonnel));
            }
        }

        public ObservableCollection<ComboItem> WageGroups { get; set; }
        private void GetWageGroups(T421T[] T_Wg)
        {
            WageGroups.Clear();

            WageGroups.Add(new ComboItem { Value = "", Text = "-" });

            foreach (T421T item in T_Wg)
            {
                WageGroups.Add(new ComboItem { Value = item.LOGRP, Text = $"{item.LOGRP} {item.TXT}" });
            }

            SelectedWageGroup = WageGroups[0];
        }
        private ComboItem? _selectedWageGroup;
        public ComboItem? SelectedWageGroup
        {
            get => _selectedWageGroup;
            set
            {
                _selectedWageGroup = value;
                OnPropertyChanged(nameof(SelectedWageGroup));
            }
        }

        public ObservableCollection<ComboItem> ParallelUnits { get; set; }
        Dictionary<string, string> arrayUom = new Dictionary<string, string>();
        private void GetParallelUnits(T006A[] T_Pu)
        {
            ParallelUnits.Clear();
            arrayUom.Clear();

            foreach (T006A item in T_Pu)
            {
                if (item.MSEHI != Unit)
                {
                    ParallelUnits.Add(new ComboItem { Value = item.MSEHI, Text = $"{item.MSEH3} {item.MSEHL}" });
                }

                arrayUom.Add(item.MSEHI, item.MSEH6);
            }

            SelectedParallelUnit = ParallelUnits[0];
        }
        private ComboItem? _selectedParallelUnit;
        public ComboItem? SelectedParallelUnit
        {
            get => _selectedParallelUnit;
            set
            {
                _selectedParallelUnit = value;
                OnPropertyChanged(nameof(SelectedParallelUnit));
            }
        }

        public ObservableCollection<ComboItem> StorageLocations { get; set; }
        private void GetStorageLocations(T001L[] T_Sl)
        {
            int cnt = 0;

            StorageLocations.Clear();

            foreach (T001L item in T_Sl)
            {
                StorageLocations.Add(new ComboItem { Value = item.LGORT, Text = $"{item.LGORT} {item.LGOBE}" });

                if (item.LGORT.Substring(0, 1) == "W")
                {
                    SelectedStorageLocation = StorageLocations[cnt];
                }

                cnt++;
            }
        }
        private ComboItem? _selectedStorageLocation;
        public ComboItem? SelectedStorageLocation
        {
            get => _selectedStorageLocation;
            set
            {
                _selectedStorageLocation = value;
                OnPropertyChanged(nameof(SelectedStorageLocation));
            }
        }

        public ObservableCollection<string> LeatherGrades { get; set; }
        private void GetLeatherGrades(TBICCF[] T_QAG)
        {
            LeatherGrades.Clear();

            foreach (TBICCF item in T_QAG)
            {
                LeatherGrades.Add(item.TABNAME);
            }

            SelectedGrade = LeatherGrades[0];
        }
        private string? _selectedGrade;
        public string? SelectedGrade
        {
            get => _selectedGrade;
            set
            {
                if (_selectedGrade != value)
                {
                    _selectedGrade = value;
                    OnPropertyChanged();

                    OnPropertyChanged(nameof(IsNGReasonEnabled));

                    if (_selectedGrade != "NG" && NGReasons.Count > 0) SelectedNGReason = NGReasons[0];

                    //if (_selectedGrade == "NC" || _selectedGrade == "NG") IsPacking = false;

                    //if (!IsPackingAllowed) IsPacking = false;
                }
            }
        }

        public ObservableCollection<ComboItem> NGReasons { get; set; }
        private void GetNGReasons(TBICCF[] T_NGRS)
        {
            NGReasons.Clear();

            NGReasons.Add(new ComboItem { Value = "", Text = "-" });

            foreach (TBICCF item in T_NGRS)
            {
                NGReasons.Add(new ComboItem { Value = item.TABNAME, Text = $"{item.TABNAME} : {item.TABFIELD}" });
            }

            SelectedNGReason = NGReasons[0];
        }
        private ComboItem? _selectedNGReason;
        public ComboItem? SelectedNGReason
        {
            get => _selectedNGReason;
            set
            {
                _selectedNGReason = value;
                OnPropertyChanged(nameof(SelectedNGReason));
            }
        }
        private int _currentMsId = 0;
        public int CurrentMsId
        {
            get => _currentMsId;
            set => SetProperty(ref _currentMsId, value);
        }

        public ObservableCollection<ComboItem> Packagings { get; set; }
        Dictionary<string, string> arrayPackaging = new Dictionary<string, string>();
        private void GetPackagings(ZSTHU[] e_PK)
        {
            Packagings.Clear();
            arrayPackaging.Clear();

            var sortedPk = e_PK.OrderBy(item => item.MAKTX);

            foreach (ZSTHU item in sortedPk)
            {
                if (item.MATNR != Packaging)
                {
                    Packagings.Add(new ComboItem { Value = item.MATNR, Text = $"{item.MAKTX}" });
                }

                arrayPackaging.Add(item.MATNR, item.MAKTX);
            }

            SelectedPackaging = Packagings[0];
        }
        private ComboItem? _selectedPackaging;
        public ComboItem? SelectedPackaging
        {
            get => _selectedPackaging;
            set
            {
                _selectedPackaging = value;
                OnPropertyChanged(nameof(SelectedPackaging));
            }
        }

        private async void ResetCommandExecute()
        {
            var result = await ConfigHelper.ShowSmartDialog(
                        "ConfirmTemplate",
                        "Are you sure you want to measure new PDO?",
                        PackIconKind.QuestionMarkCircle,
                        Brushes.Goldenrod,
                        "CONFIRM",
                        "CANCEL"
                    );

            if (result)
            {
                // Call the silent method only if they say Yes
                ResetData();
            }
        }

        public void ResetData()
        {
            OrderQty = new OrderQuantity();

            Leather.Clear();
            PackingBatch.Clear();
            PackingLeather.Clear();
            md.Clear();

            OrderNo = string.Empty;
            MatNo = string.Empty;
            Unit = string.Empty;
            MatDesc = string.Empty;
            ColorArticle = string.Empty;

            if (_dataService != null)
            {
                _dataService.Response = new Z_GET_MAT_COL_ARTResponse();

                // 3. CRITICAL: Tell the UI the DataService itself has "New Data"
                // This forces WPF to re-read all bindings that start with "_dataService."
                OnPropertyChanged(nameof(_dataService));
            }

            OperNo = string.Empty;

            //SelectedConfirmType = ConfirmTypes[0];

            Personnels.Clear();
            WageGroups.Clear();
            ParallelUnits.Clear();
            arrayUom.Clear();
            StorageLocations.Clear();
            LeatherGrades.Clear();
            NGReasons.Clear();
            Packagings.Clear();

            //_isStarted = true;
            IsStarted = false;
            StartDate = string.Empty;
            StartTime = "00:00:00";
            //_isStopped = false;
            EndDate = string.Empty;
            EndTime = "00:00:00";
            SessionDuration = "Duration: 00:00:00";
            CurrentMsId = 0;
            CurrentGroup = 1;

            TotalArea = 0.000m;
            TxtTotalArea = string.Empty;
            PackingTotalArea = 0.000m;
            TxtPackingTotalArea = string.Empty;

            IsFocused = true;
            //_hasSavedSuccessfully = false;
            CurrentState = DeviceState.Connected;
            DeleteAllTempFiles();
        }

        public DateTime startDateTime;
        private async Task StartMeasure()
        {
            if (SelectedGrade == "NG" && string.IsNullOrEmpty(SelectedNGReason?.Value))
            {
                await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        "Please selected NG Reason",
                        PackIconKind.WarningCircle,
                        Brushes.DarkOrange
                    );
            }
            else
            {
                var previousState = CurrentState;

                CurrentState = DeviceState.ConfirmStartMeasuring;

                var result = await ConfigHelper.ShowSmartDialog(
                        "ConfirmTemplate",
                        "Are you sure you want to start measuring with current settings?",
                        PackIconKind.QuestionMarkCircle,
                        Brushes.Goldenrod,
                        "CONFIRM",
                        "CANCEL"
                    );

                if (result)
                {
                    // 1. Reset the stopwatch for the new round
                    _elapsedTime = TimeSpan.Zero;
                    SessionDuration = "Duration: 00:00:00";

                    // 2. Capture the actual Start Date/Time for SAP (MsDate/MsTime)
                    startDateTime = DateTime.Now;
                    StartDate = startDateTime.ToString("yyyy-MM-dd");
                    StartTime = startDateTime.ToString("HH:mm:ss");

                    Leather.Clear();
                    PackingBatch.Clear();
                    PackingLeather.Clear();

                    TotalArea = 0.000m;
                    TxtTotalArea = "";
                    PackingTotalArea = 0.000m;
                    TxtPackingTotalArea = "";
                    CurrentGroup = 1;

                    if (_serialPort?.IsOpen == true)
                    {
                        // Clear the hardware buffers so old "garbage" data is thrown away
                        _serialPort.DiscardInBuffer();
                        _serialPort.DiscardOutBuffer();

                        // Clear your software buffer too
                        _serialBuffer = string.Empty;
                    }

                    CurrentState = DeviceState.StartMeasuring;
                    IsPaused = false;
                    IsStarted = true;
                }
                else
                {
                    CurrentState = previousState;
                }
            }
        }

        private async Task BackToReady()
        {
            var previousState = CurrentState;

            CurrentState = DeviceState.ConfirmCancel;

            var result = await ConfigHelper.ShowSmartDialog(
                        "ConfirmTemplate",
                        "Are you sure you want to cancel current measuring?",
                        PackIconKind.QuestionMarkCircle,
                        Brushes.Goldenrod,
                        "CONFIRM",
                        "CANCEL"
                    );

            if (result)
            {
                Leather.Clear();
                PackingBatch.Clear();
                PackingLeather.Clear();

                TotalArea = 0.000m;
                PackingTotalArea = 0.000m;

                TxtTotalArea = "";
                TxtPackingTotalArea = "";
                StartDate = "";
                StartTime = "";
                SessionDuration = "Duration: 00:00:00";

                CurrentState = DeviceState.Ready;
                DeleteAllTempFiles();
            }
            else
            {
                CurrentState = previousState;
            }
        }

        private bool CheckCanPause()
        {
            return CurrentState == DeviceState.StartMeasuring;
        }
        private async void TogglePause()
        {
            IsPaused = !IsPaused; // Flip the true/false value

            if (IsPaused)
            {
                // Optional: Notify the user it's paused
                await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        "Measurement Paused",
                        PackIconKind.SuccessCircle,
                        Brushes.ForestGreen
                    );
            }
            else
            {
                await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        "Measurement Resumed",
                        PackIconKind.SuccessCircle,
                        Brushes.ForestGreen
                    );
            }
        }

        private bool CheckEnableStop()
        {
            if (CurrentState == DeviceState.StartMeasuring && Leather.Count > 0 && PackingLeather.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task StopMeasure()
        {
            var previousState = CurrentState;

            CurrentState = DeviceState.ConfirmStopMeasuring;

            var result = await ConfigHelper.ShowSmartDialog(
                        "ConfirmTemplate",
                        "Are you sure you want to stop measuring?",
                        PackIconKind.QuestionMarkCircle,
                        Brushes.Goldenrod,
                        "CONFIRM",
                        "CANCEL"
                    );

            if (result)
            {
                DateTime endDateTime = DateTime.Now;
                EndDate = endDateTime.ToString("yyyy-MM-dd");
                EndTime = endDateTime.ToString("HH:mm:ss");
                CurrentState = DeviceState.StopMeasuring;
                IsStarted = false;
            }
            else
            {
                CurrentState = previousState;
            }
        }

        private readonly object _fileLock = new object();
        public string StatusMessage = string.Empty;
        private void RotateBackups(string filePath)
        {
            lock (_fileLock)
            {
                for (int i = 2; i >= 1; i--)
                {
                    string oldFile = i == 1 ? filePath : $"{filePath}.bak{i - 1}";
                    string newFile = $"{filePath}.bak{i}";

                    if (File.Exists(oldFile))
                    {
                        try { File.Copy(oldFile, newFile, true); }
                        catch { /* Ignore if a backup is locked */ }
                    }
                }
            }
        }

        private RecoverySession SetRecoveryData(string name = "")
        {
            RecoverySession session = new RecoverySession
            {
                CustomSessionName = name,
                MsId = CurrentMsId,
                OrderNo = OrderNo ?? string.Empty,
                MsDate = StartDate ?? string.Empty,
                MsTime = StartTime ?? string.Empty,
                PersonnelNo = SelectedPersonnel?.Value ?? string.Empty,
                WageGroup = SelectedWageGroup?.Value ?? string.Empty,
                PUom = SelectedParallelUnit?.Value ?? string.Empty,
                StoreLoc = SelectedStorageLocation?.Value ?? string.Empty,
                Grade = SelectedGrade ?? string.Empty,
                NgReason = SelectedNGReason?.Value ?? string.Empty,
                Packaging = SelectedPackaging?.Value ?? string.Empty,
                PackingSelected = IsPacking,
                MeasureData = Leather.ToList(), // ToList() creates a thread-safe copy
                TotalAreaSum = Leather.Sum(x => x.Area),
                CurrentGroupNumber = CurrentGroup,
                PackingData = PackingLeather.ToList(),
                TotalPackingAreaSum = PackingLeather.Sum(x => x.Area),
                PackingBatchData = PackingBatch.ToList(),
                MaterialDocRanges = md.ToList(),
                LastUpdated = DateTime.Now,
                ElapsedTime = _elapsedTime
            };

            return session;
        }

        public async Task SaveTempSession()
        {
            // Snapshot the data first on the UI thread to avoid "Collection Modified" errors
            RecoverySession sessionSnapshot = SetRecoveryData();

            bool success = false;

            await Task.Run(() =>
            {
                lock (_fileLock)
                {
                    try
                    {
                        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp_session.json");
                        string tempPath = filePath + ".tmp";

                        RotateBackups(filePath);

                        string json = JsonSerializer.Serialize(sessionSnapshot, new JsonSerializerOptions { WriteIndented = true });

                        File.WriteAllText(tempPath, json);

                        if (File.Exists(filePath)) File.Delete(filePath);
                        File.Move(tempPath, filePath);

                        success = true;
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        App.Current.Dispatcher.Invoke(() => {
                            StatusMessage = $"Warning: Auto-save failed.\n\n" + 
                                $"Message: {ex.Message}\n\n" + 
                                $"Inner Exception: {ex.InnerException?.Message ?? "None"}";
                        });
                    }
                }
            });

            // 2. Back on the UI Thread (Automatically)
            // We handle the status messages here
            if (success)
            {
                StatusMessage = $"Auto-save successful at {DateTime.Now:HH:mm:ss}";

                await Task.Delay(2000); // This works perfectly here without errors!

                // Only reset to "Ready" if no NEW message has overwritten us
                if (StatusMessage.Contains("Auto-save successful"))
                {
                    StatusMessage = "Ready";
                }
            }
            else
            {
                StatusMessage = "Warning: Auto-save failed.";
            }
        }

        public RecoverySession? LoadWithFailover()
        {
            // Ensure no other thread is writing while we are reading
            lock (_fileLock)
            {
                string mainPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp_session.json");
                var filesToTry = new List<string> { mainPath, mainPath + ".bak1", mainPath + ".bak2", mainPath + ".bak3" };

                foreach (string path in filesToTry)
                {
                    if (!File.Exists(path)) continue;

                    try
                    {
                        // We use a FileStream with 'Open' and 'Read' access for maximum safety
                        using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        var data = JsonSerializer.Deserialize<RecoverySession>(stream);

                        if (data != null) return data;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to load {path}: {ex.Message}");
                        // Continue to the next backup file...
                    }
                }
            }
            return null;
        }
        private string GetTimeAgo(DateTime lastUpdated)
        {
            var diff = DateTime.Now - lastUpdated;
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} mins ago";
            return $"{(int)diff.TotalHours} hours ago";
        }
        public async Task CheckForRecovery()
        {
            var recoveryData = LoadWithFailover(); // Your thread-safe loader

            if (recoveryData != null)
            {
                string timeAgo = GetTimeAgo(recoveryData.LastUpdated);
                string msg = $"The system found unsaved work from {recoveryData.LastUpdated:HH:mm:ss} ({timeAgo}).\n\n" +
                             $"Order: {recoveryData.OrderNo}\n" +
                             $"Pieces: {recoveryData.MeasureData.Count}\n" +
                             $"TotalArea: {recoveryData.TotalAreaSum}\n\n" +
                             "Do you want to RESTORE this session?";

                // Ask the operator if they want to restore it
                bool restore = await ConfigHelper.ShowSmartDialog(
                    "ConfirmTemplate",
                    msg,
                    PackIconKind.Restore,
                    Brushes.DeepSkyBlue,
                    "RESTORE",
                    "IGNORE"
                );

                if (restore)
                {
                    LoadManualJob(recoveryData); // Reuse your load logic
                    await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        "Session restored from auto-save.",
                        PackIconKind.SuccessCircle,
                        Brushes.ForestGreen
                    );
                }
                else
                {
                    // If they say IGNORE, NOW you can safely delete the temp file
                    DeleteAllTempFiles();
                    await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        "Auto-save cleared. Starting fresh.",
                        PackIconKind.SuccessCircle,
                        Brushes.ForestGreen
                    );
                }
            }
        }

        private void DeleteAllTempFiles()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp_session.json");
            string[] patterns = { path, path + ".bak1", path + ".bak2", path + ".bak3", path + ".tmp" };

            foreach (var file in patterns)
            {
                if (File.Exists(file)) File.Delete(file);
            }
        }

        private bool _isManualSaved = false;
        public bool IsManualSaved
        {
            get => _isManualSaved;
            set { _isManualSaved = value; OnPropertyChanged(); }
        }
        private async Task ExecuteSaveManual()
        {
            string defaultName = $"Job_{DateTime.Now:yyyyMMdd_HHmm}";

            string? jobName = await ConfigHelper.ShowInputDialog("SaveJobNameTemplate", defaultName);

            if (string.IsNullOrWhiteSpace(jobName)) return;

            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedSessions");
            string filePath = Path.Combine(directory, $"{jobName}.json");

            if (File.Exists(filePath))
            {
                // 3. Use your existing ShowSmartDialog to ask for confirmation
                bool confirmOverwrite = await ConfigHelper.ShowSmartDialog(
                    "ConfirmTemplate",
                    $"A job named '{jobName}' already exists. Do you want to overwrite it?",
                    PackIconKind.FileAlert,
                    Brushes.OrangeRed,
                    "OVERWRITE",
                    "CANCEL"
                );

                if (!confirmOverwrite) return; // User cancelled
            }

            await SaveManualJob(jobName);
            await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        $"Job '{jobName}' saved.",
                        PackIconKind.SuccessCircle,
                        Brushes.ForestGreen
                    );
        }
        public async Task SaveManualJob(string name)
        {
            RecoverySession snapshot = SetRecoveryData(name);

            await Task.Run(() => {
                lock (_fileLock)
                {
                    string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedSessions");
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    string path = Path.Combine(dir, $"{name}.json");
                    string json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(path, json);
                }
            });
            IsManualSaved = true;
        }
        private async void ExecuteLoadManual()
        {
            var loadWin = new LoadSessionWindow();

            // ShowDialog blocks until the window is closed
            if (loadWin.ShowDialog() == true)
            {
                var selected = loadWin.SelectedSession;
                if (selected != null)
                {
                    // Now call your logic method with the parameter
                    LoadManualJob(selected);

                    IsManualSaved = true; // Mark as saved since we just loaded it
                    IsPaused = true;
                    await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        "Session loaded.",
                        PackIconKind.SuccessCircle,
                        Brushes.ForestGreen
                    );
                }
            }
        }
        public async void LoadManualJob(RecoverySession session)
        {
            if (session == null) return;

            await LoadRecoveryData(session);

            //IsManualSaved = true;

            //RefreshListStats();
        }

        private async Task LoadRecoveryData(RecoverySession recovered)
        {
            OrderNo = recovered.OrderNo;

            // Now we can safely await the SAP data
            await GetOrderInfo();

            // Restore the rest of the fields after SAP has populated the ComboBoxes
            try
            {
                // This assumes MsDate is "yyyy-MM-dd" and MsTime is "HH:mm:ss"
                startDateTime = DateTime.Parse($"{recovered.MsDate} {recovered.MsTime}");
                _elapsedTime = recovered.ElapsedTime;
            }
            catch
            {
                // Fallback if recovery strings are empty or corrupted
                startDateTime = DateTime.Now;
                _elapsedTime = TimeSpan.Zero;
            }

            StartDate = startDateTime.ToString("yyyy-MM-dd");
            StartTime = startDateTime.ToString("HH:mm:ss");
            SelectedPersonnel = Personnels.FirstOrDefault(p => p.Value == recovered.PersonnelNo);
            SelectedWageGroup = WageGroups.FirstOrDefault(p => p.Value == recovered.WageGroup);
            SelectedParallelUnit = ParallelUnits.FirstOrDefault(p => p.Value == recovered.PUom);
            SelectedStorageLocation = StorageLocations.FirstOrDefault(p => p.Value == recovered.StoreLoc);
            SelectedGrade = recovered.Grade;
            SelectedNGReason = NGReasons.FirstOrDefault(p => p.Value == recovered.NgReason);
            SelectedPackaging = Packagings.FirstOrDefault(p => p.Value == recovered.Packaging);
            IsPacking = recovered.PackingSelected;
            CurrentMsId = recovered.MsId;
            CurrentGroup = recovered.CurrentGroupNumber;

            // Restore the Print list (md)
            this.md.Clear();
            if (recovered.MaterialDocRanges != null)
            {
                foreach (var range in recovered.MaterialDocRanges)
                {
                    this.md.Add(range);
                }
            }

            Leather.Clear();
            recovered.MeasureData.ForEach(x => Leather.Add(x));
            SelectedLeather = Leather.LastOrDefault();
            TotalArea = recovered.TotalAreaSum;
            TxtTotalArea = TotalArea.ToString("#,##0.000");

            if (IsPacking)
            {
                PackingLeather.Clear();
                recovered.PackingData?.ForEach(x => PackingLeather.Add(x));
                SelectedPackingLeather = PackingLeather.LastOrDefault();
                PackingTotalArea = recovered.TotalPackingAreaSum;
                TxtPackingTotalArea = PackingTotalArea.ToString("#,##0.000");

                PackingBatch.Clear();
                recovered.PackingBatchData?.ForEach(x => PackingBatch.Add(x));
            }

            IsStarted = true;
            CurrentState = DeviceState.StartMeasuring;

            UpdateTimerUI();
        }

        private bool CheckEnableSave()
        {
            // If we have items and we are finished measuring
            return Leather.Count > 0 && IsReadyToSave;
        }

        private async Task PrepareData()
        {
            int i = 1;
            string lineId = "";
            string StockType = ""; // 2 = Quality inspection, 3 = Blocked stock, "" = Unrestricted use

            if (SelectedGrade == "NG")
            {
                StockType = "3";
            }
            //else
            //{
            //    if (!IsPacking)
            //    {
            //        StockType = "2";
            //    }
            //}

            //  TimeTicket
            Tt[0].FIN_CONF = SelectedConfirmType?.Value ?? string.Empty;
            Tt[0].PERS_NO = SelectedPersonnel?.Value != "-" ? SelectedPersonnel?.Value ?? string.Empty : "";
            Tt[0].WAGEGROUP = SelectedWageGroup?.Value != "-" ? SelectedWageGroup?.Value ?? string.Empty : "";
            Tt[0].YIELD = TotalArea;
            Tt[0].EXEC_START_DATE = StartDate ?? string.Empty;
            Tt[0].EXEC_START_TIME = StartTime ?? string.Empty;
            Tt[0].EXEC_FIN_DATE = EndDate ?? string.Empty;
            Tt[0].EXEC_FIN_TIME = EndTime ?? string.Empty;
            Tt[0].POSTG_DATE = EndDate ?? string.Empty;

            Tt[0].EX_CREATED_TIME = "00:00:00";
            Tt[0].FCST_FIN_TIME = "00:00:00";
            Tt[0].LOGTIME = "00:00:00";
            Tt[0].PROC_FIN_TIME = "00:00:00";
            Tt[0].PROC_START_TIME = "00:00:00";
            Tt[0].SETUP_FIN_TIME = "00:00:00";
            Tt[0].TEARDOWN_START_TIME = "00:00:00";

            //  GoodsMovement
            Ex.Clear();
            Ex.Add(new BAPIPAREX { STRUCTURE = "/CWM/BAPI2017_GM_ITEM_CREATE" });

            Gc.GM_CODE = "02";

            Gh.DOC_DATE = StartDate;
            Gh.HEADER_TXT = "Auto GR after";
            Gh.PR_UNAME = "IHL_RFC";
            Gh.PSTNG_DATE = StartDate;
            //Gh.REF_DOC_NO = Tt[0].ORDERID;

            Gi.Clear();
            Gic.Clear();

            if (IsPacking)
            {
                foreach (LeatherArea item in PackingBatch)
                {
                    lineId = i.ToString();
                    lineId = lineId.PadLeft(6, '0');

                    Gi.Add(new BAPI2017_GM_ITEM_CREATE
                    {
                        ENTRY_QNT = item.Area,
                        ENTRY_UOM = Unit,
                        ENTRY_UOM_ISO = arrayUom.ContainsKey(Unit) ? arrayUom[Unit] : "",
                        LINE_ID = lineId,
                        MATERIAL = Oi.PLNBEZ,
                        MOVE_TYPE = "101",
                        MVT_IND = "F",
                        ORDERID = Tt[0].ORDERID,
                        PLANT = Tt[0].PLANT,
                        STGE_LOC = SelectedStorageLocation?.Value ?? string.Empty,
                        STCK_TYPE = StockType
                    });

                    Gic.Add(new _CWM_BAPI2017_GM_ITEM_CREATE
                    {
                        BASE_UOM_PME = SelectedParallelUnit?.Value ?? string.Empty,
                        ENTRY_QNT_PME = item.Piece,
                        ENTRY_UOM_PME = SelectedParallelUnit?.Value ?? string.Empty,
                        MATDOC_ITM = lineId,
                        QUANTITY_PME = item.Piece
                    });

                    i++;
                }
            }
            else
            {
                foreach (LeatherArea item in Leather)
                {
                    lineId = i.ToString();
                    lineId = lineId.PadLeft(6, '0');

                    Gi.Add(new BAPI2017_GM_ITEM_CREATE
                    {
                        ENTRY_QNT = item.Area,
                        ENTRY_UOM = Unit,
                        ENTRY_UOM_ISO = arrayUom.ContainsKey(Unit) ? arrayUom[Unit] : "",
                        LINE_ID = lineId,
                        MATERIAL = Oi.PLNBEZ,
                        MOVE_TYPE = "101",
                        MVT_IND = "F",
                        ORDERID = Tt[0].ORDERID,
                        PLANT = Tt[0].PLANT,
                        STGE_LOC = SelectedStorageLocation?.Value ?? string.Empty,
                        STCK_TYPE = StockType
                    });

                    Gic.Add(new _CWM_BAPI2017_GM_ITEM_CREATE
                    {
                        BASE_UOM_PME = SelectedParallelUnit?.Value ?? string.Empty,
                        ENTRY_QNT_PME = 1,
                        ENTRY_UOM_PME = SelectedParallelUnit?.Value ?? string.Empty,
                        MATDOC_ITM = lineId,
                        QUANTITY_PME = 1
                    });

                    i++;
                }
            }

            await GetRoutingActivity(Oi.AUFPL, Tt[0].OPERATION, TotalArea);
        }

        private async Task SaveMigo()
        {
            IsBusy = true;

            try
            {
                // Define your SAP user credentials
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
                binding.MaxReceivedMessageSize = 2147483647; // Set to 2GB (Max Int32)
                binding.MaxBufferSize = 2147483647;

                // Also update the ReaderQuotas if you have large strings or arrays
                binding.ReaderQuotas.MaxStringContentLength = 2147483647;
                binding.ReaderQuotas.MaxArrayLength = 2147483647;

                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                var endpoint = new EndpointAddress(arraySapEndpoint["z_bapi_migo"]);
                var client = new Z_BAPI_MIGOClient(binding, endpoint);

                client.ClientCredentials.UserName.UserName = _dataService.SapUser;

                if (sapPass != null)
                {
                    client.ClientCredentials.UserName.Password = sapPass;
                }
                else
                {
                    throw new System.NullReferenceException("Could not get RFC password");
                }

                var pm = new Z_BAPI_MIGORequest
                {
                    I_GM_CODE = Gc,
                    I_GM_HD = Gh,
                    I_QA_GRADE = SelectedGrade ?? string.Empty,
                    I_NG_REASON = SelectedNGReason?.Value ?? string.Empty,
                    I_TEST = "",
                    T_DR = new BAPI_CORU_RETURN[] { },
                    T_EXT = Ex.ToArray(),
                    T_GM_IT = Gi.ToArray(),
                    T_GM_IT_CWM = Gic.ToArray(),
                    T_RT = new SapMigo.BAPIRET2[] { },
                    T_RT_BATCH = new SapMigo.BAPIRET2[] { },
                    T_TT = Tt.ToArray()
                };

                var response = await client.Z_BAPI_MIGOAsync(pm);

                if (response.T_DR != null && response.T_DR.Length > 0)
                {
                    if (response.T_DR[0].TYPE != "I" && response.T_DR[0].TYPE != "S")
                    {
                        rtnRst.SetValue(false, "Fail");
                        ErrMsg = "Order Confirm ";
                        ErrMsg += response.T_DR[0].TYPE == "E" ? "Error" : "Warning";
                        ErrMsg += $"\nID : {response.T_DR[0].ID}\nNUMBER : {response.T_DR[0].NUMBER}\nPARAMETER : {response.T_DR[0].PARAMETER}\nMESSAGE : {response.T_DR[0].MESSAGE}";
                        throw new System.NullReferenceException(ErrMsg);
                    }
                    else
                    {
                        rtnRst.SetValue(true, "Success");
                        CnfNo = response.T_DR[0].CONF_NO.TrimStart('0');
                        CnfCnt = response.T_DR[0].CONF_CNT.TrimStart('0');
                    }
                }

                if (response.T_RT != null && response.T_RT.Length > 0)
                {
                    if (response.T_RT[0].TYPE != "I" && response.T_RT[0].TYPE != "S")
                    {
                        rtnRst.SetValue(false, "Fail");
                        ErrMsg = "Goods Movement Create ";
                        ErrMsg += response.T_RT[0].TYPE == "E" ? "Error" : "Warning";
                        ErrMsg += $"\nID : {response.T_RT[0].ID}\nNUMBER : {response.T_RT[0].NUMBER}\nPARAMETER : {response.T_RT[0].PARAMETER}\nMESSAGE : {response.T_RT[0].MESSAGE}";
                        throw new System.NullReferenceException(ErrMsg);
                    }
                    else
                    {
                        rtnRst.SetValue(true, "Success");
                        MatDocNo = response.E_DOC_NO;
                        await RefreshOrderQty();
                    }
                }
                else
                {
                    rtnRst.SetValue(true, "Success");
                    MatDocNo = response.E_DOC_NO;
                    await RefreshOrderQty();
                }

                if (response.T_RT_BATCH != null && response.T_RT_BATCH.Length > 0)
                {
                    if (response.T_RT_BATCH[0].TYPE != "I" && response.T_RT_BATCH[0].TYPE != "S")
                    {
                        rtnRst.SetValue(false, "Fail");
                        ErrMsg = "Batch Classification Update ";
                        ErrMsg += response.T_RT_BATCH[0].TYPE == "E" ? "Error" : "Warning";
                        ErrMsg += $"\nID : {response.T_RT_BATCH[0].ID}\nNUMBER : {response.T_RT_BATCH[0].NUMBER}\nPARAMETER : {response.T_RT_BATCH[0].PARAMETER}\nMESSAGE : {response.T_RT_BATCH[0].MESSAGE}";
                        throw new System.NullReferenceException(ErrMsg);
                    }
                    else
                    {
                        rtnRst.SetValue(true, "Success");
                    }
                }
                else
                {
                    rtnRst.SetValue(true, "Success");
                    MatDocNo = response.E_DOC_NO;
                    await RefreshOrderQty();
                }
            }
            catch (System.ServiceModel.FaultException faultEx)
            {
                if (faultEx.Message == "TIMETICKET_INPUT_ERROR")
                {
                    ErrMsg = "Time Ticket Input Error";
                }
                else if (faultEx.Message == "ZGLOI_ORDER_NOT_FOUND")
                {
                    ErrMsg = "Previous Order Operation Not Found";
                }
                else if (faultEx.Message == "ZGLOI_ORDER_NOT_RELEASED")
                {
                    ErrMsg = "Previous Order Operation Not Released";
                }
                else if (faultEx.Message == "ZGLOI_OTHER")
                {
                    ErrMsg = "Previous Order Operation Error";
                }
                else if (faultEx.Message == "OD_CF_ER")
                {
                    ErrMsg = "Order Confirmation Error";
                }
                else if (faultEx.Message == "BP_CM_ER")
                {
                    ErrMsg = "BAPI Commit Error";
                }
                else if (faultEx.Message == "GM_CR_ER")
                {
                    ErrMsg = "Goods Movement Create Error";
                }
                else
                {
                    ErrMsg = faultEx.Message;
                }

                rtnRst.SetValue(false, "Fail");
                await ConfigHelper.ShowSmartDialog(
                    "InfoTemplate",
                    ErrMsg,
                    PackIconKind.Error,
                    Brushes.DarkOrange
                );
            }
            catch (Exception ex)
            {
                rtnRst.SetValue(false, "Fail");

                string exType = ex.Message.Substring(0, 1);

                if (exType == "W")
                {
                    await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        ex.Message,
                        PackIconKind.WarningCircle,
                        Brushes.DarkOrange
                    );
                }
                else
                {
                    await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        ex.Message,
                        PackIconKind.Error,
                        Brushes.Crimson
                    );
                }
            }
            finally
            {
                IsBusy = false;
            }
        }



        private async Task RefreshOrderQty()
        {
            IsBusy = true;

            try
            {
                // Define your SAP user credentials
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
                binding.MaxReceivedMessageSize = 2147483647; // Set to 2GB (Max Int32)
                binding.MaxBufferSize = 2147483647;

                // Also update the ReaderQuotas if you have large strings or arrays
                binding.ReaderQuotas.MaxStringContentLength = 2147483647;
                binding.ReaderQuotas.MaxArrayLength = 2147483647;

                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                var endpoint = new EndpointAddress(arraySapEndpoint["z_check_measure_op"]);

                // Set parameter
                var pdoParams = new Z_CHECK_MEASURE_OPRequest
                {
                    I_PD_NO = OrderNo,
                    T_EMP = new PA0001[] { },
                    T_NGRS = new TBICCF[] { },
                    T_PU = new T006A[] { },
                    T_QAG = new TBICCF[] { },
                    T_SL = new T001L[] { },
                    T_WG = new T421T[] { }
                };

                var client = new Z_CHECK_MEASURE_OPClient(binding, endpoint);

                // Set the SAP credentials
                client.ClientCredentials.UserName.UserName = _dataService.SapUser;

                if (sapPass != null)
                {
                    client.ClientCredentials.UserName.Password = sapPass;
                }
                else
                {
                    throw new System.NullReferenceException("Could not get RFC password");
                }

                // Call the web service method
                var response = await client.Z_CHECK_MEASURE_OPAsync(pdoParams);
                // Process the result
                if (response.E_OD_INFO.GMEIN == "0")
                {
                    throw new Exception($"Order No. {OrderNo} not found");
                }
                else
                {
                    Oi = response.E_OD_INFO;

                    OrderQty.Total = Oi.GAMNG;
                    OrderQty.Delivered = Oi.ISM04;
                    OrderQty.Remaining = Oi.ISM05;
                }
            }
            catch (System.ServiceModel.FaultException faultEx)
            {
                if (faultEx.Message == "ORDER_NOT_FOUND")
                {
                    ErrMsg = "Order Not Found";
                }
                else if (faultEx.Message == "OPERATION_NOT_FOUND")
                {
                    ErrMsg = "Operation Not Found";
                }
                else if (faultEx.Message == "ONE_OP")
                {
                    ErrMsg = "Order Has One Operation Only";
                }
                else if (faultEx.Message == "LAST_OP_NOT_MEA")
                {
                    ErrMsg = "Last Operation Not Measure";
                }
                else if (faultEx.Message == "LAST_OP_STAT_CHECK")
                {
                    ErrMsg = "Check Last Operation Order Status";
                }
                else if (faultEx.Message == "PREV_OP_NOT_CF")
                {
                    ErrMsg = "Previous Operation Not Confirm";
                }
                else
                {
                    ErrMsg = faultEx.Message;
                }

                await ConfigHelper.ShowSmartDialog(
                    "InfoTemplate",
                    ErrMsg,
                    PackIconKind.WarningCircle,
                    Brushes.DarkOrange
                );

                ResetData();
            }
            catch (Exception ex)
            {
                await ConfigHelper.ShowSmartDialog(
                    "InfoTemplate",
                    ex.Message,
                    PackIconKind.WarningCircle,
                    Brushes.DarkOrange
                );

                ResetData();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveLocalDataAsync(string sapMatDoc)
        {
            DateTime endDateTime = DateTime.Parse($"{EndDate} {EndTime}");
            var matDoc = new MsMatDoc
            {
                MatDocNo = sapMatDoc ?? string.Empty,
                DocDate = startDateTime.Date,
                PersonnelNo = SelectedPersonnel?.Value ?? string.Empty,
                WageGroup = SelectedWageGroup?.Value ?? string.Empty,
                PUom = SelectedParallelUnit?.Value ?? string.Empty,
                StoreLoc = SelectedStorageLocation?.Value ?? string.Empty,
                Grade = SelectedGrade ?? string.Empty,
                NgReason = SelectedNGReason?.Value ?? string.Empty,
                Packaging = SelectedPackaging?.Value ?? string.Empty,
                StartDate = startDateTime.Date,
                StartTime = startDateTime.TimeOfDay,
                EndDate = endDateTime.Date,
                EndTime = endDateTime.TimeOfDay,
                IsPacking = IsPacking,
                ConfirmNo = CnfNo ?? string.Empty,
                ConfirmCt = CnfCnt ?? string.Empty
            };

            var dataList = Leather.Select(x => new MsData
            {
                Rank = x.Rank,
                Area = x.Area,
                PackingGroup = x.Group,
                PackingRank = x.PackingRank
            }).ToList();

            if (CurrentMsId == 0)
            {
                // FIRST ROUND: Create new Summary
                var summary = new MsSummary
                {
                    OrderNo = OrderNo ?? string.Empty,
                    DocDate = startDateTime.Date,
                    MsTime = startDateTime.TimeOfDay,
                    MatNo = MatNo ?? string.Empty,
                    MatDesc = MatDesc ?? string.Empty,
                    CreatedBy = Environment.UserName,
                    ComputerName = Environment.MachineName,
                    IPAddress = ConfigHelper.GetLocalIPAddress() ?? string.Empty,
                    SAPClient = CurrentClient ?? string.Empty
                };

                await _dataService.SaveToSql(summary, matDoc, dataList);

                // After save, the DB generates an ID. Capture it!
                CurrentMsId = summary.MsId;
            }
            else
            {
                // SUBSEQUENT ROUNDS: Update existing Summary
                await _dataService.SaveToSql(null, matDoc, dataList, CurrentMsId);
            }
        }

        private void LogSqlFailure(string sapMatDoc, Exception ex)
        {
            try
            {
                string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorLogs");
                if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);

                string fileName = $"SQL_Fail_{DateTime.Now:yyyyMMdd_HHmmss}_{sapMatDoc}.txt";
                string filePath = Path.Combine(logDir, fileName);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("--- SQL SAVE FAILURE REPORT ---");
                sb.AppendLine($"Timestamp: {DateTime.Now}");
                sb.AppendLine($"SAP Material Document: {sapMatDoc}");
                sb.AppendLine($"Order No: {this.OrderNo}");
                sb.AppendLine($"Error Message: {ex.Message}");
                sb.AppendLine($"Inner Exception: {ex.InnerException?.Message ?? "None"}");
                sb.AppendLine("-------------------------------");
                sb.AppendLine("DATA SNAPSHOT (JSON):");

                // Serialize the current state so we can re-import it later if needed
                var recoveryData = new RecoverySession {
                    OrderNo = OrderNo ?? string.Empty,
                    MsDate = StartDate ?? string.Empty,
                    MsTime = StartTime ?? string.Empty,
                    PersonnelNo = SelectedPersonnel?.Value ?? string.Empty,
                    WageGroup = SelectedWageGroup?.Value ?? string.Empty,
                    PUom = SelectedParallelUnit?.Value ?? string.Empty,
                    StoreLoc = SelectedStorageLocation?.Value ?? string.Empty,
                    Grade = SelectedGrade ?? string.Empty,
                    NgReason = SelectedNGReason?.Value ?? string.Empty,
                    Packaging = SelectedPackaging?.Value ?? string.Empty,
                    MeasureData = Leather.ToList(),
                    PackingData = PackingLeather.ToList(),
                    LastUpdated = DateTime.Now
                };
                sb.AppendLine(JsonSerializer.Serialize(recoveryData));

                File.WriteAllText(filePath, sb.ToString());
            }
            catch
            {
                // If even the text file fail, we can't do much more than the MessageBox
            }
        }

        private async Task SaveData()
        {
            var result = await ConfigHelper.ShowSmartDialog(
                    "ConfirmTemplate",
                    "Do you want to save this measurement to SAP?",
                    PackIconKind.QuestionMarkCircle,
                    Brushes.Goldenrod,
                    "CONFIRM",
                    "CANCEL"
                );

            if (result)
            {
                await PrepareData();
                await SaveMigo();

                //rtnRst.SetValue(true, "Success");
                //MatDocNo = "5000118889";

                if (rtnRst.GetValue().IsSuccess)
                {
                    CommandManager.InvalidateRequerySuggested();

                    try
                    {
                        await SaveLocalDataAsync(MatDocNo);

                        md.Add(new RSIS_S_RANGE
                        {
                            HIGH = "",
                            LOW = MatDocNo,
                            OPTION = "EQ",
                            SIGN = "I"
                        });

                        await ConfigHelper.ShowSmartDialog(
                            "InfoTemplate",
                            $"Order {Tt[0].ORDERID} confirm succeed\n\nConfirm No. : {CnfNo}\nConfirm Count : {CnfCnt}\n\nMat. Doc. No. : {MatDocNo}",
                            PackIconKind.SuccessCircle,
                            Brushes.ForestGreen
                        );

                        IsStarted = false;
                        //ResetData();
                    }
                    catch (Exception sqlEx)
                    {
                        // CRITICAL: SAP worked, but SQL failed. 
                        // We do NOT call ResetData() here because the user might need to 
                        // manually record the data since it failed to go to the local DB.
                        LogSqlFailure(MatDocNo, sqlEx);

                        await ConfigHelper.ShowSmartDialog(
                            "InfoTemplate",
                            $"SAP Saved ({MatDocNo}), but Local SQL failed: {sqlEx.Message}\n\n" +
                                $"Inner Exception: {sqlEx.InnerException?.Message ?? "None"}\n\n" +
                                "Please capture a screenshot of your screen before closing!",
                            PackIconKind.WarningCircle,
                            Brushes.DarkOrange
                        );
                    }
                    finally
                    {
                        CurrentState = DeviceState.Saved;
                        DeleteAllTempFiles();
                    }
                }
            }
        }

        public Action? RequestFocus { get; set; }
        private bool CheckEnableAdd()
        {
            // Added !IsPaused and !IsBusy for safety
            return CurrentState == DeviceState.StartMeasuring
                   && IsReadyToAdd
                   && !IsPaused
                   && !IsBusy;
        }
        private async void AddItem()
        {
            // Check if we have data to add
            if (string.IsNullOrEmpty(AddMeasureData))
            {
                await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        "Please enter value to add",
                        PackIconKind.WarningCircle,
                        Brushes.DarkOrange
                    );
                RequestFocus?.Invoke();
                return;
            }

            // 2. Check if the user is trying to insert into an old, non-current packing group
            if (SelectedLeather != null && !PackingLeather.Contains(SelectedLeather))
            {
                await ConfigHelper.ShowSmartDialog(
                        "InfoTemplate",
                        "Cannot add data to a finalized packing group. Please select an item from the current packing batch.",
                        PackIconKind.AlertCircle,
                        Brushes.Crimson
                    );
                return;
            }

            var newItem = new LeatherArea
            {
                Area = decimal.Parse(AddMeasureData),
                Group = CurrentGroup,
                IsSelected = false // Ensure new item starts unselected
            };

            if (SelectedLeather != null)
            {
                // 1. Insert into the Main List (Leather)
                int mainIndex = Leather.IndexOf(SelectedLeather);
                Leather.Insert(mainIndex + 1, newItem);

                // 2. Insert into the Packing List (PackingLeather)
                int packingIndex = PackingLeather.IndexOf(SelectedLeather);
                PackingLeather.Insert(packingIndex + 1, newItem);
            }
            else
            {
                // If nothing is selected, just add to the end of both
                Leather.Add(newItem);
                PackingLeather.Add(newItem);
            }

            // Move selection to the new item and scroll
            SelectedLeather = newItem;
            SelectedPackingLeather = newItem;
            IsAllSelected = false;

            foreach (var item in Leather)
            {
                item.IsSelected = false;
            }

            // Recalculate everything
            RefreshListStats();

            // Clear the input box for the next entry
            AddMeasureData = string.Empty;
            IsReadyToAdd = false;
            _isDataDirty = true;
        }

        private bool CheckDataGridItem()
        {
            if (Leather.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckEnablePrintA4()
        {
            // Enable if OrderNo is filled AND (either md has items OR the user filled the Reprint boxes)
            bool hasOrder = !string.IsNullOrWhiteSpace(OrderNo);
            bool hasMatDocs = md.Count > 0;

            return hasOrder && hasMatDocs;
        }

        private bool CheckEnablePrintA5()
        {
            if (CurrentState == DeviceState.Saved && Leather.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckEnableReprint()
        {
            // Enable if OrderNo is filled AND (either md has items OR the user filled the Reprint boxes)
            bool hasOrder = !string.IsNullOrWhiteSpace(ReprintOrderNo);
            bool hasDocYear = !string.IsNullOrWhiteSpace(ReprintDocYear);
            bool hasMatDocs = (!string.IsNullOrWhiteSpace(MatDocFrom) && !string.IsNullOrWhiteSpace(MatDocTo));

            return hasOrder && hasDocYear && hasMatDocs;
        }

        private bool CheckEnableNextPacking()
        {
            //if (_isPackingPrinted && PackingLeather.Count > 0)
            if (PackingLeather.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckEnablePrintPacking()
        {
            if (PackingLeather.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //private bool CheckEnableSave()
        //{
        //    if (_hasSavedSuccessfully)
        //    {
        //        return false;
        //    }

        //    if (_isPacking && PackingLeather.Count > 0)
        //    {
        //        return false;
        //    }

        //    if (_isConnected && !string.IsNullOrWhiteSpace(OrderNo) && Leather.Count > 0 && !_isStarted && !_isStopped)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //private bool CheckEnableStart()
        //{
        //    return !string.IsNullOrWhiteSpace(MatNo) && IsReadyToStart;
        //}

        //private bool CheckEnableStop()
        //{
        //    return IsMeasuring;
        //}

        //private bool CheckEnableStart()
        //{
        //    if (_isConnected && !string.IsNullOrWhiteSpace(MatNo) && _isStarted && !_isStopped)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public void ClearReprintData()
        {
            // Clear the strings bound to the TextBoxes
            MatDocFrom = string.Empty;
            MatDocTo = string.Empty;

            // Clear the actual List used for the SAP T_MAT_DOC parameter
            if (md != null)
            {
                md.Clear();
            }

            // Optional: If you want the OrderNo cleared too
            // OrderNo = string.Empty;
        }

        private DateTime ConvertTimeFormat(DateTime dt)
        {
            int hour = dt.Hour;
            int minute = dt.Minute;
            int second = dt.Second;

            TimeSpan dtFormat = new TimeSpan(
                hour,
                minute,
                second
            );

            return DateTime.Today.Date.Add(dtFormat);
        }

        private async void DeleteSelectedItems()
        {
            var itemsToRemove = Leather.Where(x => x.IsSelected).ToList();
            if (itemsToRemove.Count == 0) return;

            // Check if any selected item is NOT in the current PackingLeather list
            bool hasFinalizedItems = itemsToRemove.Any(item => !PackingLeather.Contains(item));

            if (hasFinalizedItems)
            {
                await ConfigHelper.ShowSmartDialog(
                    "InfoTemplate",
                    "Cannot delete items from a finalized packing group. You can only delete items from the current batch.",
                    PackIconKind.AlertCircle,
                    Brushes.Crimson
                );
                return;
            }

            // Ask for confirmation before deleting data
            var confirm = await ConfigHelper.ShowSmartDialog(
                "ConfirmTemplate",
                $"Are you sure you want to delete {itemsToRemove.Count} items?",
                PackIconKind.DeleteForever,
                Brushes.Crimson,
                "DELETE",
                "CANCEL"
            );

            if (!confirm) return;

            foreach (var item in itemsToRemove)
            {
                Leather.Remove(item);
                PackingLeather.Remove(item);
            }

            RefreshListStats();
            _isDataDirty = true;
        }

        private void DeleteItem()
        {
            // Check if the collection has any items to delete.
            if (Leather.Count > 0)
            {
                TotalArea -= Leather[Leather.Count - 1].Area;
                TxtTotalArea = TotalArea.ToString("#,#.###");

                // Remove the last item from the collection.
                Leather.RemoveAt(Leather.Count - 1);
            }
            else
            {
                TotalArea = 0.000m;
                TxtTotalArea = "0.000";
            }
        }

        private void ExecuteNextPacking()
        {
            if (PackingLeather.Count == 0) return;

            // 1. Check if this group is already in the Batch summary
            var existingBatch = PackingBatch.FirstOrDefault(b => b.Group == CurrentGroup);

            if (existingBatch == null)
            {
                // Only add a NEW item if it doesn't exist yet
                var newItem = new LeatherArea
                {
                    Group = CurrentGroup,
                    Area = PackingTotalArea,
                    Piece = PackingLeather.Count
                };
                PackingBatch.Add(newItem);
            }
            else
            {
                // OPTIONAL: Update the existing one if the user added more pieces 
                // after recovery but before clicking 'Next'
                existingBatch.Area = PackingTotalArea;
                existingBatch.Piece = PackingLeather.Count;

                // If your LeatherArea class doesn't implement INotifyPropertyChanged, 
                // you might need to refresh the UI binding here.
            }

            // 2. Prepare for the next bundle
            CurrentGroup++;
            PackingLeather.Clear();
            PackingTotalArea = 0.000m;
            TxtPackingTotalArea = "0.000";

            // 3. Mark data as dirty so the recovery file updates
            _isDataDirty = true;

            // Note: 'Leather' is NOT cleared. It keeps everything.
            RefreshListStats();

            //_isPackingPrinted = false;
            IsReadyToAdd = false;
        }

        private async Task GetRoutingActivity(string rt_aufpl, string op_vornr, decimal yield)
        {
            try
            {
                // Define your SAP user credentials
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
                binding.MaxReceivedMessageSize = 2147483647; // Set to 2GB (Max Int32)
                binding.MaxBufferSize = 2147483647;

                // Also update the ReaderQuotas if you have large strings or arrays
                binding.ReaderQuotas.MaxStringContentLength = 2147483647;
                binding.ReaderQuotas.MaxArrayLength = 2147483647;


                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                var endpoint = new EndpointAddress(arraySapEndpoint["z_ws_cal_activities"]);
                var client = new Z_WS_CAL_ACTIVITIESClient(binding, endpoint);

                // Set the SAP credentials
                client.ClientCredentials.UserName.UserName = _dataService.SapUser;

                if (sapPass != null)
                {
                    client.ClientCredentials.UserName.Password = sapPass;
                }
                else
                {
                    throw new System.NullReferenceException("Could not get RFC password");
                }

                // Call the web service method
                var response = await client.Z_WS_CAL_ACTIVITIESAsync(rt_aufpl, op_vornr, yield);
                // Process the result
                if (response.ET_ROUTING_ACTIVITY.LTXA1 == "")
                {
                    throw new Exception($"Routing {rt_aufpl} Activity {op_vornr} not found");
                }
                else
                {
                    Tt[0].CONF_ACTIVITY1 = response.ET_ROUTING_ACTIVITY.ACT1;
                    Tt[0].CONF_ACTIVITY2 = response.ET_ROUTING_ACTIVITY.ACT2;
                    Tt[0].CONF_ACTIVITY3 = response.ET_ROUTING_ACTIVITY.ACT3;
                }
            }
            catch (Exception ex)
            {
                await ConfigHelper.ShowSmartDialog(
                    "InfoTemplate",
                    ex.Message,
                    PackIconKind.WarningCircle,
                    Brushes.DarkOrange
                );

                ResetData();
            }
        }

        private void MapTtValue(SapChkMsOp.BAPI_PP_TIMETICKET Pp_Tt)
        {
            Tt.Clear();
            Tt.Add(new BAPI_PP_TIMETICKET
                {
                    CONF_QUAN_UNIT = Pp_Tt.CONF_QUAN_UNIT,
                    ORDERID        = Pp_Tt.ORDERID,
                    OPERATION      = Pp_Tt.OPERATION,
                    PLANT          = Pp_Tt.PLANT,
                    WORK_CNTR      = Pp_Tt.WORK_CNTR
                }
            );
                
        }

        private async Task PrintA4Form()
        {
            if (IsBusy) return; // Prevent double-execution
            IsBusy = true;
            string SelectOrderNo = OrderNo;
            string SelectDocYear = DocYear;
            var SelectMatDoc = new ObservableCollection<RSIS_S_RANGE>(md.Select(item => new RSIS_S_RANGE
            {
                HIGH = item.HIGH,
                LOW = item.LOW,
                OPTION = item.OPTION,
                SIGN = item.SIGN
            }).ToList());

            if (!string.IsNullOrWhiteSpace(MatDocFrom) && !string.IsNullOrWhiteSpace(MatDocTo))
            {
                SelectOrderNo = ReprintOrderNo;
                SelectDocYear = ReprintDocYear;
                SelectMatDoc.Clear();

                SelectMatDoc.Add(new RSIS_S_RANGE
                {
                    HIGH = MatDocTo,
                    LOW = MatDocFrom,
                    OPTION = "BT",
                    SIGN = "I"
                });
            }
            
            try
            {
                // Define your SAP user credentials
                var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
                binding.MaxReceivedMessageSize = 2147483647; // Set to 2GB (Max Int32)
                binding.MaxBufferSize = 2147483647;

                // Also update the ReaderQuotas if you have large strings or arrays
                binding.ReaderQuotas.MaxStringContentLength = 2147483647;
                binding.ReaderQuotas.MaxArrayLength = 2147483647;

                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                var endpoint = new EndpointAddress(arraySapEndpoint["z_get_order_grade_total"]);
                var pdoParams = new Z_GET_ORDER_GRADE_TOTALRequest
                {
                    I_ORDER_NO = SelectOrderNo,
                    I_DOC_YEAR = SelectDocYear,
                    T_MAT_DOC = SelectMatDoc.ToArray(),
                    T_GR_QTY = new MSEG[] { }
                };
                var client = new Z_GET_ORDER_GRADE_TOTALClient(binding, endpoint);

                client.ClientCredentials.UserName.UserName = _dataService.SapUser;

                if (sapPass != null)
                {
                    client.ClientCredentials.UserName.Password = sapPass;
                }
                else
                {
                    throw new System.NullReferenceException("Could not get RFC password");
                }

                var response = await client.Z_GET_ORDER_GRADE_TOTALAsync(pdoParams);

                if (response.E_RET.TYPE == "E")
                {
                    throw new Exception(response.E_RET.MESSAGE);
                }
                else
                {
                    GlobalFontSettings.FontResolver = new PDF.FailsafeErrorResolver();

                    string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                    string rawArt = await _dataService.GetCustomerColorArticle(response.E_MAT_NO);
                    string printA4ColorArticle = string.IsNullOrEmpty(rawArt) ? "" : $"/{rawArt}";
                    var docGenerator = new MachineSerialPort.PDF.DocumentsA4();
                    var descParts = new[] { response.E_MAT_DESC, printA4ColorArticle }
                        .Where(s => !string.IsNullOrWhiteSpace(s));
                    DocParameter para = new DocParameter
                    {
                        DocDate = response.E_DOC_DATE,
                        MatDesc = string.Join("/", descParts),
                        MatNo = response.E_MAT_NO,
                        OrderNo = OrderNo,
                        PQty = response.E_P_QTY.ToString("#,#"),
                        Qty = response.E_QTY.ToString("#,##0.000"),
                        TotalGradeQty = response.T_GR_QTY
                    };
                    docGenerator.SetData(para);

                    var document = docGenerator.CreateDocument();

                    // Write MigraDoc DDL to a file.
                    MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToFile(document, "MigraDoc.mdddl");

                    // Create a renderer for the MigraDoc document.
                    var pdfRenderer = new PdfDocumentRenderer
                    {
                        // Associate the MigraDoc document with a renderer.
                        Document = document,
                        PdfDocument =
                {
                    // Change some settings before rendering the MigraDoc document.
                    PageLayout = PdfPageLayout.SinglePage,
                    ViewerPreferences =
                    {
                        FitWindow = true
                    }
                }
                    };

                    // Layout and render document to PDF.
                    pdfRenderer.RenderDocument();

                    // Save the document...
                    var filename = PdfFileUtility.GetTempPdfFullFileName($"ใบสรุปยอดฟุตทั้ง Order {OrderNo}.pdf");
                    pdfRenderer.PdfDocument.Save(filename);

                    // ...and start a viewer.
                    PdfFileUtility.ShowDocument(filename);
                }
            }
            catch (Exception ex)
            {
                await ConfigHelper.ShowSmartDialog(
                    "InfoTemplate",
                    ex.Message,
                    PackIconKind.Error,
                    Brushes.Crimson
                );
            }
            finally
            {
                SelectMatDoc.Clear();
                IsBusy = false;
            }
        }

        private async void PrintA5Form()
        {
            GlobalFontSettings.FontResolver = new PDF.FailsafeErrorResolver();

            string CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            var docGenerator = new MachineSerialPort.PDF.DocumentsA5();
            List<DocParameter> paras = new List<DocParameter> { };

            if (!string.IsNullOrWhiteSpace(MatDocFrom) && !string.IsNullOrWhiteSpace(MatDocTo))
            {
                paras = await _dataService.GetLeatherMeasureByMatDoc(ReprintOrderNo, ReprintDocYear, MatDocFrom, MatDocTo);
            }
            else
            {
                var descParts = new[] { MatDesc, ColorArticle }
                    .Where(s => !string.IsNullOrWhiteSpace(s));
                DocParameter para = new DocParameter
                {
                    MeasureData = Leather,
                    DocDate = CurrentDate,
                    Grade = SelectedGrade ?? string.Empty,
                    NGReason = SelectedNGReason?.Value ?? string.Empty,
                    MatDesc = string.Join("/", descParts) ?? string.Empty,
                    MatNo = MatNo ?? string.Empty,
                    OrderNo = OrderNo ?? string.Empty,
                    TotalQty = TxtTotalArea ?? string.Empty
                };

                paras.Add(para);
            }

            //docGenerator.SetData(paras);

            var document = docGenerator.CreateDocument(paras);

            // Write MigraDoc DDL to a file.
            MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToFile(document, "MigraDoc.mdddl");

            // Create a renderer for the MigraDoc document.
            var pdfRenderer = new PdfDocumentRenderer
            {
                // Associate the MigraDoc document with a renderer.
                Document = document,
                PdfDocument =
                {
                    // Change some settings before rendering the MigraDoc document.
                    PageLayout = PdfPageLayout.SinglePage,
                    ViewerPreferences =
                    {
                        FitWindow = true
                    }
                }
            };

            // Layout and render document to PDF.
            pdfRenderer.RenderDocument();

            // Save the document...
            var filename = PdfFileUtility.GetTempPdfFullFileName($"ใบรายละเอียดยอดฟุต {OrderNo} Grade {SelectedGrade}.pdf");
            pdfRenderer.PdfDocument.Save(filename);

            // ...and start a viewer.
            PdfFileUtility.ShowDocument(filename);
        }

        private void PrintPackingForm()
        {
            GlobalFontSettings.FontResolver = new PDF.FailsafeErrorResolver();

            string CurrentDate = DateTime.Now.ToString("yyyy-MM-dd");
            var docGenerator = new MachineSerialPort.PDF.DocumentsPacking();
            DocParameter para = new DocParameter
            {
                MeasureData = PackingLeather,
                DocDate = CurrentDate,
                Grade = SelectedGrade ?? string.Empty,
                NGReason = SelectedNGReason?.Value ?? string.Empty,
                MatDesc = MatDesc ?? string.Empty,
                MatNo = MatNo ?? string.Empty,
                OrderNo = OrderNo ?? string.Empty,
                TotalQty = TxtPackingTotalArea ?? string.Empty,
                PackingGroup = CurrentGroup
            };
            docGenerator.SetData(para);

            var document = docGenerator.CreateDocument();

            // Write MigraDoc DDL to a file.
            MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToFile(document, "MigraDoc.mdddl");

            // Create a renderer for the MigraDoc document.
            var pdfRenderer = new PdfDocumentRenderer
            {
                // Associate the MigraDoc document with a renderer.
                Document = document,
                PdfDocument =
                {
                    // Change some settings before rendering the MigraDoc document.
                    PageLayout = PdfPageLayout.SinglePage,
                    ViewerPreferences =
                    {
                        FitWindow = true
                    }
                }
            };

            // Layout and render document to PDF.
            pdfRenderer.RenderDocument();

            // Save the document...
            var filename = PdfFileUtility.GetTempPdfFullFileName($"ใบรายละเอียดยอดฟุต {OrderNo} Grade {SelectedGrade} - {CurrentGroup}.pdf");
            pdfRenderer.PdfDocument.Save(filename);

            // ...and start a viewer.
            PdfFileUtility.ShowDocument(filename);

            //_isPackingPrinted = true;
        }

        private void RefreshListStats()
        {
            // 1. Calculate Main List Stats
            decimal totalGlobal = 0;
            for (int i = 0; i < Leather.Count; i++)
            {
                Leather[i].Rank = i + 1;
                totalGlobal += Leather[i].Area;
            }
            TotalArea = totalGlobal;
            TxtTotalArea = totalGlobal.ToString("#,##0.000");

            // 2. Calculate Packing List Stats (ONLY the items in the second grid)
            decimal totalPacking = 0;
            for (int i = 0; i < PackingLeather.Count; i++)
            {
                // This ensures the Rank in the Packing Grid always starts at 1
                PackingLeather[i].PackingRank = i + 1;
                totalPacking += PackingLeather[i].Area;
            }
            // Update the separate packing total variable
            PackingTotalArea = totalPacking;
            TxtPackingTotalArea = totalPacking.ToString("#,##0.000");
        }

        //private async Task SaveData()
        //{
        //    await PrepareData();
        //    await SaveMigo();
        //    if (returnResult == "Success")
        //    {
        //        _hasSavedSuccessfully = true;
        //        _isStarted = true;
        //        IsStarted = true;
        //        CommandManager.InvalidateRequerySuggested();

        //        md.Add(new RSIS_S_RANGE
        //        {
        //            HIGH = "",
        //            LOW = MatDocNo,
        //            OPTION = "EQ",
        //            SIGN = "I"
        //        });

        //        MessageBoxResult result = MessageBox.Show(
        //                $"Order {Tt[0].ORDERID} confirm succeed\n\nConfirm No. : {CnfNo}\nConfirm Count : {CnfCnt}\n\nMat. Doc. No. : {MatDocNo}",
        //                //$"Order {Tt[0].ORDERID} confirm succeed\n\nConfirm No. : {CnfNo}\nConfirm Count : {CnfCnt}",
        //                "Success",
        //                MessageBoxButton.OK,
        //                MessageBoxImage.Information
        //            );

        //        //ResetData();
        //    }
        //}

        //private async Task SaveTimeTicket()
        //{
        //    try
        //    {
        //        var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
        //        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
        //        //var endpoint = new EndpointAddress("http://IHL-DEV01.interhides.com:8000/sap/bc/srt/rfc/sap/z_prodorconf_create/900/z_prodorconf_create/z_prodorconf_create");
        //        var endpoint = new EndpointAddress("http://IHL-DEV01.interhides.com:8000/sap/bc/srt/rfc/sap/z_prodorconf_create_bk/900/z_prodorconf_create_bk/z_prodorconf_create_bk");
        //        //var client = new Z_PRODORCONF_CREATEClient(binding, endpoint);
        //        var client = new Z_PRODORCONF_CREATE_BKClient(binding, endpoint);

        //        client.ClientCredentials.UserName.UserName = sapUser;

        //        if (sapPass != null)
        //        {
        //            client.ClientCredentials.UserName.Password = sapPass;
        //        }
        //        else
        //        {
        //            throw new System.NullReferenceException("Could not get RFC password");
        //        }

        //        //var pm = new Z_PRODORCONF_CREATERequest
        //        var pm = new Z_PRODORCONF_CREATE_BKRequest
        //        {
        //            IV_GR_REQUIRED = "X",
        //            DETAIL_RETURN  = new BAPI_CORU_RETURN[] { },
        //            ET_RETURN      = new BAPIRET1[] { },
        //            GOODSMOVEMENTS = Gm.ToArray(),
        //            TIMETICKET     = Tt.ToArray()
        //        };

        //        //var response = await client.Z_PRODORCONF_CREATEAsync(pm);
        //        var response = await client.Z_PRODORCONF_CREATE_BKAsync(pm);

        //        if (response.DETAIL_RETURN[0].TYPE != "I" && response.DETAIL_RETURN[0].TYPE != "S")
        //        {
        //            returnResult = "Fail";
        //            ErrMsg = response.DETAIL_RETURN[0].TYPE == "E" ? "Error" : "Warning";
        //            ErrMsg += $"\nID : {response.DETAIL_RETURN[0].ID}\nNUMBER : {response.DETAIL_RETURN[0].NUMBER}\nPARAMETER : {response.DETAIL_RETURN[0].PARAMETER}\nMESSAGE : {response.DETAIL_RETURN[0].MESSAGE}";
        //            throw new System.NullReferenceException(ErrMsg);
        //        }
        //        else if (response.ET_RETURN[0].TYPE != "I" && response.ET_RETURN[0].TYPE != "S")
        //        {
        //            returnResult = "Fail";
        //            ErrMsg = response.ET_RETURN[0].TYPE == "E" ? "Error" : "Warning";
        //            ErrMsg += $"\nID : {response.ET_RETURN[0].ID}\nNUMBER : {response.ET_RETURN[0].NUMBER}\nMESSAGE : {response.ET_RETURN[0].MESSAGE}";
        //            throw new System.NullReferenceException(ErrMsg);
        //        }
        //        else
        //        {
        //            returnResult = "Success";
        //            CnfNo = response.DETAIL_RETURN[0].CONF_NO;
        //            CnfCnt = response.DETAIL_RETURN[0].CONF_CNT;
        //        }
        //    }
        //    catch (System.ServiceModel.FaultException faultEx)
        //    {
        //        if (faultEx.Message == "CONFIRM_ERROR")
        //        {
        //            ErrMsg = "Confirm error";
        //        }
        //        else if (faultEx.Message == "TIMETICKET_INPUT_ERROR")
        //        {
        //            ErrMsg = "TimeTicket Input error";
        //        }
        //        else
        //        {
        //            ErrMsg = faultEx.Message;
        //        }

        //        MessageBoxResult result = MessageBox.Show(
        //            ErrMsg,
        //            "Error",
        //            MessageBoxButton.OK,
        //            MessageBoxImage.Error
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        string exType = ex.Message.Substring(0, 1);

        //        if (exType == "W")
        //        {
        //            MessageBoxResult result = MessageBox.Show(
        //                ex.Message,
        //                "Warning",
        //                MessageBoxButton.OK,
        //                MessageBoxImage.Warning
        //            );
        //        }
        //        else
        //        {
        //            MessageBoxResult result = MessageBox.Show(
        //                ex.Message,
        //                "Error",
        //                MessageBoxButton.OK,
        //                MessageBoxImage.Error
        //            );
        //        }
        //    }
        //}

        //private void AddEndpoint(string host, string client, string serviceName)
        //{
        //    string url = $"http://{host}.interhides.com:8000/sap/bc/srt/rfc/sap/{serviceName}/{client}/{serviceName}/{serviceName}";
        //    arraySapEndpoint[serviceName] = url;
        //}

        //// Then your method becomes very clean:
        //public void SetSapEndpoint(string sapClient)
        //{
        //    string host = (sapClient == "PRD") ? "IHL-PRD02" : "IHL-QAS01";
        //    string client = (sapClient == "PRD") ? "900" : "500";

        //    AddEndpoint(host, client, "z_check_measure_op");
        //    AddEndpoint(host, client, "z_ws_cal_activities");
        //    AddEndpoint(host, client, "z_get_order_grade_total");
        //    AddEndpoint(host, client, "z_bapi_migo");
        //}

        //public void SetSapEndpoint(string sapClient)
        //{
        //    if (sapClient == "QAS")
        //    {
        //        arraySapEndpoint.Add("z_check_measure_op", "http://IHL-QAS01.interhides.com:8000/sap/bc/srt/rfc/sap/z_check_measure_op/500/z_check_measure_op/z_check_measure_op");
        //        arraySapEndpoint.Add("z_ws_cal_activities", "http://IHL-QAS01.interhides.com:8000/sap/bc/srt/rfc/sap/z_ws_cal_activities/500/z_ws_cal_activities/z_ws_cal_activities");
        //        arraySapEndpoint.Add("z_get_order_grade_total", "http://IHL-QAS01.interhides.com:8000/sap/bc/srt/rfc/sap/z_get_order_grade_total/500/z_get_order_grade_total/z_get_order_grade_total");
        //        arraySapEndpoint.Add("z_bapi_migo", "http://IHL-QAS01.interhides.com:8000/sap/bc/srt/rfc/sap/z_bapi_migo/500/z_bapi_migo/z_bapi_migo");
        //    }
        //    else if (sapClient == "PRD")
        //    {
        //        arraySapEndpoint.Add("z_check_measure_op", "http://IHL-PRD02.interhides.com:8000/sap/bc/srt/rfc/sap/z_check_measure_op/900/z_check_measure_op/z_check_measure_op");
        //        arraySapEndpoint.Add("z_ws_cal_activities", "http://IHL-PRD02.interhides.com:8000/sap/bc/srt/rfc/sap/z_ws_cal_activities/900/z_ws_cal_activities/z_ws_cal_activities");
        //        arraySapEndpoint.Add("z_get_order_grade_total", "http://IHL-PRD02.interhides.com:8000/sap/bc/srt/rfc/sap/z_get_order_grade_total/900/z_get_order_grade_total/z_get_order_grade_total");
        //        arraySapEndpoint.Add("z_bapi_migo", "http://IHL-PRD02.interhides.com:8000/sap/bc/srt/rfc/sap/z_bapi_migo/900/z_bapi_migo/z_bapi_migo");
        //    }
        //}

        //private void StartMeasure()
        //{
        //    if (SelectedGrade == "NG" && SelectedNGReason.Value == "")
        //    {
        //        MessageBox.Show("Please selected NG Reason", "Warning");
        //    }
        //    else
        //    {
        //        DateTime startDateTime = DateTime.Now;
        //        StartDate = startDateTime.ToString("yyyy-MM-dd");
        //        StartTime = startDateTime.ToString("HH:mm:ss");
        //        _hasSavedSuccessfully = false;
        //        _isStarted = false;
        //        IsStarted = false;
        //        _isStopped = true;
        //        Leather.Clear();
        //        TotalArea = 0.000m;
        //    }
        //}

        //private void StopMeasure()
        //{
        //    DateTime endDateTime = DateTime.Now;
        //    EndDate    = endDateTime.ToString("yyyy-MM-dd");
        //    EndTime    = endDateTime.ToString("HH:mm:ss");
        //    //_isStarted = true;
        //    _isStopped = false;
        //}

        //private void ToggleConnection()
        //{
        //    if (_isConnected)
        //    {
        //        DisconnectSerialPort();
        //    }
        //    else
        //    {
        //        ConnectSerialPort();
        //    }
        //}

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            // If the value is the same as the current field, don't do anything
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            // Set the new value
            field = value;

            // "Shout" to the UI that it changed
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
