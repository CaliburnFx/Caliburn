namespace Caliburn.Testability
{
	using System;

	/// <summary>
    /// Validates bindings on a data bound item.
    /// </summary>
    public class BindingValidator
    {
		private readonly IBoundElement _element;
        private readonly ElementEnumerator _enumerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingValidator"/> class.
        /// </summary>
        /// <param name="element">The data bound item.</param>
        public BindingValidator(IBoundElement element)
        {
			_element = element;
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
		/// Add a type hint for a property path, specifying the actual type returned 
		/// by the last property of the property path chain
		/// </summary>
		/// <param name="propertyPath">The property path.</param>
		/// <param name="hint">The Type actually returned.</param>
		/// <returns></returns>
		public BindingValidator WithHint(string propertyPath, Type hint)
		{
			_element.Type.AddHint(propertyPath, hint);
			return this;
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