using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPF_MARKET_APP
{
    public class MyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo ture)
        {
            return ((double)value) - 220;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo ture)
        {
            throw new NotImplementedException();
        }
    }
    public class MyConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo ture)
        {
            return ((double)value) - 100;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo ture)
        {
            throw new NotImplementedException();
        }
    }

}
