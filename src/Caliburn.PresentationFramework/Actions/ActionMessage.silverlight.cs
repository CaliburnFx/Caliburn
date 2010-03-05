#if SILVERLIGHT

namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Markup;

    using System.Windows.Controls;
    using System.Collections.Generic;
    using Core.Invocation;
    using Microsoft.Practices.ServiceLocation;
    using ViewModels;
    using RoutedMessaging;

    /// <summary>
    /// An <see cref="IRoutedMessage"/> for actions.
    /// </summary>
    [ContentProperty("Parameters")]
    public class ActionMessage : IRoutedMessageWithOutcome
    {
        private IInteractionNode _source;
        private List<Parameter> _parameters = new List<Parameter>();

        /// <summary>
        /// Gets or sets the availability effect.
        /// </summary>
        /// <value>The availability effect.</value>
        public IAvailabilityEffect AvailabilityEffect { get; set; }

        /// <summary>
        /// Gets the source of the message.
        /// </summary>
        /// <value>The source.</value>
        public IInteractionNode Source
        {
            get { return _source; }
        }

        /// <summary>
        /// Gets or sets the name of the method.
        /// </summary>
        /// <value>The name of the method.</value>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets the path to use in binding the outcome.
        /// </summary>
        /// <value>The return path.</value>
        public string OutcomePath { get; set; }

        /// <summary>
        /// Gets the default element to bind to if no outcome path is specified.
        /// </summary>
        /// <value>The default element.</value>
        public string DefaultOutcomeElement
        {
            get { return MethodName + "Result"; }
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public List<Parameter> Parameters
        {
            get { return _parameters; }
        }

        /// <summary>
        /// Initializes the message for interaction with the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void Initialize(IInteractionNode node) 
        {
            _source = node;

            foreach (var parameter in Parameters)
            {
                parameter.Configure(node);
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