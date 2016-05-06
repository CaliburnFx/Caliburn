namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using System;

    /// <summary>
    /// The outcome of processing a message.
    /// </summary>
    public class MessageProcessingOutcome
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessingOutcome"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="resultType">Type of the result.</param>
        /// <param name="wasCancelled">if set to <c>true</c> [was cancelled].</param>
        public MessageProcessingOutcome(object result, Type resultType, bool wasCancelled)
        {
            Result = result;
            ResultType = resultType;
            WasCancelled = wasCancelled;
        }

        /// <summary>
        /// Gets or sets a value indicating whether message processing was cancelled.
        /// </summary>
        /// <value><c>true</c> if cancelled; otherwise, <c>false</c>.</value>
        public bool WasCancelled { get; private set; }

        /// <summary>
        /// Gets or sets the type of the result.
        /// </summary>
        /// <value>The type of the result.</value>
        public Type ResultType { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public object Result { get; set; }
    }
}