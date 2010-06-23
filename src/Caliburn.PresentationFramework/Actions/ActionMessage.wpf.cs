#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.Actions
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Markup;
    using Core.Logging;
    using Microsoft.Practices.ServiceLocation;
    using RoutedMessaging;
    using ViewModels;

    /// <summary>
    /// An <see cref="IRoutedMessage"/> for actions.
    /// </summary>
    [ContentProperty("Parameters")]
    public class ActionMessage : Freezable, IRoutedMessageWithOutcome
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(ActionMessage));

        /// <summary>
        /// Represents the parameters of an action message.
        /// </summary>
        public static readonly DependencyProperty ParametersProperty =
            DependencyProperty.Register(
                "Parameters",
                typeof(FreezableCollection<Parameter>),
                typeof(ActionMessage)
                );

        /// <summary>
        /// Represents the method name of an action message.
        /// </summary>
        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register(
                "MethodName",
                typeof(string),
                typeof(ActionMessage)
                );

        /// <summary>
        /// Represents the return path of an action message.
        /// </summary>
        public static readonly DependencyProperty OutcomePathProperty =
            DependencyProperty.Register(
                "OutcomePath",
                typeof(string),
                typeof(ActionMessage)
                );

        /// <summary>
        /// Represents the availability effect of an action message.
        /// </summary>
        public static readonly DependencyProperty AvailabilityEffectProperty =
            DependencyProperty.Register(
                "AvailabilityEffect",
                typeof(IAvailabilityEffect),
                typeof(ActionMessage)
                );

        private IInteractionNode _source;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionMessage"/> class.
        /// </summary>
        public ActionMessage()
        {
            SetValue(ParametersProperty, new FreezableCollection<Parameter>());
        }

        /// <summary>
        /// Gets or sets the name of the method to be invoked on the presentation model class.
        /// </summary>
        /// <value>The name of the method.</value>
        public string MethodName
        {
            get { return (string)GetValue(MethodNameProperty); }
            set { SetValue(MethodNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the path to use in binding the return value.
        /// </summary>
        /// <value>The return path.</value>
        public string OutcomePath
        {
            get { return (string)GetValue(OutcomePathProperty); }
            set { SetValue(OutcomePathProperty, value); }
        }

        /// <summary>
        /// Gets the default element to bind to if no return path is specified.
        /// </summary>
        /// <value>The default element.</value>
        public string DefaultOutcomeElement
        {
            get { return MethodName + "Result"; }
        }

        /// <summary>
        /// Gets or sets the availability effect.
        /// </summary>
        /// <value>The availability effect.</value>
        public IAvailabilityEffect AvailabilityEffect
        {
            get { return (IAvailabilityEffect)GetValue(AvailabilityEffectProperty); }
            set { SetValue(AvailabilityEffectProperty, value); }
        }

        /// <summary>
        /// Gets the parameters to pass as part of the method invocation.
        /// </summary>
        /// <value>The parameters.</value>
        public FreezableCollection<Parameter> Parameters
        {
            get { return (FreezableCollection<Parameter>)GetValue(ParametersProperty); }
        }

        /// <summary>
        /// Gets the source of the message.
        /// </summary>
        /// <value>The source.</value>
        public IInteractionNode Source
        {
            get { return _source; }
        }

        /// <summary>
        /// Initializes the message for interaction with the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void Initialize(IInteractionNode node)
        {
            _source = node;

            foreach(var parameter in Parameters)
            {
                parameter.ValueChanged += () => Invalidated();
            }
        }

        /// <summary>
        /// Indicates whether this message is related to the potential target.
        /// </summary>
        /// <param name="potentialTarget">The potential target.</param>
        /// <returns></returns>
        public bool RelatesTo(object potentialTarget)
        {
            var memberInfo = potentialTarget as MemberInfo;
            if (memberInfo == null) return false;

            return memberInfo.Name == MethodName;
        }

        /// <summary>
        /// Occurs when the message is invalidated.
        /// </summary>
        public event System.Action Invalidated = delegate { };

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable"/> derived class.
        /// </summary>
        /// <returns>The new instance.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new ActionMessage();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(IRoutedMessage other)
        {
            return ReferenceEquals(this, other);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "Action: " + MethodName;
        }

        /// <summary>
        /// Gets the default handlers for this type of message.
        /// </summary>
        /// <param name="node">The node to get default handlers for.</param>
        /// <returns></returns>
        public IEnumerable<IRoutedMessageHandler> GetDefaultHandlers(IInteractionNode node)
        {
            Log.Info("Checking default handlers for {0}.", this);

            var context = node.UIElement.GetDataContext();
            if(context == null)
                yield break;

            yield return new ActionMessageHandler(
                ServiceLocator.Current.GetInstance<IViewModelDescriptionFactory>()
                    .Create(context.GetType()),
                context
                );
        }
    }
}

#endif