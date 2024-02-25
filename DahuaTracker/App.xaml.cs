using System.Windows;

namespace DahuaTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            this.DispatcherUnhandledException += (sender, args) =>
            {
                Exception ex = args.Exception;

                args.Handled = true; // Mark the exception as handled to prevent application termination
            };
        }
    }

}
