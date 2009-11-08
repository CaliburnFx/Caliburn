namespace Caliburn.Testability
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Provides convenience methods for creating data binding validators.
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// Creates a validator for a <see cref="DependencyObject"/> and a <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="Element">The type of the element.</typeparam>
        /// <typeparam name="Type">The type of the ype.</typeparam>
        /// <returns></returns>
        public static BindingValidator<Type> For<Element, Type>()
            where Element : FrameworkElement
        {
            return For<Element, Type>(Activator.CreateInstance<Element>());
        }

        /// <summary>
        /// Creates a validator for a <see cref="DependencyObject"/> and a <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="Element">The type of the lement.</typeparam>
        /// <typeparam name="Type">The type of the ype.</typeparam>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static BindingValidator<Type> For<Element, Type>(Element element)
            where Element : DependencyObject
        {
            var boundType = new BoundType(typeof(Type));

            return new BindingValidator<Type>(
                Bound.DependencyObject(element, boundType)
                );
        }

        /// <summary>
        /// Creates a validator for a <see cref="DataTemplate"/>.
        /// </summary>
        /// <param name="dataTemplate">The data template.</param>
        /// <returns></returns>
        public static BindingValidator For(DataTemplate dataTemplate)
        {
            var dataType = dataTemplate.DataType as Type;
            var boundType = new BoundType(dataType);

            return new BindingValidator<Type>(
                Bound.DataTemplate(dataTemplate, boundType)
                );
        }

        /// <summary>
        /// Creates a validator for a <see cref="DataTemplate"/>.
        /// </summary>
        /// <typeparam name="Type">The type of the ype.</typeparam>
        /// <param name="dataTemplate">The data template.</param>
        /// <returns></returns>
        public static BindingValidator<Type> For<Type>(DataTemplate dataTemplate)
        {
            var boundType = new BoundType(typeof(Type));

            return new BindingValidator<Type>(
                Bound.DataTemplate(dataTemplate, boundType)
                );
        }

        /// <summary>
        /// Creates a validator for a <see cref="Style"/>.
        /// </summary>
        /// <typeparam name="Type">The type of the ype.</typeparam>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        public static BindingValidator<Type> For<Type>(Style style)
        {
            var boundType = new BoundType(typeof(Type));

            return new BindingValidator<Type>(
                Bound.Style(style, boundType)
                );
        }

        /// <summary>
        /// Creates a validator for a resource.
        /// </summary>
        /// <typeparam name="Resource">The type of the esource.</typeparam>
        /// <typeparam name="Type">The type of the ype.</typeparam>
        /// <param name="resourceKey">The resource key.</param>
        /// <returns></returns>
        public static BindingValidator<Type> For<Resource, Type>(object resourceKey)
            where Resource : ResourceDictionary
        {
            var resourceDictionary = Activator.CreateInstance<Resource>();
            return For<Type>(resourceDictionary, resourceKey);
        }

        /// <summary>
        /// Creates a validator for a resource.
        /// </summary>
        /// <typeparam name="Type">The type of the ype.</typeparam>
        /// <param name="resourceDictionary">The resource dictionary.</param>
        /// <param name="resourceKey">The resource key.</param>
        /// <returns></returns>
        public static BindingValidator<Type> For<Type>(ResourceDictionary resourceDictionary, object resourceKey)
        {
            var method = resourceDictionary.GetType().GetMethod("InitializeComponent");

            if(method != null)
                method.Invoke(resourceDictionary, null);

            var resource = resourceDictionary[resourceKey];
            var boundType = new BoundType(typeof(Type));

            if(resource is DataTemplate)
                return new BindingValidator<Type>(
                    Bound.DataTemplate(resource as DataTemplate, boundType)
                    );

            if(resource is Style)
                return new BindingValidator<Type>(
                    Bound.Style(resource as Style, boundType)
                    );

            if(resource is GroupStyle)
                return new BindingValidator<Type>(
                    Bound.GroupStyle(resource as GroupStyle, boundType)
                    );

            if(resource is DependencyObject)
                return new BindingValidator<Type>(
                    Bound.DependencyObject(resource as DependencyObject, boundType)
                    );

            throw new NotSupportedException(
                string.Format(
                    "Resources of type '{0}' are not supported",
                    resource.GetType().FullName
                    )
                );
        }
    }
}