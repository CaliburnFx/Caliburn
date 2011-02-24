#if !SILVERLIGHT

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Markup;

namespace Caliburn.PresentationFramework.RoutedMessaging.Triggers.Support
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
        readonly string displayString;
        readonly Key key;
        static readonly TypeConverter KeyGestureConverter = new UnrestrictedKeyGestureConverter();
        readonly ModifierKeys modifiers;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnrestrictedKeyGesture"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public UnrestrictedKeyGesture(Key key)
            : this(key, ModifierKeys.None)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnrestrictedKeyGesture"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="modifiers">The modifiers.</param>
        public UnrestrictedKeyGesture(Key key, ModifierKeys modifiers)
            : this(key, modifiers, string.Empty, true)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnrestrictedKeyGesture"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="modifiers">The modifiers.</param>
        /// <param name="validateGesture">if set to <c>true</c> [validate gesture].</param>
        internal UnrestrictedKeyGesture(Key key, ModifierKeys modifiers, bool validateGesture)
            : this(key, modifiers, string.Empty, validateGesture)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnrestrictedKeyGesture"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="modifiers">The modifiers.</param>
        /// <param name="displayString">The display string.</param>
        public UnrestrictedKeyGesture(Key key, ModifierKeys modifiers, string displayString)
            : this(key, modifiers, displayString, true)
        { }

        UnrestrictedKeyGesture(Key key, ModifierKeys modifiers, string displayString, bool validateGesture)
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

            this.modifiers = modifiers;
            this.key = key;
            this.displayString = displayString;
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
                var inputGesture = CreateFromResourceStrings(str2, str);
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

            return (KeyGestureConverter.ConvertFromInvariantString(keyGestureToken) as UnrestrictedKeyGesture);
        }

        /// <summary>
        /// Gets the display string for culture.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public string GetDisplayStringForCulture(CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(displayString))
                return displayString;

            return (string)KeyGestureConverter.ConvertTo(null, culture, this, typeof(string));
        }

        internal static bool IsDefinedKey(Key key)
        {
            return ((key >= Key.None) && (key <= Key.OemClear));
        }

        /// <summary>
        /// When overridden in a derived class, determines whether the specified <see cref="T:System.Windows.Input.InputGesture"/> matches the input associated with the specified <see cref="T:System.Windows.Input.InputEventArgs"/> object.
        /// </summary>
        /// <param name="targetElement">The target of the command.</param>
        /// <param name="inputEventArgs">The input event data to compare this gesture to.</param>
        /// <returns>
        /// true if the gesture matches the input; otherwise, false.
        /// </returns>
        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            var args = inputEventArgs as KeyEventArgs;

            if ((args == null) || !IsDefinedKey(args.Key))
                return false;

            return ((Key == args.Key || (args.Key == Key.System && Key == args.SystemKey)) && (Modifiers == Keyboard.Modifiers));
        }

        /// <summary>
        /// Gets the display string.
        /// </summary>
        /// <value>The display string.</value>
        public string DisplayString
        {
            get { return displayString; }
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public Key Key
        {
            get { return key; }
        }

        /// <summary>
        /// Gets the modifiers.
        /// </summary>
        /// <value>The modifiers.</value>
        public ModifierKeys Modifiers
        {
            get { return modifiers; }
        }
    }
}

#endif