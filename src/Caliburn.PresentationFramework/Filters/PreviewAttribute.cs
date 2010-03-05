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
    /// A basic pre execution filter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class PreviewAttribute : MethodCallFilterBase, IPreProcessor, IHandlerAware
    {
        private IMethodFactory _methodFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewAttribute"/> class.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        public PreviewAttribute(string methodName)
            : base(methodName)
        {
            AffectsTriggers = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewAttribute"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        public PreviewAttribute(IMethod method)
            : base(method)
        {
            AffectsTriggers = true;
        }

        /// <summary>
        /// Gets a value indicating whether this filter affects triggers.
        /// </summary>
        /// <value><c>true</c> if affects triggers; otherwise, <c>false</c>.</value>
        /// <remarks>True by default.</remarks>
        public bool AffectsTriggers { get; set; }

        /// <summary>
        /// Initializes the filter.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="member">The member.</param>
        /// <param name="serviceLocator">The serviceLocator.</param>
        public override void Initialize(Type targetType, MemberInfo member, IServiceLocator serviceLocator)
        {
            base.Initialize(targetType, member, serviceLocator);
            _methodFactory = serviceLocator.GetInstance<IMethodFactory>();
        }

        /// <summary>
        /// Executes the filter.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="handlingNode">The handling node.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public bool Execute(IRoutedMessage message, IInteractionNode handlingNode, object[] parameters)
        {
            var result = _method.Invoke(handlingNode.MessageHandler.Unwrap(), parameters);

            if (_method.Info.ReturnType == typeof(bool)) return (bool)result;
            return true;
        }

        /// <summary>
        /// Makes the filter aware of the <see cref="IRoutedMessageHandler"/>.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        public void MakeAwareOf(IRoutedMessageHandler messageHandler)
        {
            if (!AffectsTriggers || !IsGetter)
                return;

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
            if (!AffectsTriggers || !IsGetter)
                return;

            var helper = messageHandler.Metadata.FirstOrDefaultOfType<DependencyObserver>();
            if (helper == null) return;

            if (trigger.Message.RelatesTo(Member))
                helper.MakeAwareOf(trigger, new[] { MethodName });
        }
    }
}