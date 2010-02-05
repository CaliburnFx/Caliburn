namespace Caliburn.Core.IoC
{
    using System;
    using Metadata;

    /// <summary>
    /// Metadat the describes a component registration.
    /// </summary>
    public interface IComponentMetadata : IMetadata
    {
        /// <summary>
        /// Gets the component info.
        /// </summary>
        /// <param name="decoratedType">Type of the decorated.</param>
        /// <returns></returns>
        IComponentRegistration GetComponentInfo(Type decoratedType);
    }
}