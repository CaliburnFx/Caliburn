namespace Caliburn.ModelFramework
{
    using System;
    using System.Collections.Generic;
    using PresentationFramework;

    /// <summary>
    /// An implementation of <see cref="IPropertyDefinition{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyDefinition<T> : PropertyChangedBase, IPropertyDefinition<T>
    {
        private readonly Func<T> _defaultValue;
        private IList<object> _metadata;

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        public IList<object> Metadata
        {
            get
            {
                if (_metadata == null)
                    _metadata = new List<object>();

                return _metadata;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDefinition&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        public PropertyDefinition(string name, Func<T> defaultValue)
        {
            Name = name;
            _defaultValue = defaultValue;
        }

        /// <summary>
        /// Creates a property instance based on this defintion.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        public IProperty CreateInstance(IModel parent)
        {
            return new Property<T>(this, parent, _defaultValue());
        }
    }
}