namespace Caliburn.PresentationFramework.Filters
{
    using System;
    using Core.Metadata;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// An <see cref="IFilter"/> that requires initialization.
    /// </summary>
    public interface IInitializable : IFilter
    {
        /// <summary>
        /// Initializes the filter.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="metadataContainer">The metadata container.</param>
        /// <param name="serviceLocator">The serviceLocator.</param>
        void Initialize(Type targetType, IMetadataContainer metadataContainer, IServiceLocator serviceLocator);
    }
}