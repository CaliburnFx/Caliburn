namespace Caliburn.Testability
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;

    /// <summary>
    /// An implementation of <see cref="IElement"/> for <see cref="GroupStyle"/>.
    /// </summary>
	public class GroupStyleElement : IBoundElement
    {
        private readonly GroupStyle _style;
        private readonly BoundType _type;
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleElement"/> class.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="boundType">Type of the bound.</param>
        /// <param name="name">The name.</param>
        internal GroupStyleElement(GroupStyle style, BoundType boundType, string name)
        {
            _style = style;
            _type = boundType;
            _name = name + " [GroupStyle] ";
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
        /// Gets the style.
        /// </summary>
        /// <value>The style.</value>
        public GroupStyle Style
        {
            get { return _style; }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public IEnumerable<IElement> GetChildren(ElementEnumeratorSettings settings)
        {
            if(Type == null) yield break;

            if(_style.ContainerStyle != null)
            {
                var target = _style.ContainerStyle.TargetType != null
                                 ? new BoundType(_style.ContainerStyle.TargetType)
                                 : new BoundType(typeof(GroupItem));

                yield return Bound.Style(_style.ContainerStyle, target, Name);
            }

            if(_style.HeaderTemplate != null)
            {
                var target = _style.HeaderTemplate.DataType as Type != null
                                 ? new BoundType(_style.HeaderTemplate.DataType as Type)
                                 : new BoundType(typeof(GroupItem));

                yield return Bound.DataTemplate(_style.HeaderTemplate, target, Name);
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