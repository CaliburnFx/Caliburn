namespace Caliburn.PresentationFramework.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows;
    using Actions;
    using Conventions;

    /// <summary>
    /// Describes a View Model.
    /// </summary>
    public interface IViewModelDescription : IActionHost
    {
        IEnumerable<PropertyInfo> Properties { get; }

        void SetConventionsFor(Type viewType, IEnumerable<IViewApplicable> applicableConventions);
        IEnumerable<IViewApplicable> GetConventionsFor(DependencyObject view);
    }
}