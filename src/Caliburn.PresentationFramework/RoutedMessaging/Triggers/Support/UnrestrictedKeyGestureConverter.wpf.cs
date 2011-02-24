#if !SILVERLIGHT

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;

namespace Caliburn.PresentationFramework.RoutedMessaging.Triggers.Support
{
    /// <summary>
    /// This is an import of System.Windows.Input.KeyGestureConverter, unmodified except for references
    /// to KeyGesture replaced with UnrestrictedKeyGesture.
    /// </summary>
    public class UnrestrictedKeyGestureConverter : TypeConverter
    {
        internal const char DisplaystringSeparator = ',';
        static readonly KeyConverter KeyConverter = new KeyConverter();
        static readonly ModifierKeysConverter ModifierKeysConverter = new ModifierKeysConverter();

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (((destinationType == typeof(string)) && (context != null)) && (context.Instance != null))
            {
                var instance = context.Instance as UnrestrictedKeyGesture;
                if (instance != null)
                {
                    return (ModifierKeysConverter.IsDefinedModifierKeys(instance.Modifiers) && IsDefinedKey(instance.Key));
                }
            }
            return false;
        }

        /// <summary>
        /// Converts from.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object source)
        {
            if ((source != null) && (source is string))
            {
                string str2;
                string str3;
                string str4;
                string str = ((string)source).Trim();
                if (str == string.Empty)
                {
                    return new UnrestrictedKeyGesture(Key.None);
                }
                int index = str.IndexOf(',');
                if (index >= 0)
                {
                    str4 = str.Substring(index + 1).Trim();
                    str = str.Substring(0, index).Trim();
                }
                else
                {
                    str4 = string.Empty;
                }
                index = str.LastIndexOf('+');
                if (index >= 0)
                {
                    str3 = str.Substring(0, index);
                    str2 = str.Substring(index + 1);
                }
                else
                {
                    str3 = string.Empty;
                    str2 = str;
                }
                var none = ModifierKeys.None;
                var obj3 = KeyConverter.ConvertFrom(context, culture, str2);
                if (obj3 != null)
                {
                    object obj2 = ModifierKeysConverter.ConvertFrom(context, culture, str3);
                    if (obj2 != null)
                    {
                        none = (ModifierKeys)obj2;
                    }
                    return new UnrestrictedKeyGesture((Key)obj3, none, str4);
                }
            }
            throw base.GetConvertFromException(source);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="destinationType"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The conversion cannot be performed.
        /// </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if (destinationType == typeof(string))
            {
                if (value == null)
                {
                    return string.Empty;
                }
                var gesture = value as UnrestrictedKeyGesture;
                if (gesture != null)
                {
                    if (gesture.Key == Key.None)
                    {
                        return string.Empty;
                    }
                    var str = "";
                    var str2 = (string)KeyConverter.ConvertTo(context, culture, gesture.Key, destinationType);
                    if (str2 != string.Empty)
                    {
                        str = str + (ModifierKeysConverter.ConvertTo(context, culture, gesture.Modifiers, destinationType) as string);
                        if (str != string.Empty)
                        {
                            str = str + '+';
                        }
                        str = str + str2;
                        if (!string.IsNullOrEmpty(gesture.DisplayString))
                        {
                            str = str + ',' + gesture.DisplayString;
                        }
                    }
                    return str;
                }
            }
            throw base.GetConvertToException(value, destinationType);
        }

        internal static bool IsDefinedKey(Key key)
        {
            return ((key >= Key.None) && (key <= Key.OemClear));
        }
    }
}

#endif