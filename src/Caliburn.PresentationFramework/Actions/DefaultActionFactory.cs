namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Core.Invocation;
    using Core.Metadata;
    using Filters;

    /// <summary>
    /// An implementation of <see cref="IActionFactory"/>.
    /// </summary>
    public class DefaultActionFactory : IActionFactory
    {
        private readonly IMethodFactory _methodFactory;
        private readonly IMessageBinder _messageBinder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultActionFactory"/> class.
        /// </summary>
        /// <param name="methodFactory">The method factory.</param>
        /// <param name="messageBinder">The parameter binder used by actions.</param>
        public DefaultActionFactory(IMethodFactory methodFactory, IMessageBinder messageBinder)
        {
            _methodFactory = methodFactory;
            _messageBinder = messageBinder;
        }

        /// <summary>
        /// Creates actions for the provided host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        public IEnumerable<IAction> CreateFor(IActionHost host)
        {
            var actions = new List<IAction>();
            var methodGroups = SelectMethods(host.TargetType);

            foreach(var methodGroup in methodGroups)
            {
                var methodList = methodGroup.ToList();

                if(methodList.Count == 1)
                    actions.Add(CreateAction(host, methodList[0]));
                else
                {
                    var overloadedAction = new OverloadedAction(methodGroup.Key);

                    foreach(var methodInfo in methodList)
                    {
                        overloadedAction.AddOverload(CreateAction(host, methodInfo));
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
        /// <returns></returns>
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
        /// <param name="host">The host.</param>
        /// <param name="methodInfo">The method info.</param>
        /// <returns></returns>
        protected virtual IAction CreateAction(IActionHost host, MethodInfo methodInfo)
        {
            var method = _methodFactory.CreateFrom(methodInfo);
            var asyncAtt = method.GetMetadata<AsyncActionAttribute>();

            var filters = host.GetFilterManager(method);

            TryAddCanExecute(filters, method);

            if(asyncAtt == null)
                return new SynchronousAction(method, _messageBinder, filters);
            return new AsynchronousAction(method, _messageBinder, filters, asyncAtt.BlockInteraction);
        }

        /// <summary>
        /// Searches for a "CanExecute" method and add the Preview if found.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <param name="method">The method.</param>
        protected void TryAddCanExecute(IFilterManager manager, IMethod method)
        {
            var found = method.FindMetadata<PreviewAttribute>()
                .Where(x => x.MethodName == "Can" + method.Info.Name
                );

            if(found.Count() > 0) return;

            var canExecute = method.Info.DeclaringType.GetMethod(
                                 "Can" + method.Info.Name,
                                 method.Info.GetParameters().Select(x => x.ParameterType).ToArray()
                                 )
                             ?? method.Info.DeclaringType.GetMethod("get_Can" + method.Info.Name);

            if(canExecute != null)
                manager.Add(new PreviewAttribute(_methodFactory.CreateFrom(canExecute)));
        }
    }
}