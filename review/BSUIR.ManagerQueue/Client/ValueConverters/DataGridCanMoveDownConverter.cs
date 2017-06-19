using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace BSUIR.ManagerQueue.Client.ValueConverters
{
    [ValueConversion(typeof(DataGrid), typeof(bool))]
    public class DataGridCanMoveDownConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dataGrid = value as DataGrid;
            if (dataGrid == null || !(targetType != typeof(bool)))
                throw new ArgumentException("Type mismatch.");

            var itemSource = dataGrid.ItemsSource;
            if (itemSource == null || !itemSource.OfType<object>().Any())
                return false;

            var itemCount = dataGrid.ItemsSource.OfType<object>().Count();
            return dataGrid.SelectedIndex < itemCount - 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
