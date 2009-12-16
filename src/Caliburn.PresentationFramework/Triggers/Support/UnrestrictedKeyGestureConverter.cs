#if !SILVERLIGHT

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;

namespace Caliburn.PresentationFramework.Triggers.Support
{
    /// <summary>
    /// This is an import of System.Windows.Input.KeyGestureConverter, unmodified except for references
    /// to KeyGesture replaced with UnrestrictedKeyGesture.
    /// </summary>
    public class UnrestrictedKeyGestureConverter : TypeConverter
    {
        internal const char DISPLAYSTRING_SEPARATOR = ',';
        private static KeyConverter keyConverter = new KeyConverter();
        private static ModifierKeysConverter modifierKeysConverter = new ModifierKeysConverter();
        private const char MODIFIERS_DELIMITER = '+';

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (((destinationType == typeof(string)) && (context != null)) && (context.Instance != null))
            {
                UnrestrictedKeyGesture instance = context.Instance as UnrestrictedKeyGesture;
                if (instance != null)
                {
                    return (ModifierKeysConverter.IsDefinedModifierKeys(instance.Modifiers) && IsDefinedKey(instance.Key));
                }
            }
            return false;
        }

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
                ModifierKeys none = ModifierKeys.None;
                object obj3 = keyConverter.ConvertFrom(context, culture, str2);
                if (obj3 != null)
                {
                    object obj2 = modifierKeysConverter.ConvertFrom(context, culture, str3);
                    if (obj2 != null)
                    {
                        none = (ModifierKeys)obj2;
                    }
                    return new UnrestrictedKeyGesture((Key)obj3, none, str4);
                }
            }
            throw base.GetConvertFromException(source);
        }

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
                UnrestrictedKeyGesture gesture = value as UnrestrictedKeyGesture;
                if (gesture != null)
                {
                    if (gesture.Key == Key.None)
                    {
                        return string.Empty;
                    }
                    string str = "";
                    string str2 = (string)keyConverter.ConvertTo(context, culture, gesture.Key, destinationType);
                    if (str2 != string.Empty)
                    {
                        str = str + (modifierKeysConverter.ConvertTo(context, culture, gesture.Modifiers, destinationType) as string);
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