namespace Caliburn.PresentationFramework.Parsers
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Core;
    using Triggers;

    /// <summary>
    /// An implementation of <see cref="ITriggerParser"/> that parses gestures.
    /// </summary>
    public class GestureTriggerParser : ITriggerParser
    {
        /// <summary>
        /// Parses the specified trigger text.
        /// </summary>
        /// <param name="target">The targeted ui element.</param>
        /// <param name="triggerText">The trigger text.</param>
        /// <returns></returns>
        public IMessageTrigger Parse(DependencyObject target, string triggerText)
        {
            var keyValues = triggerText.Split(new[] {",", ":"}, StringSplitOptions.RemoveEmptyEntries);
            var trigger = new GestureMessageTrigger();

            for(int i = 0; i < keyValues.Length; i += 2)
            {
                switch(keyValues[i].Trim())
                {
                    case "Key":
#if !SILVERLIGHT
                        trigger.Key = (Key)new KeyConverter()
                                               .ConvertFrom(keyValues[i + 1].Trim());
#else
                        trigger.Key = (Key)Enum.Parse(typeof(Key), keyValues[i + 1].Trim(), true);
#endif
                        break;
                    case "Modifiers":
#if !SILVERLIGHT
                        trigger.Modifiers = (ModifierKeys)new ModifierKeysConverter()
                                                              .ConvertFrom(keyValues[i + 1].Trim());
#else
                        trigger.Modifiers = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), keyValues[i + 1].Trim(), true);
#endif
                        break;
                    case "MouseAction":
#if !SILVERLIGHT
                        trigger.MouseAction = (MouseAction)new MouseActionConverter()
                                                               .ConvertFrom(keyValues[i + 1].Trim());
#else
                        trigger.MouseAction = (MouseAction)Enum.Parse(typeof(MouseAction), keyValues[i + 1].Trim(), true);
#endif
                        break;
                    default:
                        throw new CaliburnException(keyValues[i] + " was not recognized by the gesture trigger parser.");
                }
            }

            return trigger;
        }
    }
}