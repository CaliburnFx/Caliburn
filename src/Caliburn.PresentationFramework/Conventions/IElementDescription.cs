namespace Caliburn.PresentationFramework.Conventions
{
    using System;

    /// <summary>
    /// Describes an element in the Logical or Visual Treen.
    /// </summary>
    public interface IElementDescription
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        Type Type { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the conventions associated with the element.
        /// </summary>
        /// <value>The convention.</value>
        IElementConvention Convention { get; }
    }
}