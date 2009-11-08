namespace Caliburn.Core.Metadata
{
    using System;

    /// <summary>
    /// An attribute that gives directions to Caliburn concerning component registration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class RegisterAttribute : Attribute, IMetadata
    {
        /// <summary>
        /// Registers the type with the specified container.
        /// </summary>
        /// <param name="decoratedType">The decorated type.</param>
        public abstract ComponentInfo GetComponentInfo(Type decoratedType);
    }
}