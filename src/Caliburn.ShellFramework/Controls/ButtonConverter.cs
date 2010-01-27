namespace Caliburn.ShellFramework.Controls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using Core;
    using PresentationFramework;

    public class ButtonConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if(typeof(string).IsAssignableFrom(sourceType))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var theString = value.ToString();
            var parts = theString.Split(new[] {' ', ','}, StringSplitOptions.RemoveEmptyEntries);

            var collection = new BindableCollection<ButtonModel>();
            parts.Apply(x => collection.Add(new ButtonModel(x)));

            return collection;
        }
    }
}