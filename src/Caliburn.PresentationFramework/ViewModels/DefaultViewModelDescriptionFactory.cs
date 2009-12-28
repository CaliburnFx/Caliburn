namespace Caliburn.PresentationFramework.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Actions;
    using Core;
    using Filters;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The default implementation of <see cref="IViewModelDescriptionFactory"/>.
    /// </summary>
    public class DefaultViewModelDescriptionFactory : IViewModelDescriptionFactory
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly IActionLocator _actionLocator;
        private readonly Dictionary<Type, IViewModelDescription> _cache = new Dictionary<Type, IViewModelDescription>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewModelDescriptionFactory"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="actionLocator">The action locator.</param>
        public DefaultViewModelDescriptionFactory(IServiceLocator serviceLocator, IActionLocator actionLocator)
        {
            _serviceLocator = serviceLocator;
            _actionLocator = actionLocator;
        }

        /// <summary>
        /// Creates a description based on the target type.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>A view model description.</returns>
        public IViewModelDescription Create(Type targetType)
        {
            IViewModelDescription description;

            if(!_cache.TryGetValue(targetType, out description))
            {
                description = CreateCore(targetType);
                _cache[targetType] = description;
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
            var customFactory = targetType.GetCustomAttributes(typeof(IViewModelDescriptionFactory), true)
                .OfType<IViewModelDescriptionFactory>()
                .FirstOrDefault();

            if (customFactory != null)
                return customFactory.Create(targetType);

            var description = new DefaultViewModelDescription(targetType);
            var filters = new FilterManager(targetType, description, _serviceLocator);
            var actions = _actionLocator.Locate(new ActionLocationContext(_serviceLocator, targetType, filters));

            description.Filters = filters;
            actions.Apply(description.AddAction);

            return description;
        }
    }
}