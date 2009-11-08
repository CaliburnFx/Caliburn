namespace Caliburn.Testability
{
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Represents information needed to validate a data binding.
    /// </summary>
    public class BindingInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingInfo"/> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="property">The property.</param>
        public BindingInfo(Binding binding, DependencyProperty property)
        {
            Binding = binding;
            Property = property;
        }

        /// <summary>
        /// Gets or sets the binding.
        /// </summary>
        /// <value>The binding.</value>
        public Binding Binding { get; private set; }

        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        /// <value>The property.</value>
        public DependencyProperty Property { get; private set; }

        /// <summary>
        /// Determines whether this information needs to be validated.
        /// </summary>
        /// <returns></returns>
        public bool ShouldBeValidated()
        {
            return Binding.ShouldValidate();
        }
    }
}