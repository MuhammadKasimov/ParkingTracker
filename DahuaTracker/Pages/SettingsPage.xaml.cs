using DahuaTracker.Data.IRepositories;
using DahuaTracker.Data.Repositories;
using DahuaTracker.Enums;
using System.Windows;

namespace DahuaTracker.Pages
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Window
    {
        IGenericRepository<CameraCredentials> genericRepository;
        public event EventHandler RequestReload;
        public SettingsPage()
        {
            genericRepository = new GenericRepository<CameraCredentials>();
            InitializeComponent();
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            CameraCredentials credentials = genericRepository.Get(c => c.Mode == Mode.Kirganlar);

            if (credentials == null)
                credentials = new CameraCredentials();

            credentials.Ip = IpTxt.Text;
            credentials.Port = PortTxt.Text;
            credentials.Login = UsernameTxt.Text;
            credentials.Password = PasswordTxt.Password;
            genericRepository.Update(c => c.Mode == Mode.Chiqqanlar, credentials);

            await genericRepository.SaveChangesAsync();

            RequestReload?.Invoke(credentials, EventArgs.Empty);
        }

        private async void SaveLeaveBtn_Click(object sender, RoutedEventArgs e)
        {
            CameraCredentials credentials = genericRepository.Get(c => c.Mode == Mode.Chiqqanlar);

            if (credentials == null)
                credentials = new CameraCredentials();

            credentials.Ip = IpLeaveTxt.Text;
            credentials.Port = PortLeaveTxt.Text;
            credentials.Login = UsernameLeaveTxt.Text;
            credentials.Password = PasswordLeaveTxt.Password;
            genericRepository.Update(c => c.Mode == Mode.Chiqqanlar, credentials);

            await genericRepository.SaveChangesAsync();
            RequestReload?.Invoke(credentials, EventArgs.Empty);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var credentials = genericRepository.GetAll().OrderBy(c => c.Mode).ToList();

            if (credentials.Count == 2)
            {
                IpTxt.Text = credentials[0].Ip;
                PortTxt.Text = credentials[0].Port;
                UsernameTxt.Text = credentials[0].Login;
                PasswordTxt.Password = credentials[0].Password;

                IpLeaveTxt.Text = credentials[1].Ip;
                PortLeaveTxt.Text = credentials[1].Port;
                UsernameLeaveTxt.Text = credentials[1].Login;
                PasswordLeaveTxt.Password = credentials[1].Password;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
