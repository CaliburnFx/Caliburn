namespace Caliburn.PresentationFramework.ViewModels
{
    using System;
    using Actions;
    using Core;
    using Filters;
    using Microsoft.Practices.ServiceLocation;

    public class DefaultViewModelDescriptionBuilder : IViewModelDescriptionBuilder
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly IActionLocator _actionLocator;

        public DefaultViewModelDescriptionBuilder(IServiceLocator serviceLocator, IActionLocator actionLocator)
        {
            _serviceLocator = serviceLocator;
            _actionLocator = actionLocator;
        }

        public IViewModelDescription Build(Type targetType)
        {
            var description = new DefaultViewModelDescription(targetType);
            var filters = new FilterManager(targetType, description, _serviceLocator);
            var actions = _actionLocator.Locate(new ActionLocationContext(_serviceLocator, targetType, filters));

            description.Filters = filters;
            actions.Apply(description.AddAction);

            return description;
        }
    }
}