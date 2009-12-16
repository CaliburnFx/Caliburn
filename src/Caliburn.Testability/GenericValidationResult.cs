namespace Caliburn.Testability
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// A type specific implementation of <see cref="ValidationResult"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValidationResult<T> : ValidationResult
    {
        /// <summary>
        /// Determines if the property was bound.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public bool WasBoundTo<K>(Expression<Func<T, K>> property)
        {
			string propertyPath = ExpressionHelper.GetPathFromExpression(property);
            return WasBoundTo(propertyPath);
        }

        /// <summary>
        ///  Determines if the property was not bound.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public bool WasNotBoundTo<K>(Expression<Func<T, K>> property)
        {
            return !WasBoundTo(property);
        }

        /// <summary>
        /// Asserts that the validation process yielded no errors.
        /// </summary>
        public void AssertNoErrors()
        {
            if(HasErrors)
                throw new ValidationException(ErrorSummary);
        }

        /// <summary>
        /// Asserts that the specified property was bound.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="property">The property.</param>
        public void AssertWasBound<K>(Expression<Func<T, K>> property)
        {
            if(WasBoundTo(property)) return;

            throw new ValidationException(
                string.Format(
                    "No binding {0} on {1} was found.",
                    ExpressionHelper.GetPathFromExpression(property),
                    typeof(T).Name
                    )
                );
        }

        /// <summary>
        /// Asserts that the specified property was not bound.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="property">The property.</param>
        public void AssertWasNotBound<K>(Expression<Func<T, K>> property)
        {
            if(WasNotBoundTo(property)) return;

            throw new ValidationException(
                string.Format(
                    "The binding {0} on {1} was found, but should not exist.",
                    ExpressionHelper.GetPathFromExpression(property),
                    typeof(T).Name
                    )
                );
        }

      
    }
}