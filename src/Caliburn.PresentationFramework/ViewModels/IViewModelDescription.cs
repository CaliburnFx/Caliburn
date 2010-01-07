namespace Caliburn.PresentationFramework.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows;
    using Actions;

    /// <summary>
    /// Describes a View Model.
    /// </summary>
    public interface IViewModelDescription : IActionHost
    {
        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        IEnumerable<PropertyInfo> Properties { get; }

        /// <summary>
        /// Sets the conventions for a particualr view type.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="applicableConventions">The applicable conventions.</param>
        void SetConventionsFor(Type viewType, IEnumerable<IViewApplicable> applicableConventions);

        /// <summary>
        /// Gets the conventions for the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The applicable conventions.</returns>
        IEnumerable<IViewApplicable> GetConventionsFor(DependencyObject view);
    }
}