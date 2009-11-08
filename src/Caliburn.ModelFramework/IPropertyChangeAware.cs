namespace Caliburn.ModelFramework
{
    using Core.Metadata;

    /// <summary>
    /// Implemented by classes that want to be aware of property value changes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPropertyChangeAware<T> : IMetadata
    {
        /// <summary>
        /// Interrogates the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        bool Interrogate(IProperty<T> instance);
    }
}