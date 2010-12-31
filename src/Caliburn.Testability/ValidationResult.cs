namespace Caliburn.Testability
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The results of a validation operation.
    /// </summary>
    public class ValidationResult
    {
        readonly List<ValidatedProperty> validatedProperties = new List<ValidatedProperty>();
        readonly List<IError> errors = new List<IError>();

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrors
        {
            get { return Errors.Count() > 0; }
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>The errors.</value>
        public IEnumerable<IError> Errors
        {
            get
            {
                return (from prop in validatedProperties
                        where prop.HasError
                        select prop.Error).Concat(errors);
            }
        }

        /// <summary>
        /// Gets the error summary.
        /// </summary>
        /// <value>The error summary.</value>
        public string ErrorSummary
        {
            get { return Errors.Aggregate(string.Empty, (agg, cur) => agg + cur.Message + "\n"); }
        }

        /// <summary>
        /// Gets the bound properties.
        /// </summary>
        /// <value>The bound properties.</value>
        public IEnumerable<string> BoundProperties
        {
            get
            {
                return from prop in validatedProperties
                       where !prop.HasError && !string.IsNullOrEmpty(prop.FullPath)
                       select prop.FullPath;
            }
        }

        /// <summary>
        /// Adds the specified validated property.
        /// </summary>
        /// <param name="validatedProperty">The validated property.</param>
        public void Add(ValidatedProperty validatedProperty)
        {
            validatedProperties.Add(validatedProperty);
        }

        /// <summary>
        /// Adds the specified validation result.
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        public void Add(ValidationResult validationResult)
        {
            validatedProperties.AddRange(validationResult.validatedProperties);
            errors.AddRange(validationResult.errors);
        }

        /// <summary>
        /// Adds the specified error.
        /// </summary>
        /// <param name="error">The error.</param>
        public void Add(IError error)
        {
            errors.Add(error);
        }

        /// <summary>
        /// Determines if the property was bound.
        /// </summary>
        /// <param name="fullPropertyPath">The full property path.</param>
        /// <returns></returns>
        public bool WasBoundTo(string fullPropertyPath)
        {
            return BoundProperties.Contains(fullPropertyPath);
        }
    }
}