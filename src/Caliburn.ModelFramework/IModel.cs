namespace Caliburn.ModelFramework
{
    using System.Collections.Generic;

    /// <summary>
    /// Implemented by presentation model instances.
    /// </summary>
    public interface IModel : IModelNode, ISupportInterrogation, IEnumerable<IProperty>
    {
        /// <summary>
        /// Gets the definition.
        /// </summary>
        /// <value>The definition.</value>
        IModelDefinition Definition { get; }

        /// <summary>
        /// Gets the <see cref="IProperty"/> with the specified property name.
        /// </summary>
        /// <value></value>
        IProperty this[string propertyName] { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="definition">The definition.</param>
        /// <returns></returns>
        K GetValue<K>(IPropertyDefinition<K> definition);

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="definition">The definition.</param>
        /// <param name="value">The value.</param>
        void SetValue<K>(IPropertyDefinition<K> definition, K value);
    }
}