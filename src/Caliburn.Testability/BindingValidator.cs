namespace Caliburn.Testability
{
    /// <summary>
    /// Validates bindings on a data bound item.
    /// </summary>
    public class BindingValidator
    {
        private readonly ElementEnumerator _enumerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingValidator"/> class.
        /// </summary>
        /// <param name="element">The data bound item.</param>
        public BindingValidator(IElement element)
        {
            _enumerator = new ElementEnumerator(element);
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public ElementEnumeratorSettings Settings
        {
            get { return _enumerator.Settings; }
        }

        /// <summary>
        /// Validates the bound item.
        /// </summary>
        /// <returns></returns>
        public virtual ValidationResult Validate()
        {
            var visitor = new ValidationVisitor();

            _enumerator.Enumerate(visitor);

            return visitor.Result;
        }
    }
}