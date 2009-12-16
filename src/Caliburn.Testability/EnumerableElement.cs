namespace Caliburn.Testability
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of <see cref="IElement"/> that handles enumerables.
    /// </summary>
	public class EnumerableElement : IBoundElement
    {
        private readonly IEnumerable _enumerable;
        private readonly BoundType _type;
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableElement"/> class.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="boundType">Type of the bound.</param>
        /// <param name="name">The name.</param>
        internal EnumerableElement(IEnumerable enumerable, BoundType boundType, string name)
        {
            _enumerable = enumerable;
            _type = boundType;
            _name = name;
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
        /// Gets the type the item is bound to.
        /// </summary>
        /// <value>The type.</value>
        public BoundType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <value>The children.</value>
        public IEnumerable<IElement> GetChildren(ElementEnumeratorSettings settings)
        {
            foreach(var item in _enumerable)
            {
                var element = settings.GetBoundProperty(item, Type, Name);
                if(element != null) yield return element;
            }
        }

        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public void Accept(IElementVisitor visitor) {}
    }
}