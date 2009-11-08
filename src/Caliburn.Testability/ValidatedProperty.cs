namespace Caliburn.Testability
{
    /// <summary>
    /// A property that has been processes by the validation engine.
    /// </summary>
    public class ValidatedProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatedProperty"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="fullPath">The full path.</param>
        public ValidatedProperty(IError error, string fullPath)
        {
            Error = error;
            FullPath = fullPath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatedProperty"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public ValidatedProperty(IError error)
            : this(error, string.Empty) {}

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>The error.</value>
        public IError Error { get; private set; }

        /// <summary>
        /// Gets or sets the full path.
        /// </summary>
        /// <value>The full path.</value>
        public string FullPath { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an error.
        /// </summary>
        /// <value><c>true</c> if this instance has an error; otherwise, <c>false</c>.</value>
        public bool HasError
        {
            get { return Error != null; }
        }
    }
}