using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BSUIR.ManagerQueue.Client.ValueConverters
{
    public class NullOrWhitespaceToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value as string;
            if (stringValue != null)
                return string.IsNullOrWhiteSpace(stringValue) ? Visibility.Visible : Visibility.Collapsed;

            return value == null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
