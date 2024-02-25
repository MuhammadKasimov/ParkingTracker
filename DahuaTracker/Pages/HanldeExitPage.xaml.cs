using NetSDKCS;
using System.Printing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Xps;

namespace DahuaTracker.Pages
{
    /// <summary>
    /// Логика взаимодействия для HanldeExitPage.xaml
    /// </summary>
    public partial class HanldeExitPage : Window
    {
        public string PlateNumber { get; set; }
        public DateTime DateEnter { get; set; }
        public DateTime DateExit { get; set; }
        private const int m_WaitTime = 20000;
        public IntPtr m_LoginID = IntPtr.Zero;

        public HanldeExitPage()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FlowDocument document = new FlowDocument();
            document.PageWidth = 180;

            BitmapImage logo = new BitmapImage(new Uri("pack://application:,,,/dahua-logo.png"));
            System.Windows.Controls.Image logoImage = new System.Windows.Controls.Image();
            logoImage.Source = logo;
            logoImage.Width = 130; // Adjust width as needed
            logoImage.Margin = new Thickness(0, 0, 0, 10);

            document.Blocks.Add(new BlockUIContainer(logoImage));
            document.Blocks.Add(new BlockUIContainer(new TextBlock()
            {
                Text = $"Moshina nomeri:\n{PlateNumber}\n"
            }));
            document.Blocks.Add(new BlockUIContainer(new TextBlock()
            {
                Text = $"Narx:\n5000\n"
            }));
            document.Blocks.Add(new BlockUIContainer(new TextBlock()
            {
                Text = $"Kirgan Vaqt:\n{DateEnter.ToString("g")}\n"
            }));
            document.Blocks.Add(new BlockUIContainer(new TextBlock()
            {
                Text = $"Chiqqan Vaqt:\n{DateExit.ToString("g")}\n"
            }));

            BillReader.Document = document;
        }

        private void PrintBillBtn_Click(object sender, RoutedEventArgs e)
        {
            LocalPrintServer localPrintServer = new LocalPrintServer();
            PrintQueue defaultPrintQueue = localPrintServer.DefaultPrintQueue;

            // Create a PrintTicket
            PrintTicket printTicket = defaultPrintQueue.DefaultPrintTicket;

            // Send the FlowDocument to the printer
            XpsDocumentWriter xpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(defaultPrintQueue);
            xpsDocumentWriter.Write(((IDocumentPaginatorSource)BillReader.Document).DocumentPaginator, printTicket);

            OpenBtn.IsEnabled = true;
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            NET_CTRL_OPEN_STROBE openStrobe = new NET_CTRL_OPEN_STROBE();
            openStrobe.dwSize = (uint)Marshal.SizeOf(typeof(NET_CTRL_OPEN_STROBE));
            openStrobe.nChannelId = 0;
            openStrobe.szPlateNumber = "1";
            openStrobe.emOpenType = EM_OPEN_STROBE_TYPE.EM_OPEN_STROBE_TYPE_TEST;

            IntPtr pOpenStrobe = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NET_CTRL_OPEN_STROBE)));
            Marshal.StructureToPtr(openStrobe, pOpenStrobe, true);
            bool ret = NETClient.ControlDevice(m_LoginID, EM_CtrlType.OPEN_STROBE, pOpenStrobe, m_WaitTime);
            Marshal.FreeHGlobal(pOpenStrobe);
            if (!ret)
            {
                System.Windows.MessageBox.Show(this, NETClient.GetLastError());
                return;
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
