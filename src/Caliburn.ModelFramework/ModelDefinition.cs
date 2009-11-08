namespace Caliburn.ModelFramework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Core.Metadata;

    /// <summary>
    /// An implementation of <see cref="IModelDefinition"/>.
    /// </summary>
    public class ModelDefinition : MetadataContainer, IModelDefinition
    {
        private readonly Dictionary<string, IPropertyDefinition> _propertyDefinitions =
            new Dictionary<string, IPropertyDefinition>();

        /// <summary>
        /// Creates an instance of <see cref="IModel"/> that matches this definition.
        /// </summary>
        /// <returns></returns>
        public IModel CreateInstance()
        {
            return new ModelBase(this);
        }

        /// <summary>
        /// Adds the property to the definition.
        /// </summary>
        /// <param name="propertyDefinition">The property definition.</param>
        public void AddProperty(IPropertyDefinition propertyDefinition)
        {
            _propertyDefinitions[propertyDefinition.Name] = propertyDefinition;
        }

        /// <summary>
        /// Adds the property to the definition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public IPropertyDefinition<T> AddProperty<T>(string propertyName, Func<T> defaultValue)
        {
            var model = new PropertyDefinition<T>(propertyName, defaultValue);
            _propertyDefinitions[propertyName] = model;
            return model;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IPropertyDefinition> GetEnumerator()
        {
            return _propertyDefinitions.Values.GetEnumerator();
        }
    }
}