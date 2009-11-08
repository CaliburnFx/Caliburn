namespace Caliburn.ModelFramework
{
    using System;
    using System.Collections.Generic;
    using Core.Metadata;

    /// <summary>
    /// Implemented by presentation model definitions.
    /// </summary>
    public interface IModelDefinition : IMetadataContainer, IEnumerable<IPropertyDefinition>
    {
        /// <summary>
        /// Creates an instance of <see cref="IModel"/> that matches this definition.
        /// </summary>
        /// <returns></returns>
        IModel CreateInstance();

        /// <summary>
        /// Adds the property to the definition.
        /// </summary>
        /// <param name="propertyDefinition">The property definition.</param>
        void AddProperty(IPropertyDefinition propertyDefinition);

        /// <summary>
        /// Adds the property to the definition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        IPropertyDefinition<T> AddProperty<T>(string propertyName, Func<T> defaultValue);
    }
}