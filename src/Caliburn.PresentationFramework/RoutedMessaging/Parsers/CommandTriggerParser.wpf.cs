#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.Parsers
{
    using System.Windows;
    using Triggers;

    /// <summary>
    /// An implementation of <see cref="ITriggerParser"/> that parses command source hookups.
    /// </summary>
    public class CommandTriggerParser : ITriggerParser
    {
        /// <summary>
        /// Parses the specified trigger text.
        /// </summary>
        /// <param name="target">The targeted ui element.</param>
        /// <param name="triggerText">The trigger text.</param>
        /// <returns></returns>
        public IMessageTrigger Parse(DependencyObject target, string triggerText)
        {
            return new CommandMessageTrigger();
        }
    }
}

#endif