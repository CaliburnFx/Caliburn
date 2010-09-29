namespace Caliburn.PresentationFramework.Conventions
{
    using System;

    /// <summary>
    /// Describes and element that is available for convention application.
    /// </summary>
    public class ElementDescription
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name;

        /// <summary>
        /// Gets or sets the conventions associated with the element.
        /// </summary>
        /// <value>The convention.</value>
        public IElementConvention Convention;
    }
}