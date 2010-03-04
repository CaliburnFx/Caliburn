namespace Caliburn.PresentationFramework.Parsers
{
    using System.Windows;

    /// <summary>
    /// Parses an <see cref="IRoutedMessage"/> from text.
    /// </summary>
    public interface IMessageParser
    {
        /// <summary>
        /// Parses the specified message text.
        /// </summary>
        /// <param name="target">The targeted UI element.</param>
        /// <param name="messageText">The message text.</param>
        /// <returns></returns>
        IRoutedMessage Parse(DependencyObject target, string messageText);
    }
}