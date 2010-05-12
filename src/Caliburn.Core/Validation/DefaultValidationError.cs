namespace Caliburn.Core.Validation
{
    /// <summary>
    /// The default implementation of <see cref="IValidationError"/>.
    /// </summary>
    public class DefaultValidationError : IValidationError
    {
        /// <summary>
        /// Gets the invalid instance.
        /// </summary>
        /// <value>The instance.</value>
        public object Instance { get; private set; }

        /// <summary>
        /// Gets the name of the invalid property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValidationError"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="message">The message.</param>
        public DefaultValidationError(object instance, string propertyName, string message)
        {
            Instance = instance;
            PropertyName = propertyName;
            Message = message;
        }
    }
}