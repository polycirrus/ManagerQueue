using System;
using System.Globalization;
using System.Windows.Data;

namespace BSUIR.ManagerQueue.Client.ValueConverters
{
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new NotSupportedException($"{nameof(NullToBoolConverter)} only supports conversion to bool.");

            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
