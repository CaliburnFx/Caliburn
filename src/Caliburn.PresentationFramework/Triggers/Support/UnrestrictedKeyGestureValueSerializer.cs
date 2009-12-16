#if !SILVERLIGHT

using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Markup;

namespace Caliburn.PresentationFramework.Triggers.Support
{
    /// <summary>
    /// This is an import of System.Windows.Input.KeyGestureValueSerializer, unmodified except for references
    /// to KeyGesture replaced with UnrestrictedKeyGesture.
    /// </summary>
    public class UnrestrictedKeyGestureValueSerializer : ValueSerializer
    {
        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }

        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            UnrestrictedKeyGesture gesture = value as UnrestrictedKeyGesture;
            return (((gesture != null) && ModifierKeysConverter.IsDefinedModifierKeys(gesture.Modifiers)) && UnrestrictedKeyGestureConverter.IsDefinedKey(gesture.Key));
        }

        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(UnrestrictedKeyGesture));
            if (converter != null)
            {
                return converter.ConvertFromString(value);
            }
            return base.ConvertFromString(value, context);
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(UnrestrictedKeyGesture));
            if (converter != null)
            {
                return converter.ConvertToInvariantString(value);
            }
            return base.ConvertToString(value, context);
        }
    }
}

#endif