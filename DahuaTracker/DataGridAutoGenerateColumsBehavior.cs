using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DahuaTracker
{
    public static class DataGridAutoGenerateColumnsBehavior
    {
        public static readonly DependencyProperty UseDisplayNameForColumnHeadersProperty =
            DependencyProperty.RegisterAttached("UseDisplayNameForColumnHeaders", typeof(bool), typeof(DataGridAutoGenerateColumnsBehavior), new PropertyMetadata(false, OnUseDisplayNameForColumnHeadersChanged));

        public static bool GetUseDisplayNameForColumnHeaders(DataGrid obj)
        {
            return (bool)obj.GetValue(UseDisplayNameForColumnHeadersProperty);
        }

        public static void SetUseDisplayNameForColumnHeaders(DataGrid obj, bool value)
        {
            obj.SetValue(UseDisplayNameForColumnHeadersProperty, value);
        }

        private static void OnUseDisplayNameForColumnHeadersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid)
            {
                if ((bool)e.NewValue)
                {
                    dataGrid.AutoGeneratingColumn += DataGrid_AutoGeneratingColumn;
                }
                else
                {
                    dataGrid.AutoGeneratingColumn -= DataGrid_AutoGeneratingColumn;
                }
            }
        }

        private static void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyDescriptor is PropertyDescriptor propDesc)
            {
                var displayNameAttr = propDesc.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;
                if (displayNameAttr != null)
                {
                    e.Column.Header = displayNameAttr.DisplayName;
                }
            }
        }
    }
}
