namespace Caliburn.Testability
{
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Represents a binding related error.
    /// </summary>
    public class BindingError : IError
    {
        readonly IElement element;
        readonly BoundType type;
        readonly DependencyProperty property;
        readonly Binding binding;
        readonly string message;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingError"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="type">The type.</param>
        /// <param name="property">The property.</param>
        /// <param name="binding">The binding.</param>
        /// <param name="message">The message.</param>
        public BindingError(IElement item, BoundType type, DependencyProperty property, Binding binding, string message)
        {
            element = item;
            this.type = type;
            this.property = property;
            this.binding = binding;
            this.message = message;
        }

        /// <summary>
        /// Gets the item that yielded the binding error.
        /// </summary>
        /// <value>The item.</value>
        public IElement Element
        {
            get { return element; }
        }

        /// <summary>
        /// Gets the type that was bound to.
        /// </summary>
        /// <value>The type.</value>
        public BoundType Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the property on the UI element that was bound.
        /// </summary>
        /// <value>The property.</value>
        public DependencyProperty Property
        {
            get { return property; }
        }

        /// <summary>
        /// Gets the binding.
        /// </summary>
        /// <value>The binding.</value>
        public Binding Binding
        {
            get { return binding; }
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get { return message; }
        }
    }
}