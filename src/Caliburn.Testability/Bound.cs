namespace Caliburn.Testability
{
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// A factory for data bound items.
    /// </summary>
    public static class Bound
    {
        /// <summary>
        /// Creates a data bound element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="boundType">Type of the bound object.</param>
        /// <returns></returns>
        public static IBoundElement DependencyObject(DependencyObject element, BoundType boundType)
        {
            return DependencyObject(element, boundType, string.Empty);
        }

        /// <summary>
        /// Creates a data bound element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="boundType">Type of the bound object.</param>
        /// <param name="baseName">A base name.</param>
        /// <returns></returns>
		public static IBoundElement DependencyObject(DependencyObject element, BoundType boundType, string baseName)
        {
            return DependencyObject(element, boundType, baseName, true);
        }

        /// <summary>
        /// Creates a data bound element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="boundType">Type of the bound object.</param>
        /// <param name="baseName">A base name.</param>
        /// <param name="checkLogicalChildren">Indicate whether this elements children should be checked.</param>
        /// <returns></returns>
		public static IBoundElement DependencyObject(DependencyObject element, BoundType boundType, string baseName,
                                                bool checkLogicalChildren)
        {
            var dependencyObjectElement = new DependencyObjectElement(element, boundType, baseName)
            {
                CheckLogicalChildren = checkLogicalChildren
            };
            return dependencyObjectElement;
        }

        /// <summary>
        /// Creates a bound data template.
        /// </summary>
        /// <param name="dataTemplate">The data template.</param>
        /// <param name="boundType">Type of the bound.</param>
        /// <returns></returns>
		public static IBoundElement DataTemplate(DataTemplate dataTemplate, BoundType boundType)
        {
            return DataTemplate(dataTemplate, boundType, string.Empty);
        }

        /// <summary>
        /// Creates a bound data template.
        /// </summary>
        /// <param name="dataTemplate">The data template.</param>
        /// <param name="boundType">Type of the bound object.</param>
        /// <param name="baseName">A base name.</param>
        /// <returns></returns>
		public static IBoundElement DataTemplate(DataTemplate dataTemplate, BoundType boundType, string baseName)
        {
            if(!dataTemplate.IsSealed) dataTemplate.Seal();
            return new DataTemplateElement(dataTemplate, boundType, baseName);
        }

        /// <summary>
        /// Creates a bound control template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="boundType">Type of the bound object.</param>
        /// <param name="baseName">A base name.</param>
        /// <returns></returns>
		public static IBoundElement ControlTemplate(ControlTemplate template, BoundType boundType, string baseName)
        {
            return new ControlTemplateElement(template, boundType, baseName);
        }

        /// <summary>
        /// Creates a bound style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
		public static IBoundElement Style(Style style, BoundType type)
        {
            return Style(style, type, string.Empty);
        }

        /// <summary>
        /// Creates a bound style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public static IBoundElement Style(Style style, BoundType type, string name)
        {
            if(!style.IsSealed) style.Seal();
            return new StyleElement(style, type, name);
        }

        /// <summary>
        /// Creates a bound group style.
        /// </summary>
        /// <param name="groupStyle">The group style.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
		public static IBoundElement GroupStyle(GroupStyle groupStyle, BoundType type)
        {
            return GroupStyle(groupStyle, type, string.Empty);
        }

        /// <summary>
        /// Creates a bound group style.
        /// </summary>
        /// <param name="groupStyle">The group style.</param>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public static IBoundElement GroupStyle(GroupStyle groupStyle, BoundType type, string name)
        {
            return new GroupStyleElement(groupStyle, type, name);
        }

        /// <summary>
        /// Creates a bound enumerable.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="boundType">Type of the bound object.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public static IBoundElement Enumerable(IEnumerable enumerable, BoundType boundType, string name)
        {
            return new EnumerableElement(enumerable, boundType, name);
        }
    }
}