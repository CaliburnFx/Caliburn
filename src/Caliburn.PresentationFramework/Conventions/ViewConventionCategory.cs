namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ViewModels;
    using Views;

    /// <summary>
    /// The default implemenation of <see cref="IViewConventionCategory"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ViewConventionCategory<T> : IViewConventionCategory
    {
        private readonly Func<IViewModelDescription, IEnumerable<T>> getTargets;
        private readonly List<IViewConvention<T>> conventions = new List<IViewConvention<T>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewConventionCategory{T}"/> class.
        /// </summary>
        /// <param name="getTargets">The get targets.</param>
        public ViewConventionCategory(Func<IViewModelDescription, IEnumerable<T>> getTargets)
        {
            this.getTargets = getTargets;
        }

        /// <summary>
        /// Gets the applications.
        /// </summary>
        /// <param name="conventionManager">The convention manager.</param>
        /// <param name="viewModelDescription">The view model description.</param>
        /// <param name="elementDescription">The element description.</param>
        /// <returns>The applications.</returns>
        public IEnumerable<IViewApplicable> GetApplications(IConventionManager conventionManager, IViewModelDescription viewModelDescription, ElementDescription elementDescription)
        {
            return from convention in conventions
                   from target in getTargets(viewModelDescription)
                   let application = convention.TryCreateApplication(conventionManager, viewModelDescription, elementDescription, target)
                   where application != null
                   select application;
        }

        /// <summary>
        /// Adds the convention to this category.
        /// </summary>
        /// <param name="convention">The convention.</param>
        public void AddConvention(IViewConvention<T> convention)
        {
            conventions.Add(convention);
        }
    }
}