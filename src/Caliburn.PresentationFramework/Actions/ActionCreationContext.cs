namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Core.Invocation;
    using Filters;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Provides services and information during creation of an <see cref="IAction"/>.
    /// </summary>
    public class ActionCreationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionCreationContext"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="methodFactory">The method factory.</param>
        /// <param name="messageBinder">The message binder.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetFilters">The target filters.</param>
        /// <param name="method">The method.</param>
        public ActionCreationContext(IServiceLocator serviceLocator, IMethodFactory methodFactory,
                                     IMessageBinder messageBinder, Type targetType, IFilterManager targetFilters,
                                     MethodInfo method)
        {
            ServiceLocator = serviceLocator;
            MethodFactory = methodFactory;
            MessageBinder = messageBinder;
            TargetType = targetType;
            TargetFilters = targetFilters;
            Method = method;
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
        /// Gets or sets the method.
        /// </summary>
        /// <value>The method.</value>
        public MethodInfo Method { get; private set; }

        /// <summary>
        /// Gets or sets the service locator.
        /// </summary>
        /// <value>The service locator.</value>
        public IServiceLocator ServiceLocator { get; private set; }

        /// <summary>
        /// Gets or sets the method factory.
        /// </summary>
        /// <value>The method factory.</value>
        public IMethodFactory MethodFactory { get; private set; }

        /// <summary>
        /// Gets or sets the message binder.
        /// </summary>
        /// <value>The message binder.</value>
        public IMessageBinder MessageBinder { get; private set; }

        /// <summary>
        /// Applies the action filter conventions.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="targetMethod">The target method.</param>
        public void ApplyActionFilterConventions(IAction action, IMethod targetMethod)
        {
            var found = targetMethod.FindMetadata<PreviewAttribute>()
                .FirstOrDefault(x => x.MethodName == "Can" + targetMethod.Info.Name);

            if(found != null)
                return;

            var canExecute = targetMethod.Info.DeclaringType.GetMethod(
                                 "Can" + targetMethod.Info.Name,
                                 targetMethod.Info.GetParameters().Select(x => x.ParameterType).ToArray()
                                 )
                             ?? targetMethod.Info.DeclaringType.GetMethod("get_Can" + targetMethod.Info.Name);

            if(canExecute != null)
                action.Filters.Add(new PreviewAttribute(MethodFactory.CreateFrom(canExecute)));
        }
    }
}