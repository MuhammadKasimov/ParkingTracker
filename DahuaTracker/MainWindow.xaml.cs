using DahuaTracker.Data.IRepositories;
using DahuaTracker.Data.Repositories;
using DahuaTracker.Enums;
using DahuaTracker.Pages;
using NetSDKCS;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DahuaTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static fDisConnectCallBack m_DisConnectCallBack;
        private static fHaveReConnectCallBack m_ReConnectCallBack;

        private static fAnalyzerDataCallBack m_AnalyzerDataCallBack;
        private static fAnalyzerDataCallBack m_LeavedAnalyzerDataCallBack;
        private const int m_WaitTime = 5000;
        private const int ListViewCount = 100;
        HanldeExitPage exitPage;
        private IntPtr m_LoginID = IntPtr.Zero;
        private IntPtr m_LeavedLoginID = IntPtr.Zero;
        private NET_DEVICEINFO_Ex m_DeviceInfo;
        private NET_DEVICEINFO_Ex m_LeavedDeviceInfo;
        private Int64 m_ID = 1;
        private IntPtr m_RealPlayID = IntPtr.Zero;
        private IntPtr m_LeavedRealPlayID = IntPtr.Zero;
        private IntPtr m_EventID = IntPtr.Zero;
        private IntPtr m_LeavedEventID = IntPtr.Zero;
        SearchPage searchPage;
        CultureInfo cultureInfo;

        private DispatcherTimer checkDate;

        private ObservableCollection<InfoView> dataItems;
        private ObservableCollection<InfoView> dataLeavedItems;
        private readonly CollectionViewSource collectionViewSource;
        private readonly CollectionViewSource collectionLeavedViewSource;

        private readonly IGenericRepository<EventInfo> genericRepository;
        private readonly IGenericRepository<CameraCredentials> settingsRepository;
        public MainWindow()
        {
            InitializeComponent();
            dataItems = new ObservableCollection<InfoView>();
            dataLeavedItems = new ObservableCollection<InfoView>();
            collectionViewSource = new CollectionViewSource();
            collectionLeavedViewSource = new CollectionViewSource();
            collectionViewSource.Source = dataItems;
            collectionLeavedViewSource.Source = dataLeavedItems;
            cultureInfo = CultureInfo.InvariantCulture;

            EnterDataGrid.ItemsSource = dataItems;
            LeaveDataGrid.ItemsSource = dataLeavedItems;
            genericRepository = new GenericRepository<EventInfo>();
            settingsRepository = new GenericRepository<CameraCredentials>();
            checkDate = new DispatcherTimer();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var data = genericRepository.GetAll(ei =>
                ei.Time.Date == DateTime.Now.Date && ei.Mode == Mode.Kirganlar)
                .OrderByDescending(ei => ei.Time)
                .ToList();

            int enteredCars = data.Count;
            EnteredCarsTxt.Text = enteredCars.ToString();
            var dataLeaved = genericRepository.GetAll(ei =>
                ei.Time.Date == DateTime.Now.Date && ei.Mode == Mode.Chiqqanlar)
                .OrderByDescending(ei => ei.Time)
                .ToList();
            LeavedCarsTxt.Text = dataLeaved.Count.ToString();

            foreach (var item in data)
            {
                InfoView infoView = new InfoView()
                {
                    FileName = item.FileName,
                    PlateColor = item.PlateColor,
                    PlateNumber = item.PlateNumber,
                    VehicleColor = item.VehicleColor,
                    VehicleSize = item.VehicleSize,
                    VehicleType = item.VehicleType,
                    Mode = item.Mode,
                    Time = item.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                    Date = DateTime.Now
                };

                dataItems.Add(infoView);
            }

            foreach (var item in dataLeaved)
            {
                InfoView infoView = new InfoView()
                {
                    FileName = item.FileName,
                    PlateColor = item.PlateColor,
                    PlateNumber = item.PlateNumber,
                    VehicleColor = item.VehicleColor,
                    VehicleSize = item.VehicleSize,
                    VehicleType = item.VehicleType,
                    Time = item.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                    Date = DateTime.Now
                };

                dataLeavedItems.Add(infoView);
            }
            m_DisConnectCallBack = new fDisConnectCallBack(DisConnectCallBack);
            m_ReConnectCallBack = new fHaveReConnectCallBack(ReConnectCallBack);
            m_AnalyzerDataCallBack = new fAnalyzerDataCallBack(AnalyzerDataCallBack);
            m_LeavedAnalyzerDataCallBack = new fAnalyzerDataCallBack(LeavedAnalyzerDataCallBack);
            try
            {
                NETClient.Init(m_DisConnectCallBack, IntPtr.Zero, null);
                NETClient.SetAutoReconnect(m_ReConnectCallBack, IntPtr.Zero);
            }
            catch
            {
                Process.GetCurrentProcess().Kill();
            }
            checkDate.Tick += CheckDate;
            checkDate.Start();
            checkDate.Interval = TimeSpan.FromSeconds(20);

            var entereCredentials = await settingsRepository.GetAsync(s => s.Mode == Mode.Kirganlar);
            var leaveCredentials = await settingsRepository.GetAsync(s => s.Mode == Mode.Chiqqanlar);

            if (entereCredentials is not null && leaveCredentials is not null)
            {
                LoginBtn_Click(entereCredentials);
                LoginLeaveBtn_Click(leaveCredentials);
            }
        }

        private void CheckDate(object sender, EventArgs e)
        {
            var data = dataItems.FirstOrDefault();
            if (data != null && data.Date.Date != DateTime.Now.Date)
            {
                dataLeavedItems.Clear();
                dataItems.Clear();
            }
        }

        private void SubscribeEvent()
        {
            if (IntPtr.Zero == m_EventID)
            {
                m_ID = 1;
                m_EventID = NETClient.RealLoadPicture(m_LoginID, 0, (uint)EM_EVENT_IVS_TYPE.ALL, true, m_AnalyzerDataCallBack, m_LoginID, IntPtr.Zero);
                if (IntPtr.Zero == m_EventID)
                {
                    System.Windows.MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
            }
            else
            {
                bool ret = NETClient.StopLoadPic(m_EventID);
                if (!ret)
                {
                    System.Windows.MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                m_EventID = IntPtr.Zero;
                EnterRealTimeCam.Image = null;
                EnterRealTimeCam.Refresh();
            }
        }
        private void SubscribeLeavedEvent()
        {
            if (IntPtr.Zero == m_LeavedEventID)
            {
                m_ID = 1;
                m_LeavedEventID = NETClient.RealLoadPicture(m_LeavedLoginID, 0, (uint)EM_EVENT_IVS_TYPE.ALL, true, m_LeavedAnalyzerDataCallBack, m_LeavedLoginID, IntPtr.Zero);
                if (IntPtr.Zero == m_LeavedEventID)
                {
                    System.Windows.MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
            }
            else
            {
                bool ret = NETClient.StopLoadPic(m_LeavedEventID);
                if (!ret)
                {
                    System.Windows.MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                m_EventID = IntPtr.Zero;
                LeaveRealTimeCam.Image = null;
                LeaveRealTimeCam.Refresh();
            }
        }

        private void LoginBtn_Click(CameraCredentials credentials)
        {
            if (IntPtr.Zero == m_LoginID)
            {
                ushort port = 0;
                try
                {
                    port = Convert.ToUInt16(credentials.Port.Trim());
                }
                catch
                {
                    System.Windows.MessageBox.Show("Input port error");
                    return;
                }
                m_DeviceInfo = new NET_DEVICEINFO_Ex();
                m_LoginID = NETClient.LoginWithHighLevelSecurity(credentials.Ip,
                    port, credentials.Login, credentials.Password,
                    EM_LOGIN_SPAC_CAP_TYPE.TCP, IntPtr.Zero, ref m_DeviceInfo);

                if (m_DeviceInfo.sSerialNumber != "8K0B19BPAJ00005" && m_DeviceInfo.sSerialNumber != "8K0B19BPAJ00009")
                {
                    System.Windows.MessageBox.Show("Seriya raqami xato");
                    return;
                }
                if (IntPtr.Zero == m_LoginID)
                {
                    System.Windows.MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                StartRealPlay();
                SubscribeEvent();
            }
            else
            {
                bool result = NETClient.Logout(m_LoginID);
                if (!result)
                {
                    System.Windows.MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                m_LoginID = IntPtr.Zero;
            }
        }

        private void LoginLeaveBtn_Click(CameraCredentials credentials)
        {
            if (IntPtr.Zero == m_LeavedLoginID)
            {
                ushort port;
                try
                {
                    port = Convert.ToUInt16(credentials.Port.Trim());
                }
                catch
                {
                    System.Windows.MessageBox.Show("Input port error");
                    return;
                }
                m_LeavedDeviceInfo = new NET_DEVICEINFO_Ex();

                m_LeavedLoginID = NETClient.LoginWithHighLevelSecurity(credentials.Ip,
                port, credentials.Login, credentials.Password,
                    EM_LOGIN_SPAC_CAP_TYPE.TCP, IntPtr.Zero, ref m_LeavedDeviceInfo);
                if (m_DeviceInfo.sSerialNumber != "8K0B19BPAJ00005" && m_DeviceInfo.sSerialNumber != "8K0B19BPAJ00009")
                {
                    System.Windows.MessageBox.Show("Seriya raqami xato");
                    return;
                }
                if (IntPtr.Zero == m_LeavedLoginID)
                {
                    System.Windows.MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                StartRealPlayLeaved();
                SubscribeLeavedEvent();
            }
            else
            {
                bool result = NETClient.Logout(m_LeavedLoginID);
                if (!result)
                {
                    System.Windows.MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                m_LeavedLoginID = IntPtr.Zero;
            }
        }

        private void StartRealPlay()
        {
            if (IntPtr.Zero == m_RealPlayID)
            {
                m_RealPlayID = NETClient.RealPlay(m_LoginID, 0, EnterRealTimeCam.Handle);
                if (IntPtr.Zero == m_RealPlayID)
                {
                    System.Windows.MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
            }
            else
            {
                bool ret = NETClient.StopRealPlay(m_RealPlayID);
                if (!ret)
                {
                    System.Windows.MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                m_RealPlayID = IntPtr.Zero;
                EnterRealTimeCam.Refresh();
            }
        }
        private void StartRealPlayLeaved()
        {
            if (IntPtr.Zero == m_LeavedRealPlayID)
            {
                m_LeavedRealPlayID = NETClient.RealPlay(m_LeavedLoginID, 0, LeaveRealTimeCam.Handle); ;
                if (IntPtr.Zero == m_LeavedRealPlayID)
                {
                    System.Windows.MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
            }
            else
            {
                bool ret = NETClient.StopRealPlay(m_LeavedRealPlayID);
                if (!ret)
                {
                    System.Windows.MessageBox.Show(this, NETClient.GetLastError());
                    return;
                }
                m_LeavedRealPlayID = IntPtr.Zero;
                EnterRealTimeCam.Refresh();
            }
        }

        private void DisConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            UpdateDisConnectUI();
        }

        private void UpdateDisConnectUI()
        {
        }

        private void ReConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            UpdateReConnectUI();
        }
        private void UpdateReConnectUI()
        {
        }
        private string SavePicture(byte[] buffer)
        {
            string path = System.IO.Path.Combine(Environment.SpecialFolder.ApplicationData + "DahuaSnaps");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileName = Guid.NewGuid() + ".jpg";
            string filePath = path + "\\" + fileName;
            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                fileStream.Write(buffer, 0, buffer.Length);
                fileStream.Flush();
                fileStream.Dispose();
            }

            return filePath;
        }

        private string GetVehicleSize(EM_VehicleSizeType nVehicleSize)
        {
            string result = "UnKnow";
            switch (nVehicleSize)
            {
                case EM_VehicleSizeType.UnKnow:
                    break;
                case EM_VehicleSizeType.Light_Duty:
                    result = "Light-duty";
                    break;
                case EM_VehicleSizeType.Medium:
                    result = "Medium(中型车)";
                    break;
                case EM_VehicleSizeType.Oversize:
                    result = "Oversize";
                    break;
                case EM_VehicleSizeType.Minisize:
                    result = "Minisize";
                    break;
                case EM_VehicleSizeType.Largesize:
                    result = "Largesize";
                    break;
                default:
                    break;
            }
            return result;
        }

        private int AnalyzerDataCallBack(IntPtr lAnalyzerHandle, uint dwEventType, IntPtr pEventInfo, IntPtr pBuffer, uint dwBufSize, IntPtr dwUser, int nSequence, IntPtr reserved)
        {
            NET_A_DEV_EVENT_TRAFFICJUNCTION_INFO info = (NET_A_DEV_EVENT_TRAFFICJUNCTION_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_A_DEV_EVENT_TRAFFICJUNCTION_INFO));

            EventInfo eventInfo = new EventInfo();
            eventInfo.Time = DateTime.UtcNow + TimeSpan.FromHours(5);
            eventInfo.Index = info.stuFileInfo.bIndex.ToString();
            eventInfo.Count = info.stuFileInfo.bCount.ToString();
            eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
            eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
            eventInfo.PlateType = info.stTrafficCar.szPlateType;
            eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
            eventInfo.VehicleSize = GetVehicleSize((EM_VehicleSizeType)info.stTrafficCar.nVehicleSize);
            eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
            eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
            eventInfo.Mode = Mode.Kirganlar;
            if (IntPtr.Zero != pBuffer && dwBufSize > 0)
            {
                byte[] buffer = new byte[dwBufSize];
                Marshal.Copy(pBuffer, buffer, 0, (int)dwBufSize);
                eventInfo.FileName = SavePicture(buffer);
            }


            UpdateUI(eventInfo);
            m_ID++;
            return 0;
        }
        private int LeavedAnalyzerDataCallBack(IntPtr lAnalyzerHandle, uint dwEventType, IntPtr pEventInfo, IntPtr pBuffer, uint dwBufSize, IntPtr dwUser, int nSequence, IntPtr reserved)
        {
            NET_A_DEV_EVENT_TRAFFICJUNCTION_INFO info = (NET_A_DEV_EVENT_TRAFFICJUNCTION_INFO)Marshal.PtrToStructure(pEventInfo, typeof(NET_A_DEV_EVENT_TRAFFICJUNCTION_INFO));

            EventInfo eventInfo = new EventInfo();
            eventInfo.Time = DateTime.UtcNow + TimeSpan.FromHours(5);
            eventInfo.Index = info.stuFileInfo.bIndex.ToString();
            eventInfo.Count = info.stuFileInfo.bCount.ToString();
            eventInfo.PlateNumber = info.stTrafficCar.szPlateNumber;
            eventInfo.PlateColor = info.stTrafficCar.szPlateColor;
            eventInfo.PlateType = info.stTrafficCar.szPlateType;
            eventInfo.VehicleColor = info.stTrafficCar.szVehicleColor;
            eventInfo.VehicleSize = GetVehicleSize((EM_VehicleSizeType)info.stTrafficCar.nVehicleSize);
            eventInfo.VehicleType = System.Text.Encoding.Default.GetString(info.stuVehicle.szObjectSubType);
            eventInfo.LaneNumber = info.stTrafficCar.nLane.ToString();
            eventInfo.Mode = Mode.Chiqqanlar;
            if (IntPtr.Zero != pBuffer && dwBufSize > 0)
            {
                byte[] buffer = new byte[dwBufSize];
                Marshal.Copy(pBuffer, buffer, 0, (int)dwBufSize);
                eventInfo.FileName = SavePicture(buffer);
            }


            UpdateUI(eventInfo);
            m_ID++;
            return 0;
        }

        private void UpdateUI(EventInfo eventInfo)
        {
            Thread thread = new Thread(async () =>
            {
                await genericRepository.AddAsync(eventInfo);
                await genericRepository.SaveChangesAsync();
            });
            thread.Start();

            this.Dispatcher.Invoke(() =>
            {
                InfoView infoView = new InfoView()
                {
                    FileName = eventInfo.FileName,
                    PlateColor = eventInfo.PlateColor,
                    PlateNumber = eventInfo.PlateNumber,
                    VehicleColor = eventInfo.VehicleColor,
                    VehicleSize = eventInfo.VehicleSize,
                    VehicleType = eventInfo.VehicleType,
                    Time = eventInfo.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                    Date = DateTime.Now,
                };
                if (eventInfo.Mode == Mode.Kirganlar)
                {
                    dataItems.Add(infoView);
                    EnteredCarsTxt.Text = ((int.Parse(EnteredCarsTxt.Text)) + 1).ToString();
                }
                else
                {
                    dataLeavedItems.Add(infoView);
                    LeavedCarsTxt.Text = ((int.Parse(LeavedCarsTxt.Text)) + 1).ToString();
                    if (exitPage is not null)
                        exitPage.Close();

                    exitPage = new HanldeExitPage();
                    exitPage.PlateNumber = eventInfo.PlateNumber;
                    exitPage.DateExit = eventInfo.Time;
                    var entered = dataItems
                            .OrderBy(x => x.Time)
                            .LastOrDefault(x => x.PlateNumber == eventInfo.PlateNumber);
                    if (entered is not null)
                        exitPage.DateEnter = entered.Date;
                    FileInfo fileInfo = new FileInfo(eventInfo.FileName);
                    exitPage.m_LoginID = m_LeavedLoginID;
                    exitPage.TrafficImage.Source = new BitmapImage(new Uri(fileInfo.FullName, UriKind.Absolute));
                    exitPage.Show();
                }
            });
        }

        private void EnterDataGrid_Selected(object sender, MouseButtonEventArgs e)
        {
            var datagrid = sender as DataGrid;

            if (datagrid.SelectedItem != null)
            {
                // Get the selected item
                InfoView selectedItem = (InfoView)datagrid.SelectedItem;
                string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                string directoryName = Path.GetDirectoryName(assemblyLocation);
                var fileName = Path.Combine(directoryName,selectedItem.FileName);
                if (selectedItem != null)
                {
                    InfoPage infoPage = new InfoPage();
                    infoPage.Owner = this;
                    infoPage.PlateColorTxt.Text += selectedItem.PlateColor;
                    infoPage.PlateNumberTxt.Text += selectedItem.PlateNumber;
                    infoPage.TimeTxt.Text += selectedItem.Time;
                    infoPage.VehicleColorTxt.Text += selectedItem.VehicleColor;
                    infoPage.VehicleSizeTxt.Text += selectedItem.VehicleSize;
                    infoPage.VehicleTypeTxt.Text += selectedItem.VehicleType;
                    if (File.Exists(fileName))
                    {
                        FileInfo file = new FileInfo(fileName);
                        infoPage.InfoImage.Source = new BitmapImage(new Uri(file.FullName, UriKind.Absolute));
                    }
                    infoPage.ShowDialog();
                }
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            searchPage = new SearchPage();
            searchPage.Owner = this;
            searchPage.ShowDialog();
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            SettingsPage settingsPage = new SettingsPage();
            settingsPage.Owner = this;
            settingsPage.RequestReload += ReloadLogin;
            settingsPage.Show();
        }

        private void ReloadLogin(object sender, EventArgs e)
        {
            var credentials = sender as CameraCredentials;
            if (credentials != null)
            {
                if (credentials.Mode == Mode.Kirganlar)
                {
                    if (m_EventID != IntPtr.Zero)
                    {
                        NETClient.StopLoadPic(m_EventID);
                        m_EventID = IntPtr.Zero;
                    }
                    if (m_RealPlayID != IntPtr.Zero)
                    {
                        NETClient.StopRealPlay(m_RealPlayID);
                        m_RealPlayID = IntPtr.Zero;
                    }
                    if (m_LoginID != IntPtr.Zero)
                    {
                        NETClient.Logout(m_LoginID);
                        m_LoginID = IntPtr.Zero;
                    }

                    LoginBtn_Click(credentials);
                }
                else
                {
                    if (m_LeavedEventID != IntPtr.Zero)
                    {
                        NETClient.StopLoadPic(m_LeavedEventID);
                        m_LeavedEventID = IntPtr.Zero;
                    }
                    if (m_LeavedRealPlayID != IntPtr.Zero)
                    {
                        NETClient.StopRealPlay(m_LeavedRealPlayID);
                        m_LeavedRealPlayID = IntPtr.Zero;
                    }
                    if (m_LeavedLoginID != IntPtr.Zero)
                    {
                        NETClient.Logout(m_LeavedLoginID);
                        m_LeavedLoginID = IntPtr.Zero;
                    }
                    LoginLeaveBtn_Click(credentials);
                }
            }
        }
    }
}