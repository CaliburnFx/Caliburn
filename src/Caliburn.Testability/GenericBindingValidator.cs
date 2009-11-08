namespace Caliburn.Testability
{
    /// <summary>
    /// A type specific implementation of <see cref="BindingValidator"/>,
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindingValidator<T> : BindingValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingValidator&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="element">The data bound item.</param>
        public BindingValidator(IElement element)
            : base(element) {}

        /// <summary>
        /// Validates the bound item.
        /// </summary>
        /// <returns></returns>
        public new ValidationResult<T> Validate()
        {
            var result = base.Validate();
            var typedResult = new ValidationResult<T>();

            typedResult.Add(result);

            return typedResult;
        }
    }
}