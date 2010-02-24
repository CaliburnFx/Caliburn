namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Data;
    using Actions;
    using Core.Invocation;
    using ViewModels;

    /// <summary>
    /// Implemented by services that understand conventions.
    /// </summary>
    public interface IConventionManager
    {
        /// <summary>
        /// Adds the element convention.
        /// </summary>
        /// <param name="convention">The convention.</param>
        void AddElementConvention(IElementConvention convention);

        /// <summary>
        /// Adds the view convention category.
        /// </summary>
        /// <param name="conventionCategory">The convention category.</param>
        void AddViewConventions(IViewConventionCategory conventionCategory);

        /// <summary>
        /// Gets the element convention for the type of element specified.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <returns>The convention.</returns>
        IElementConvention GetElementConvention(Type elementType);

        /// <summary>
        /// Adds the value converter convention.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        /// <param name="converter">The converter.</param>
        void AddConverterConvention(DependencyProperty target, Type source, IValueConverter converter);

        /// <summary>
        /// Gets the conventional value converter.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        /// <returns>The converter or null if none is defined.</returns>
        IValueConverter GetValueConverter(DependencyProperty target, Type source);

        /// <summary>
        /// Determines the conventions for a view model and a set of UI elements.
        /// </summary>
        /// <param name="viewModelDescription">The view model description.</param>
        /// <param name="elementDescriptions">The element descriptions.</param>
        /// <returns>The applicable conventions.</returns>
        IEnumerable<IViewApplicable> DetermineConventions(IViewModelDescription viewModelDescription, IEnumerable<IElementDescription> elementDescriptions);

        /// <summary>
        /// Applies the action creation conventions to the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="targetMethod">The target method.</param>
        void ApplyActionCreationConventions(IAction action, IMethod targetMethod);
    }
}