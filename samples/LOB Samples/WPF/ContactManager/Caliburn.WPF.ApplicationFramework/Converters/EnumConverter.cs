namespace Caliburn.WPF.ApplicationFramework.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(typeof(int).IsAssignableFrom(targetType))
                return (int)value;

            return new BindableEnum
            {
                Value = value,
                UnderlyingValue = (int)value,
                DisplayName = Enum.GetName(value.GetType(), value)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
                return null;

            if(value is string)
                return Enum.Parse(targetType, value.ToString(), true);

            return value.GetType() == targetType
                       ? value
                       : ((BindableEnum)value).Value;
        }
    }
}