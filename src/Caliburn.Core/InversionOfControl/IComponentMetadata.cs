namespace Caliburn.Core.InversionOfControl
{
    using System;

    /// <summary>
    /// Metadat the describes a component registration.
    /// </summary>
    public interface IComponentMetadata
    {
        /// <summary>
        /// Gets the component info.
        /// </summary>
        /// <param name="decoratedType">Type of the decorated.</param>
        /// <returns></returns>
        IComponentRegistration GetComponentInfo(Type decoratedType);
    }
}