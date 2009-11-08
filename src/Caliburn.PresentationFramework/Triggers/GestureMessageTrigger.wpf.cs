#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.Triggers
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;
    using Core;

    /// <summary>
    /// An implentation of <see cref="InputGesture"/> that enables triggers based on an <see cref="IMessageTrigger"/>.
    /// </summary>
    public class GestureMessageTrigger : BaseMessageTrigger
    {
        /// <summary>
        /// A dependency property representing the key's value.
        /// </summary>
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register(
                "Key",
                typeof(Key),
                typeof(GestureMessageTrigger),
                new PropertyMetadata(Key.None)
                );

        /// <summary>
        /// A dependency property representing the modifier's value.
        /// </summary>
        public static readonly DependencyProperty ModifiersProperty =
            DependencyProperty.Register(
                "Modifiers",
                typeof(ModifierKeys),
                typeof(GestureMessageTrigger),
                new PropertyMetadata(ModifierKeys.None)
                );

        /// <summary>
        /// A dependency property representing the mouse action's value.
        /// </summary>
        public static readonly DependencyProperty MouseActionProperty =
            DependencyProperty.Register(
                "MouseAction",
                typeof(MouseAction),
                typeof(GestureMessageTrigger),
                new PropertyMetadata(MouseAction.None)
                );

        private bool _canExecute = true;

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public Key Key
        {
            get { return (Key)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the modifiers.
        /// </summary>
        /// <value>The modifiers.</value>
        public ModifierKeys Modifiers
        {
            get { return (ModifierKeys)GetValue(ModifiersProperty); }
            set { SetValue(ModifiersProperty, value); }
        }

        /// <summary>
        /// Gets or sets the mouse action.
        /// </summary>
        /// <value>The mouse action.</value>
        public MouseAction MouseAction
        {
            get { return (MouseAction)GetValue(MouseActionProperty); }
            set { SetValue(MouseActionProperty, value); }
        }

        /// <summary>
        /// Wires the trigger into the interactin hierarchy.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Attach(IInteractionNode node)
        {
            InputGesture gesture;

            if(Key != Key.None)
                gesture = new KeyGesture(Key, Modifiers);
            else gesture = new MouseGesture(MouseAction, Modifiers);

            var uiElement = node.UIElement as UIElement;

            if(uiElement == null)
                throw new CaliburnException(
                    string.Format(
                        "You cannot use a GestureMessageTrigger with an instance of {0}.  The source element must inherit from UIElement.",
                        node.UIElement.GetType().FullName
                        )
                    );

            FindOrCreateLookup(uiElement, gesture)
                .AddTrigger(this);

            base.Attach(node);
        }

        private GestureCommand FindOrCreateLookup(UIElement uiElement, InputGesture gesture)
        {
            foreach(var item in uiElement.InputBindings)
            {
                var binding = (InputBinding)item;
                var command = binding.Command as GestureCommand;

                if(command != null
                   && AreGestureEqual(gesture, command.Gesture))
                {
                    return command;
                }
            }

            var lookup = new GestureCommand(gesture);

            uiElement.InputBindings.Add(
                new InputBinding(lookup, gesture)
                );

            return lookup;
        }

        private bool AreGestureEqual(InputGesture left, InputGesture right)
        {
            if(left.GetType() != right.GetType()) return false;

            var leftKeyGesture = left as KeyGesture;
            if(leftKeyGesture != null)
            {
                var rightKeyGesture = (KeyGesture)right;

                return rightKeyGesture.Key == leftKeyGesture.Key
                       && rightKeyGesture.Modifiers == leftKeyGesture.Modifiers;
            }

            var leftMouseGesture = left as MouseGesture;
            if(leftMouseGesture != null)
            {
                var rightMouseGesture = (MouseGesture)right;

                return rightMouseGesture.MouseAction == leftMouseGesture.MouseAction
                       && rightMouseGesture.Modifiers == leftMouseGesture.Modifiers;
            }

            return false;
        }

        /// <summary>
        /// Updates the UI to reflect the availabilty of the trigger.
        /// </summary>
        /// <param name="isAvailable">if set to <c>true</c> [is available].</param>
        public override void UpdateAvailabilty(bool isAvailable)
        {
            _canExecute = isAvailable;
            base.UpdateAvailabilty(_canExecute);
        }

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable"/> derived class.
        /// </summary>
        /// <returns>The new instance.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new GestureMessageTrigger();
        }

        /// <summary>
        /// Used to attach gestures.
        /// </summary>
        public class GestureCommand : ICommand
        {
            private readonly InputGesture _gesture;
            private readonly List<GestureMessageTrigger> _triggers = new List<GestureMessageTrigger>();

            /// <summary>
            /// Initializes a new instance of the <see cref="GestureCommand"/> class.
            /// </summary>
            /// <param name="gesture">The gesture.</param>
            public GestureCommand(InputGesture gesture)
            {
                _gesture = gesture;
            }

            /// <summary>
            /// Gets the gesture.
            /// </summary>
            /// <value>The gesture.</value>
            internal InputGesture Gesture
            {
                get { return _gesture; }
            }

            /// <summary>
            /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
            /// </summary>
            /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
            /// <returns>
            /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
            /// </returns>
            /// <exception cref="T:System.NullReferenceException">
            /// The <paramref name="obj"/> parameter is null.
            /// </exception>
            public override bool Equals(object obj)
            {
                var other = obj as GestureCommand;
                return other != null && _gesture.Equals(other._gesture);
            }

            /// <summary>
            /// Serves as a hash function for a particular type.
            /// </summary>
            /// <returns>
            /// A hash code for the current <see cref="T:System.Object"/>.
            /// </returns>
            public override int GetHashCode()
            {
                return (GetType().FullName + "|" + _gesture.GetHashCode()).GetHashCode();
            }

            /// <summary>
            /// Implements the operator ==.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator ==(GestureCommand x, GestureCommand y)
            {
                return Equals(x, y);
            }

            /// <summary>
            /// Implements the operator !=.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator !=(GestureCommand x, GestureCommand y)
            {
                return !(x == y);
            }

            /// <summary>
            /// Adds the trigger.
            /// </summary>
            /// <param name="trigger">The trigger.</param>
            public void AddTrigger(GestureMessageTrigger trigger)
            {
                _triggers.Add(trigger);
            }

            /// <summary>
            /// Defines the method to be called when the command is invoked.
            /// </summary>
            /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
            public void Execute(object parameter)
            {
                foreach(var trigger in _triggers)
                {
                    trigger.Node.ProcessMessage(trigger.Message, parameter);
                }
            }

            /// <summary>
            /// Defines the method that determines whether the command can execute in its current state.
            /// </summary>
            /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
            /// <returns>
            /// true if this command can be executed; otherwise, false.
            /// </returns>
            public bool CanExecute(object parameter)
            {
                bool result = true;

                foreach(var trigger in _triggers)
                {
                    var effect = trigger.Message.AvailabilityEffect
                                 ?? PresentationFramework.Message.GetAvailabilityEffect(trigger.Message.Source.UIElement);

                    if(effect != null && effect == AvailabilityEffect.Disable)
                        result = result && trigger._canExecute;
                    else result = result && true;
                }

                return result;
            }

            /// <summary>
            /// Occurs when changes occur that affect whether or not the command should execute.
            /// </summary>
            event EventHandler ICommand.CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }
        }
    }
}

#endif