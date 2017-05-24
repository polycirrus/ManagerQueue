using BSUIR.ManagerQueue.Client.Properties;
using BSUIR.ManagerQueue.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BSUIR.ManagerQueue.Client.ValueConverters
{
    [ValueConversion(typeof(UserType), typeof(string))]
    public class UserTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is UserType) || targetType != typeof(string))
                throw new ArgumentException("Type mismatch");

            var userType = (UserType)value;
            switch (userType)
            {
                case UserType.Manager:
                    return Resources.ManagerRoleName;
                case UserType.Secretary:
                    return Resources.SecretaryRoleName;
                case UserType.Vice:
                    return Resources.ViceManagerRoleName;
                case UserType.Employee:
                default:
                    return Resources.EmployeeRoleName;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
