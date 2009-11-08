namespace Caliburn.ModelFramework
{
    using Core.Metadata;

    /// <summary>
    /// A definition of a property.
    /// </summary>
    public interface IPropertyDefinition : IMetadataContainer
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Creates a property instance based on this defintion.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        IProperty CreateInstance(IModel parent);
    }

    /// <summary>
    /// A strongly type version of <see cref="IModel"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPropertyDefinition<T> : IPropertyDefinition {}
}