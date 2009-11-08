namespace Caliburn.PresentationFramework
{
    /// <summary>
    /// Allows am <see cref="IMessageBinder"/> to bind return values.
    /// </summary>
    public interface IRoutedMessageWithOutcome : IRoutedMessage
    {
        /// <summary>
        /// Gets or sets the path to use in binding the <see cref="MessageProcessingOutcome"/>.
        /// </summary>
        /// <value>The outcome path.</value>
        string OutcomePath { get; }

        /// <summary>
        /// Gets the default element to bind to if no outcome path is specified.
        /// </summary>
        /// <value>The default element.</value>
        string DefaultOutcomeElement { get; }
    }
}