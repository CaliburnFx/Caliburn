namespace Caliburn.PresentationFramework.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Actions;
    using Conventions;
    using Core;
    using Core.InversionOfControl;
    using Core.Logging;
    using Filters;

    /// <summary>
    /// The default implementation of <see cref="IViewModelDescriptionFactory"/>.
    /// </summary>
    public class DefaultViewModelDescriptionFactory : IViewModelDescriptionFactory
    {
        static readonly ILog Log = LogManager.GetLog(typeof(DefaultViewModelDescriptionFactory));

        readonly IServiceLocator serviceLocator;
        readonly IActionLocator actionLocator;
        readonly IConventionManager conventionManager;
        readonly Dictionary<Type, IViewModelDescription> cache = new Dictionary<Type, IViewModelDescription>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewModelDescriptionFactory"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="actionLocator">The action locator.</param>
        /// <param name="conventionManager">The convention manager.</param>
        public DefaultViewModelDescriptionFactory(IServiceLocator serviceLocator, IActionLocator actionLocator, IConventionManager conventionManager)
        {
            this.serviceLocator = serviceLocator;
            this.actionLocator = actionLocator;
            this.conventionManager = conventionManager;
        }

        /// <summary>
        /// Creates a description based on the target type.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>A view model description.</returns>
        public IViewModelDescription Create(Type targetType)
        {
            IViewModelDescription description;

            if(!cache.TryGetValue(targetType, out description))
            {
                description = CreateCore(targetType);
                cache[targetType] = description;
                Log.Info("Created and cached view model description for {0}", targetType);
            }

            return description;
        }

        /// <summary>
        /// Creates the actual description.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns></returns>
        protected virtual IViewModelDescription CreateCore(Type targetType) 
        {
            var customFactory = targetType
                .GetAttributes<IViewModelDescriptionFactory>(true)
                .FirstOrDefault();

            if (customFactory != null)
                return customFactory.Create(targetType);

            var description = new DefaultViewModelDescription(conventionManager, targetType);
            var filters = new FilterManager(targetType, description.TargetType, serviceLocator);
            var actions = actionLocator.Locate(new ActionLocationContext(serviceLocator, targetType, filters));

            description.Filters = filters;
            actions.Apply(description.AddAction);

            return description;
        }
    }
}