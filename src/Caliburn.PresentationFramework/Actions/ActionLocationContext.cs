namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using Filters;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Provides services and information for locating instances of <see cref="IAction"/>.
    /// </summary>
    public class ActionLocationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionLocationContext"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetFilters">The target filters.</param>
        public ActionLocationContext(IServiceLocator serviceLocator, Type targetType, IFilterManager targetFilters)
        {
            ServiceLocator = serviceLocator;
            TargetType = targetType;
            TargetFilters = targetFilters;
        }

        /// <summary>
        /// Gets or sets the type of the target.
        /// </summary>
        /// <value>The type of the target.</value>
        public Type TargetType { get; private set; }

        /// <summary>
        /// Gets or sets the target filters.
        /// </summary>
        /// <value>The target filters.</value>
        public IFilterManager TargetFilters { get; private set; }

        /// <summary>
        /// Gets or sets the service locator.
        /// </summary>
        /// <value>The service locator.</value>
        public IServiceLocator ServiceLocator { get; private set; }
    }
}