#if SILVERLIGHT

namespace Caliburn.PresentationFramework.Commands
{
    using System;
    using System.Linq;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Markup;

    using System.Windows.Controls;
    using System.Collections.Generic;
    using Actions;
    using Configuration;
    using Core;
    using Microsoft.Practices.ServiceLocation;
    using ViewModels;
    using RoutedMessaging;
    using RoutedMessaging.Triggers;

    /// <summary>
    /// An <see cref="IRoutedMessage"/> for commands.
    /// </summary>
    [ContentProperty("Parameters")]
    public class CommandMessage : Control, IRoutedMessageWithOutcome, IRoutedMessageHandler
    {
        /// <summary>
        /// Represents the command tied to the message.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                "Command",
                typeof(object),
                typeof(CommandMessage),
                new PropertyMetadata(CommandChanged)
                );

        /// <summary>
        /// Represents the parent of the command.
        /// </summary>
        public static readonly DependencyProperty ParentCommandProperty =
            DependencyProperty.Register(
                "ParentCommand",
                typeof(ICompositeCommand),
                typeof(CommandMessage),
                null
                );

        /// <summary>
        /// Represents the return path of an command message.
        /// </summary>
        public static readonly DependencyProperty OutcomePathProperty =
            DependencyProperty.Register(
                "OutcomePath",
                typeof(string),
                typeof(CommandMessage),
                null
                );

        /// <summary>
        /// Represents the availability effect of an command message.
        /// </summary>
        public static readonly DependencyProperty AvailabilityEffectProperty =
            DependencyProperty.Register(
                "AvailabilityEffect",
                typeof(IAvailabilityEffect),
                typeof(CommandMessage),
                null
                );

        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue != e.NewValue && e.NewValue != null)
            {
                var message = (CommandMessage)d;

                if(e.NewValue is string)
                {
                    message._isSetting = true;
                    d.SetValue(CommandProperty, ServiceLocator.Current.GetInstance(null, e.NewValue as string));
                    message._isSetting = false;
                }
                
                if(!message._isSetting)
                    message.CreateAction(true);
            }
        }

        private bool _isSetting;
        private IInteractionNode _source;
        private readonly IViewModelDescriptionFactory _factory;

        private ActionMessage _actionMessage;
        private IAction _action;
        private readonly List<Parameter> _parameters = new List<Parameter>();
        private IList<object> _metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMessage"/> class.
        /// </summary>
        public CommandMessage()
        {
            if(!PresentationFrameworkConfiguration.IsInDesignMode)
                _factory = ServiceLocator.Current.GetInstance<IViewModelDescriptionFactory>();
        }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        public IList<object> Metadata
        {
            get
            {
                if(_metadata == null)
                    _metadata = new List<object>();

                return _metadata;
            }
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        public object Command
        {
            get { return GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value);}
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public ICompositeCommand ParentCommand
        {
            get { return (ICompositeCommand)GetValue(ParentCommandProperty); }
            set { SetValue(ParentCommandProperty, value); }
        }

        /// <summary>
        /// Occurs when the command has completed executing.
        /// </summary>
        public event EventHandler Completed = delegate { };

        private void OnCompleted()
        {
            Completed(this, EventArgs.Empty);
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
        /// Gets the default element to bind to if no outcome path is specified.
        /// </summary>
        /// <value>The default element.</value>
        public string DefaultOutcomeElement
        {
            get { return _actionMessage.DefaultOutcomeElement; }
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
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public List<Parameter> Parameters
        {
            get { return _parameters; }
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

            foreach (var parameter in Parameters)
            {
                parameter.Configure(node);
                parameter.ValueChanged += () => Invalidated();
            }
        }

        /// <summary>
        /// Occurs when the message is invalidated.
        /// </summary>
        public event System.Action Invalidated = delegate { };

        /// <summary>
        /// Gets the underlying object instance to which this handler routes requests.
        /// </summary>
        /// <returns></returns>
        public object Unwrap()
        {
            return Command;
        }

        /// <summary>
        /// Indicates whether this instance can handle the speicified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public bool Handles(IRoutedMessage message)
        {
            return message == this;
        }

        /// <summary>
        /// Processes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">An object that provides additional context for message processing.</param>
        public void Process(IRoutedMessage message, object context)
        {
            if(message != this)
                throw new CaliburnException("The handler cannot process the message.");

            CreateActionMessage();

            _action.Execute(_actionMessage, Source, context);
        }

        /// <summary>
        /// Updates the availability of the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        public void UpdateAvailability(IMessageTrigger trigger)
        {
            if(trigger.Message != this)
                throw new CaliburnException("The handler cannot update availability for this trigger.");

            if (Command != null)
			{
				CreateActionMessage();

				if (_action.HasTriggerEffects())
				{
					bool isAvailable = _action.ShouldTriggerBeAvailable(_actionMessage, Source);
                    trigger.UpdateAvailabilty(isAvailable);
					TryUpdateParentAvailability(isAvailable);
				}
			}
        }

        /// <summary>
        /// Makes the handler aware of a specific trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        public void MakeAwareOf(IMessageTrigger trigger)
        {
            if(trigger.Message != this) return;
			
            CreateActionMessage();

            if(_action.HasTriggerEffects())
            {
                bool isAvailable = _action.ShouldTriggerBeAvailable(_actionMessage, Source);
                trigger.UpdateAvailabilty(isAvailable);
                TryUpdateParentAvailability(isAvailable);
            }

            _action.Filters.HandlerAware.Apply(x => x.MakeAwareOf(this, trigger));
        }

        /// <summary>
        /// Indicates whether this message is related to the potential target.
        /// </summary>
        /// <param name="potentialTarget">The potential target.</param>
        /// <returns></returns>
        public bool RelatesTo(object potentialTarget)
        {
            return _actionMessage.RelatesTo(potentialTarget);
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
            if (other is ActionMessage)
                return ReferenceEquals(_actionMessage, other);
            return ReferenceEquals(this, other);
        }

        private void TryUpdateParentAvailability(bool isAvailable)
        {
            var parent = ParentCommand ?? Commands.Command.GetParent(Source.UIElement);
            if(parent != null) parent.AddOrUpdateChild(this, isAvailable);
        }

        private void CreateActionMessage()
        {
            string methodName = "Execute";

            var att = Command.GetType()
                .GetAttributes<CommandAttribute>(true)
                .FirstOrDefault();

            if(att != null)
                methodName = att.ExecuteMethod;

            _actionMessage = new ActionMessage
            {
                MethodName = methodName,
                AvailabilityEffect = AvailabilityEffect,
                OutcomePath = OutcomePath
            };

            _actionMessage.Initialize(Source);

            foreach(var parameter in Parameters)
            {
                _actionMessage.Parameters.Add(new Parameter(parameter.Value));
            }
        }

        private void CreateAction(bool forceRecreate)
        {
            if(_action != null && !forceRecreate)
                return;

            var host = _factory.Create(Command.GetType());

            host.Actions.SelectMany(x => x.Filters.HandlerAware)
                .Union(host.Filters.HandlerAware)
                .Apply(x => x.MakeAwareOf(this));

            CreateActionMessage();

            _action = host.GetAction(_actionMessage);
            _action.Completed += delegate { OnCompleted(); };
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "Command: " + Command;
        }

        /// <summary>
        /// Gets the default handlers for this type of message.
        /// </summary>
        /// <param name="node">The node to get default handlers for.</param>
        /// <returns></returns>
        public IEnumerable<IRoutedMessageHandler> GetDefaultHandlers(IInteractionNode node)
        {
            return _actionMessage.GetDefaultHandlers(node);
        }
    }
}

#endif