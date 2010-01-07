namespace Caliburn.Testability
{
	using System;
	using System.Linq.Expressions;

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
		public BindingValidator(IBoundElement element)
			: base(element) { }

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
		
		/// <summary>
        /// Add a type hint for a property path, specifying the actual type returned 
		/// by the last property of the property path chain
        /// </summary>
        /// <typeparam name="K"></typeparam>
		/// <param name="property">The property</param>
		/// <param name="hint">The Type actually returned.</param>
        /// <returns></returns>
		public BindingValidator<T> WithHint<K>(Expression<Func<T, K>> property, Type hint)
		{
			WithHint(ExpressionHelper.GetPathFromExpression(property), hint);
			return this;
		}
    }
}