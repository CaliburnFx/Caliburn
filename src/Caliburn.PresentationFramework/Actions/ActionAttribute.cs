namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using Filters;

    public class ActionAttribute : Attribute, IActionBuilder
    {
        public IAction Build(ActionBuildingContext context)
        {
            var method = context.MethodFactory
                .CreateFrom(context.Method);

            var action = new SynchronousAction(
                method,
                context.MessageBinder,
                new FilterManager(context.TargetType, method, context.ServiceLocator).Combine(context.TargetFilters)
                );

            context.ApplyActionFilterConventions(action, method);

            return action;
        }
    }
}