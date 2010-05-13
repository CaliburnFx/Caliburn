namespace Caliburn.FluentValidation
{
    using Core.Validation;
    using global::FluentValidation.Results;

    /// <summary>
    /// Adapts <see cref="ValidationFailure"/> to <see cref="IValidationError"/>.
    /// </summary>
    public class ValidationErrorAdapter : IValidationError
    {
        private readonly ValidationFailure _failure;
        private readonly object _instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationErrorAdapter"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="failure">The failure.</param>
        public ValidationErrorAdapter(object instance, ValidationFailure failure)
        {
            _failure = failure;
            _instance = instance;
        }

        /// <summary>
        /// Gets the failure details.
        /// </summary>
        /// <value>The failure.</value>
        public ValidationFailure Failure
        {
            get { return _failure; }
        }

        /// <summary>
        /// Gets the invalid instance.
        /// </summary>
        /// <value>The instance.</value>
        public object Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets the name of the invalid property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName
        {
            get { return _failure.PropertyName; }
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get { return _failure.ErrorMessage; }
        }
    }
}