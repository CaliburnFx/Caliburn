namespace BackgroundProcessing.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Input;

    public class WaitCursorWhenTrueConverter : IValueConverter
    {
        public static readonly WaitCursorWhenTrueConverter Instance = new WaitCursorWhenTrueConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool && ((bool)value))
                return Cursors.Wait;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}