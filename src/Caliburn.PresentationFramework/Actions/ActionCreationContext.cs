namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using System.Reflection;
    using Conventions;
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
        /// <param name="conventionManager">The convention manager</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="targetFilters">The target filters.</param>
        /// <param name="method">The method.</param>
        public ActionCreationContext(IServiceLocator serviceLocator, IMethodFactory methodFactory,
                                     IMessageBinder messageBinder, IConventionManager conventionManager, Type targetType, IFilterManager targetFilters,
                                     MethodInfo method)
        {
            ConventionManager = conventionManager;
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
        /// Gets or sets the convention manager.
        /// </summary>
        /// <value>The convention manager.</value>
        public IConventionManager ConventionManager { get; private set; }
    }
}