namespace Caliburn.PresentationFramework.Filters
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using Core;
    using Core.Invocation;
    using Microsoft.Practices.ServiceLocation;
    using RoutedMessaging;

    /// <summary>
    /// A filter capable of specifying the dependencies raised by an implementor of <see cref="INotifyPropertyChanged"/> which can cause a trigger's availability to be re-evaluated.
    /// </summary>
#if !SILVERLIGHT
    [CLSCompliant(false)]
#endif
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public class DependenciesAttribute : Attribute, IHandlerAware, IInitializable
    {
        private readonly string[] _dependencies;
        private MemberInfo _target;
        private IMethodFactory _methodFactory;

        /// <summary>
        /// Gets the priority used to order filters.
        /// </summary>
        /// <value>The order.</value>
        /// <remarks>Higher numbers are evaluated first.</remarks>
        public int Priority { get; set; }

        /// <summary>
        /// Gets the dependencies.
        /// </summary>
        /// <value>The dependencies.</value>
        public string[] Dependencies
        {
            get { return _dependencies; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependenciesAttribute"/> class.
        /// </summary>
        /// <param name="dependencies">The dependencies.</param>
        public DependenciesAttribute(params string[] dependencies)
        {
            _dependencies = dependencies;
        }

        /// <summary>
        /// Initializes the filter.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="memberInfo">The member.</param>
        /// <param name="serviceLocator">The serviceLocator.</param>
        public void Initialize(Type targetType, MemberInfo memberInfo, IServiceLocator serviceLocator)
        {
            _target = memberInfo;
            _methodFactory = serviceLocator.GetInstance<IMethodFactory>();
        }

        /// <summary>
        /// Makes the filter aware of the <see cref="IRoutedMessageHandler"/>.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        public void MakeAwareOf(IRoutedMessageHandler messageHandler)
        {
            var notifier = messageHandler.Unwrap() as INotifyPropertyChanged;
            if (notifier == null) return;

            var helper = messageHandler.Metadata.FirstOrDefaultOfType<DependencyObserver>();
            if (helper != null) return;

            helper = new DependencyObserver(messageHandler, _methodFactory, notifier);
            messageHandler.Metadata.Add(helper);
        }

        /// <summary>
        /// Makes the filter aware of the <see cref="IMessageTrigger"/>.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        /// <param name="trigger">The trigger.</param>
        public void MakeAwareOf(IRoutedMessageHandler messageHandler, IMessageTrigger trigger)
        {
            var helper = messageHandler.Metadata.FirstOrDefaultOfType<DependencyObserver>();
            if (helper == null) return;

            if (trigger.Message.RelatesTo(_target))
                helper.MakeAwareOf(trigger, _dependencies);
        }
    }
}