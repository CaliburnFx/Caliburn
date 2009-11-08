namespace Caliburn.PresentationFramework.Parsers
{
    using System.Collections.Generic;
    using System.Windows;

    /// <summary>
    /// Parses text into a fully functional <see cref="IMessageTrigger"/>.
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Gets or sets the message delimiter.
        /// </summary>
        /// <value>The message delimiter.</value>
        string MessageDelimiter { get; set; }

        /// <summary>
        /// Sets the default message parser.
        /// </summary>
        /// <param name="key">The key.</param>
        void SetDefaultMessageParser(string key);

        /// <summary>
        /// Registers a trigger parser.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="parser">The parser.</param>
        void RegisterTriggerParser(string key, ITriggerParser parser);

        /// <summary>
        /// Registers a message parser.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="parser">The parser.</param>
        void RegisterMessageParser(string key, IMessageParser parser);

        /// <summary>
        /// Parses the specified message text.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="messageText">The message text.</param>
        /// <returns>The triggers parsed from the text.</returns>
        IEnumerable<IMessageTrigger> Parse(DependencyObject target, string messageText);
    }
}