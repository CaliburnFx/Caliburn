namespace Caliburn.FluentValidation
{
    using Core.Validation;
    using global::FluentValidation.Results;

    /// <summary>
    /// Adapts <see cref="ValidationFailure"/> to <see cref="IError"/>.
    /// </summary>
    public class ErrorAdapter : IError
    {
        private readonly ValidationFailure failure;
        private readonly object instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorAdapter"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="failure">The failure.</param>
        public ErrorAdapter(object instance, ValidationFailure failure)
        {
            this.failure = failure;
            this.instance = instance;
        }

        /// <summary>
        /// Gets the failure details.
        /// </summary>
        /// <value>The failure.</value>
        public ValidationFailure Failure
        {
            get { return failure; }
        }

        /// <summary>
        /// Gets the invalid instance.
        /// </summary>
        /// <value>The instance.</value>
        public object Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Gets the name of the invalid property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string Key
        {
            get { return failure.PropertyName; }
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get { return failure.ErrorMessage; }
        }
    }
}