namespace Tests.Caliburn.Fakes.UI
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class SimpleMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[0];
        }
    }
}