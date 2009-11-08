namespace Caliburn.ModelFramework
{
    using System;
    using System.Collections.Generic;
    using Core.Metadata;

    /// <summary>
    /// Implemented by models that support interrogation.
    /// </summary>
    public interface ISupportInterrogation
    {
        /// <summary>
        /// Gets the validation results.
        /// </summary>
        /// <value>The validation results.</value>
        IList<IValidationResult> ValidationResults { get; }

        /// <summary>
        /// Uses the interrogators.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="borrower">The borrower.</param>
        void UseInterrogators<T>(Action<IEnumerable<T>> borrower) where T : IMetadata;
    }
}