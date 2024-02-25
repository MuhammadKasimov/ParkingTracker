using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DahuaTracker.Pages
{
    /// <summary>
    /// Логика взаимодействия для ImagePage.xaml
    /// </summary>
    public partial class ImagePage : Window
    {

        private System.Windows.Point origin;
        private System.Windows.Point start;
        public ImagePage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TransformGroup transformGroup = new TransformGroup();
            ScaleTransform scaleTransform = new ScaleTransform();

            transformGroup.Children.Add(scaleTransform);
            TranslateTransform translateTransform = new TranslateTransform();

            transformGroup.Children.Add(translateTransform);

            InfoImage.RenderTransform = transformGroup;
        }

        private void InfoImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            InfoImage.CaptureMouse();

            var translateTransform = (TranslateTransform)((TransformGroup)InfoImage.RenderTransform).Children.First(c => c is TranslateTransform);

            start = e.GetPosition(ImageBorder);

            origin = new System.Windows.Point(translateTransform.X, translateTransform.Y);
        }

        private void InfoImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            InfoImage.ReleaseMouseCapture();
        }

        private void InfoImage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!InfoImage.IsMouseCaptured) return;

            var translateTransform = (TranslateTransform)((TransformGroup)InfoImage.RenderTransform).Children.First(c => c is TranslateTransform);

            Vector v = start - e.GetPosition(ImageBorder);

            translateTransform.X = origin.X - v.X;

            translateTransform.Y = origin.Y - v.Y;
        }

        private void InfoImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var transform = (ScaleTransform)((TransformGroup)InfoImage.RenderTransform).Children.First(c => c is ScaleTransform);

            double zoom = e.Delta > 0 ? .2 : -.2;

            double newScale = transform.ScaleX + zoom;

            if (newScale > 0.8)
            {
                transform.ScaleX += zoom;
                transform.ScaleY += zoom;
            }
        }

    }
}
