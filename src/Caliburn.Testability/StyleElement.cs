namespace Caliburn.Testability
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Represents a data bound <see cref="Style"/>.
    /// </summary>
	public class StyleElement : IBoundElement
    {
        readonly Style style;
        readonly BoundType type;
        readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleElement"/> class.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="boundType">Type of the bound.</param>
        /// <param name="name">The name.</param>
        internal StyleElement(Style style, BoundType boundType, string name)
        {
            this.style = style;
            type = boundType;
            this.name = name + " [Style] ";
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the type the item is bound to.
        /// </summary>
        /// <value>The type.</value>
        public BoundType Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <value>The style.</value>
        public Style Style
        {
            get { return style; }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public IEnumerable<IElement> GetChildren(ElementEnumeratorSettings settings)
        {
            foreach(var setterBase in style.Setters)
            {
                var setter = setterBase as Setter;

                if(setter == null) continue;
                if(setter.Value is BindingBase) continue;

                var dataBound = settings.GetBoundProperty(setter.Value, Type, Name);

                if(dataBound != null)
                    yield return dataBound;
            }
        }

        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}