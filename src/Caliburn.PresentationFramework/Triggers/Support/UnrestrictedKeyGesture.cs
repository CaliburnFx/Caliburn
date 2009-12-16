#if !SILVERLIGHT

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Markup;

namespace Caliburn.PresentationFramework.Triggers.Support
{
    /// <summary>
    /// This is an import of System.Windows.Input.KeyGesture, with the validation behaviour disabled.
    /// </summary>
    /// <remarks>
    /// The default KeyGesture supplied with WPF is hard-coded to validate the key combination, and does
    /// so based on an arbitary rule to disallow any alpha-numeric keys without modifiers. This decision
    /// makes it very difficult to create a mouseless application with WPF and Caliburn.
    /// </remarks>
    [TypeConverter(typeof(UnrestrictedKeyGestureConverter)), ValueSerializer(typeof(UnrestrictedKeyGestureValueSerializer))]
    public class UnrestrictedKeyGesture : InputGesture
    {
        private string _displayString;
        private Key _key;
        private static TypeConverter _keyGestureConverter = new UnrestrictedKeyGestureConverter();
        private ModifierKeys _modifiers;
        private const string MULTIPLEGESTURE_DELIMITER = ";";

        public UnrestrictedKeyGesture(Key key)
            : this(key, ModifierKeys.None)
        { }

        public UnrestrictedKeyGesture(Key key, ModifierKeys modifiers)
            : this(key, modifiers, string.Empty, true)
        { }

        internal UnrestrictedKeyGesture(Key key, ModifierKeys modifiers, bool validateGesture)
            : this(key, modifiers, string.Empty, validateGesture)
        { }

        public UnrestrictedKeyGesture(Key key, ModifierKeys modifiers, string displayString)
            : this(key, modifiers, displayString, true)
        { }

        private UnrestrictedKeyGesture(Key key, ModifierKeys modifiers, string displayString, bool validateGesture)
        {
            if (!ModifierKeysConverter.IsDefinedModifierKeys(modifiers))
                throw new InvalidEnumArgumentException("modifiers", (int)modifiers, typeof(ModifierKeys));
            if (!IsDefinedKey(key))
                throw new InvalidEnumArgumentException("key", (int)key, typeof(Key));
            if (displayString == null)
                throw new ArgumentNullException("displayString");

            // Do not validate the gesture, can't see any reason why you'd want to limit what keys a gesture
            // can be composed of. Default behaviour is to require modifier keys for alpha-numeric keys
            // if (validateGesture && !IsValid(key, modifiers))
            // {
            //     throw new NotSupportedException(SR.Get("KeyGesture_Invalid", new object[] { modifiers, key }));
            // }

            _modifiers = modifiers;
            _key = key;
            _displayString = displayString;
        }

        internal static void AddGesturesFromResourceStrings(string keyGestures, string displayStrings, InputGestureCollection gestures)
        {
            while (!string.IsNullOrEmpty(keyGestures))
            {
                string str;
                string str2;
                int index = keyGestures.IndexOf(";", StringComparison.Ordinal);
                if (index >= 0)
                {
                    str2 = keyGestures.Substring(0, index);
                    keyGestures = keyGestures.Substring(index + 1);
                }
                else
                {
                    str2 = keyGestures;
                    keyGestures = string.Empty;
                }
                index = displayStrings.IndexOf(";", StringComparison.Ordinal);
                if (index >= 0)
                {
                    str = displayStrings.Substring(0, index);
                    displayStrings = displayStrings.Substring(index + 1);
                }
                else
                {
                    str = displayStrings;
                    displayStrings = string.Empty;
                }
                UnrestrictedKeyGesture inputGesture = CreateFromResourceStrings(str2, str);
                if (inputGesture != null)
                {
                    gestures.Add(inputGesture);
                }
            }
        }

        internal static UnrestrictedKeyGesture CreateFromResourceStrings(string keyGestureToken, string keyDisplayString)
        {
            if (!string.IsNullOrEmpty(keyDisplayString))
                keyGestureToken = keyGestureToken + ',' + keyDisplayString;

            return (_keyGestureConverter.ConvertFromInvariantString(keyGestureToken) as UnrestrictedKeyGesture);
        }

        public string GetDisplayStringForCulture(CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(_displayString))
                return _displayString;

            return (string)_keyGestureConverter.ConvertTo(null, culture, this, typeof(string));
        }

        internal static bool IsDefinedKey(Key key)
        {
            return ((key >= Key.None) && (key <= Key.OemClear));
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            KeyEventArgs args = inputEventArgs as KeyEventArgs;

            if ((args == null) || !IsDefinedKey(args.Key))
                return false;

            return ((Key == args.Key) && (Modifiers == Keyboard.Modifiers));
        }

        public string DisplayString
        {
            get { return _displayString; }
        }

        public Key Key
        {
            get { return _key; }
        }

        public ModifierKeys Modifiers
        {
            get { return _modifiers; }
        }
    }
}

#endif