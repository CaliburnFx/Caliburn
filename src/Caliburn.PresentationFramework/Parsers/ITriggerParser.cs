namespace Caliburn.PresentationFramework.Parsers
{
    using System.Windows;

    /// <summary>
    /// Parses the trigger specific aspects of an <see cref="IMessageTrigger"/>.
    /// </summary>
    public interface ITriggerParser
    {
        /// <summary>
        /// Parses the specified trigger text.
        /// </summary>
        /// <param name="target">The targeted ui element.</param>
        /// <param name="triggerText">The trigger text.</param>
        /// <returns></returns>
        IMessageTrigger Parse(DependencyObject target, string triggerText);
    }
}