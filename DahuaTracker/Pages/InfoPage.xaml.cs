using System.Windows;
using System.Windows.Input;

namespace DahuaTracker.Pages
{
    /// <summary>
    /// Логика взаимодействия для InfoPage.xaml
    /// </summary>
    public partial class InfoPage : Window
    {
        public InfoPage()
        {
            InitializeComponent();
        }
        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void InfoImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ImagePage imagePage = new ImagePage();
            imagePage.Owner = this;
            imagePage.InfoImage.Source = InfoImage.Source;
            imagePage.ShowDialog();
        }
    }
}
