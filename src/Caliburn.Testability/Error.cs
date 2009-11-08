namespace Caliburn.Testability
{
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// A factory for common error instances.
    /// </summary>
    public static class Error
    {
        /// <summary>
        /// Creates a "bad property" error.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="type">The type.</param>
        /// <param name="property">The property.</param>
        /// <param name="binding">The binding.</param>
        /// <returns></returns>
        public static IError BadProperty(IElement item, BoundType type, DependencyProperty property, Binding binding)
        {
            return new BindingError(
                item,
                type,
                property,
                binding,
                string.Format(
                    "[{0}] The property '{1}' was not found on '{2}'.",
                    item.Name,
                    binding.Path.Path,
                    type.Type.Name
                    )
                );
        }

        /// <summary>
        /// Creates an enumerable error.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="type">The type.</param>
        /// <param name="property">The property.</param>
        /// <param name="binding">The binding.</param>
        /// <returns></returns>
        public static IError NotEnumerable(IElement item, BoundType type, DependencyProperty property, Binding binding)
        {
            return new BindingError(
                item,
                type,
                property,
                binding,
                string.Format(
                    "[{0}] The property '{1}' on '{2}' is not an IEnumerable, which is required by {3} on {4}.",
                    item.Name,
                    binding.Path.Path,
                    type.Type.Name,
                    property.Name,
                    property.OwnerType.Name
                    )
                );
        }

        /// <summary>
        /// Creates a "bad data context" error.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="type">The type.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public static IError BadDataContext(IElement item, BoundType type, DependencyProperty property)
        {
            return new BindingError(
                item,
                type,
                property,
                null,
                string.Format(
                    "[{0}] The data context was not valid.",
                    item.Name
                    )
                );
        }

        /// <summary>
        /// Creates an ambiguous template error.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="type">The type.</param>
        /// <param name="templatePropertyName">The name of the template property.</param>
        /// <returns></returns>
        public static IError TemplateSelectorAmbiguity(IElement item, BoundType type, string templatePropertyName)
        {
            return new GeneralError(
                item,
                type,
                string.Format(
                    "[{0}] {1} ambiguity.  You can either set a TemplateSelector or a DataTemplate, but not both.",
                    item.Name,
                    templatePropertyName
                    )
                );
        }

        /// <summary>
        /// Creates an ambiguous style error.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="type">The type.</param>
        /// <param name="stylePropertyName">The name of the style property.</param>
        /// <returns></returns>
        public static IError StyleSelectorAmbiguity(IElement item, BoundType type, string stylePropertyName)
        {
            return new GeneralError(
                item,
                type,
                string.Format(
                    "[{0}] {1} ambiguity.  You can either set a StyleSelector or a Style, but not both.",
                    item.Name,
                    stylePropertyName
                    )
                );
        }
    }
}