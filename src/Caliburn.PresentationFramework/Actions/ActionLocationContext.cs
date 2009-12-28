namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using Filters;
    using Microsoft.Practices.ServiceLocation;

    public class ActionLocationContext
    {
        public ActionLocationContext(IServiceLocator serviceLocator, Type targetType, IFilterManager targetFilters)
        {
            ServiceLocator = serviceLocator;
            TargetType = targetType;
            TargetFilters = targetFilters;
        }

        public Type TargetType { get; private set; }
        public IFilterManager TargetFilters { get; private set; }
        public IServiceLocator ServiceLocator { get; private set; }
    }
}