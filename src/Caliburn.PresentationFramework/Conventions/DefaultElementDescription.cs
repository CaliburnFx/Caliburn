namespace Caliburn.PresentationFramework.Conventions
{
    using System;

    /// <summary>
    /// The default implementation of <see cref="IElementDescription"/>.
    /// </summary>
    public class DefaultElementDescription : IElementDescription
    {
        private readonly Type _type;
        private readonly string _name;
        private readonly IElementConvention _convention;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultElementDescription"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="convention">The convention.</param>
        public DefaultElementDescription(Type type, string name, IElementConvention convention)
        {
            _type = type;
            _name = name;
            _convention = convention;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the conventions associated with the element.
        /// </summary>
        /// <value>The convention.</value>
        public IElementConvention Convention
        {
            get { return _convention; }
        }
    }
}