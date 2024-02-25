using DahuaTracker.Data.IRepositories;
using DahuaTracker.Data.Repositories;
using DahuaTracker.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DahuaTracker.Pages
{
    /// <summary>
    /// Логика взаимодействия для xaml
    /// </summary>
    public partial class SearchPage : Window
    {
        private readonly IGenericRepository<EventInfo> genericRepository;
        private ObservableCollection<InfoView> dataItems;
        private readonly CollectionViewSource collectionViewSource;

        public SearchPage()
        {
            InitializeComponent();

            dataItems = new ObservableCollection<InfoView>();
            collectionViewSource = new CollectionViewSource();

            collectionViewSource.Source = dataItems;
            EnterDataGrid.ItemsSource = collectionViewSource.View;

            genericRepository = new GenericRepository<EventInfo>();

            StartDatePicker.SelectedDate = DateTime.Now;
            EndDatePicker.SelectedDate = DateTime.Now;
            StartDatePicker.SelectedDateChanged += FilterByDate;
            EndDatePicker.SelectedDateChanged += FilterByDate;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartTime.Text = "00:00";
            EndTime.Text = "23:59";
            LoadDefault();
        }
        private void FilterByDate(object sender, SelectionChangedEventArgs e)
        {
            DateTime testDate;
            DateTime startDate = (DateTime)StartDatePicker.SelectedDate;
            DateTime endDate = (DateTime)EndDatePicker.SelectedDate;
            if (DateTime.TryParseExact(
                StartTime?.Text,
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out testDate))
            {
                startDate += testDate.TimeOfDay;
            }
            if (DateTime.TryParseExact(
                EndTime?.Text,
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out testDate))
            {
                endDate += testDate.TimeOfDay;
            }
            LoadByDate(startDate, endDate);
        }


        private void FilterByNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterByNumber(SearchTxt.Text);
        }
        private void FilterByNumber(string filterText)
        {
            ICollectionView view = collectionViewSource.View;

            if (!string.IsNullOrEmpty(filterText))
            {
                view.Filter = item =>
                {
                    InfoView dataItem = item as InfoView;
                    return dataItem != null && dataItem.PlateNumber.Contains(filterText);
                };
            }
            else
            {
                view.Filter = null;
            }

            EnterCountTxt.Text = dataItems.Count.ToString();
        }

        private void LoadByDate(DateTime startDateTime, DateTime endDateTime)
        {
            dataItems.Clear();
            var data = genericRepository.GetAll(
                ei => ei.Time >= startDateTime &&
                ei.Time <= endDateTime)
                .ToList();

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
                    Time = item.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                    Mode = item.Mode
                };

                dataItems.Add(infoView);
            }

            FilterByNumber(SearchTxt.Text);
        }
        private void LoadDefault()
        {
            TimeSpan time = TimeSpan.FromHours(23) + TimeSpan.FromMinutes(59);
            var endDate = DateTime.Now.Date + time;
            DateTime startDateTime = DateTime.Now.Date;
            DateTime endDateTime = endDate;

            var data = genericRepository.GetAll(
                ei => ei.Time.Date >= startDateTime.Date &&
                ei.Time.Date <= endDateTime.Date).AsEnumerable();

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
                    Time = item.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                    Mode = item.Mode
                };

                dataItems.Add(infoView);
            }

            EnterCountTxt.Text = dataItems.Count.ToString();
        }

        private void EnterDataGrid_SelectionChanged(object sender, MouseButtonEventArgs e)
        {
            var datagrid = sender as DataGrid;

            if (datagrid.SelectedItem != null)
            {
                // Get the selected item
                InfoView selectedItem = (InfoView)datagrid.SelectedItem;

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
                    if (File.Exists(selectedItem.FileName))
                    {
                        FileInfo file = new FileInfo(selectedItem.FileName);
                        infoPage.InfoImage.Source = new BitmapImage(new Uri(file.FullName, UriKind.Absolute));
                    }
                    infoPage.ShowDialog();
                }
            }
        }

        private void EndTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            DateTime startDate = (DateTime)StartDatePicker.SelectedDate;
            DateTime endDate = (DateTime)EndDatePicker.SelectedDate;
            DateTime testDate;
            if (DateTime.TryParseExact(
                StartTime?.Text,
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out testDate))
            {
                startDate += testDate.TimeOfDay;
            }
            if (DateTime.TryParseExact(
                EndTime?.Text,
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out testDate))
            {
                endDate += testDate.TimeOfDay;
            }
            LoadByDate(startDate, endDate);
        }

        private void StartTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            DateTime startDate = (DateTime)StartDatePicker.SelectedDate;
            DateTime endDate = (DateTime)EndDatePicker.SelectedDate;
            DateTime testDate;
            if (DateTime.TryParseExact(
                StartTime?.Text,
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out testDate))
            {
                startDate += testDate.TimeOfDay;
            }
            if (DateTime.TryParseExact(
                EndTime?.Text,
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out testDate))
            {
                endDate += testDate.TimeOfDay;
            }
            LoadByDate(startDate, endDate);
        }

        private void LeaveCheckBox_Click(object sender, RoutedEventArgs e)
        {
            ICollectionView view = collectionViewSource.View;

            if (LeaveCheckBox.IsChecked == true && EnterCheckBox.IsChecked == true ||
                LeaveCheckBox.IsChecked == false && EnterCheckBox.IsChecked == false)
            {
                FilterByNumber(SearchTxt.Text);
            }
            else
            {
                Mode mode = Mode.Chiqqanlar;

                if (EnterCheckBox.IsChecked == true)
                    mode = Mode.Kirganlar;

                if (!string.IsNullOrEmpty(SearchTxt.Text))
                {
                    view.Filter = item =>
                    {
                        InfoView dataItem = item as InfoView;

                        return dataItem != null && dataItem.PlateNumber.Contains(SearchTxt.Text) && dataItem.Mode == mode;
                    };
                    EnterCountTxt.Text = dataItems.Count.ToString();
                }
                else
                {
                    view.Filter = item =>
                    {
                        InfoView dataItem = item as InfoView;

                        return dataItem != null && dataItem.Mode == mode;
                    };
                    EnterCountTxt.Text = dataItems.Count.ToString();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
