namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Core.Invocation;
    using Filters;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The default implementation of <see cref="IActionLocator"/>.
    /// </summary>
    public class DefaultActionLocator : IActionLocator
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly IMethodFactory _methodFactory;
        private readonly IMessageBinder _messageBinder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultActionLocator"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="methodFactory">The method factory.</param>
        /// <param name="messageBinder">The message binder.</param>
        public DefaultActionLocator(IServiceLocator serviceLocator, IMethodFactory methodFactory, IMessageBinder messageBinder)
        {
            _serviceLocator = serviceLocator;
            _methodFactory = methodFactory;
            _messageBinder = messageBinder;
        }

        /// <summary>
        /// Locates actions for the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Discovered actions.</returns>
        public IEnumerable<IAction> Locate(ActionLocationContext context)
        {
            var actions = new List<IAction>();
            var methodGroups = SelectMethods(context.TargetType);

            foreach(var methodGroup in methodGroups)
            {
                var methodList = methodGroup.ToList();

                if(methodList.Count == 1)
                    actions.Add(CreateAction(context.TargetType, context.TargetFilters, methodList[0]));
                else
                {
                    var overloadedAction = new OverloadedAction(methodGroup.Key);

                    foreach(var methodInfo in methodList)
                    {
                        overloadedAction.AddOverload(CreateAction(context.TargetType, context.TargetFilters, methodInfo));
                    }

                    actions.Add(overloadedAction);
                }
            }

            return actions;
        }

        /// <summary>
        /// Selects the methods approprate for actions.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>The action appropriate methods.</returns>
        protected virtual IEnumerable<IGrouping<string, MethodInfo>> SelectMethods(Type targetType)
        {
            return
                from method in targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                where method.DeclaringType != typeof(object)
                      && !method.ContainsGenericParameters
                      && !method.Name.StartsWith("get_")
                      && !method.Name.StartsWith("set_")
                      && !method.Name.StartsWith("remove_")
                      && !method.Name.StartsWith("add_")
                      && method.GetParameters().All(x => !x.IsOut)
                group method by method.Name
                into groups
                    select groups;
        }

        /// <summary>
        /// Creates the action.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetFilters">The target filters.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <returns>The action.</returns>
        protected virtual IAction CreateAction(Type targetType, IFilterManager targetFilters, MethodInfo methodInfo)
        {
            var builder = methodInfo.GetCustomAttributes(typeof(IActionFactory), true)
                              .OfType<IActionFactory>()
                              .FirstOrDefault() ?? new ActionAttribute();

            return builder.Create(
                new ActionCreationContext(
                    _serviceLocator,
                    _methodFactory,
                    _messageBinder,
                    targetType,
                    targetFilters,
                    methodInfo
                    )
                );
        }
    }
}