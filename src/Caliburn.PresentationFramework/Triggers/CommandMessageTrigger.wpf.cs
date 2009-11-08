#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.Triggers
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Core;

    /// <summary>
    /// A message trigger that triggers from command execution of an ICommandSource.
    /// </summary>
    public class CommandMessageTrigger : BaseMessageTrigger, ICommand
    {
        private bool _canExecute = true;

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable"/> derived class.
        /// </summary>
        /// <returns>The new instance.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new CommandMessageTrigger();
        }

        /// <summary>
        /// Wires the trigger into the interaction hierarchy.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Attach(IInteractionNode node)
        {
            var commandProperty = node.UIElement.GetType().GetProperty("Command");

            if(commandProperty == null)
            {
                throw new CaliburnException(
                    string.Format(
                        "You cannot use a CommandMessageTrigger with an instance of {0}.  The source element should be an ICommandSource.",
                        node.UIElement.GetType().FullName
                        )
                    );
            }

            commandProperty.SetValue(node.UIElement, this, null);
            base.Attach(node);
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
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        void ICommand.Execute(object parameter)
        {
            Node.ProcessMessage(Message, parameter);
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        bool ICommand.CanExecute(object parameter)
        {
            var effect = Message.AvailabilityEffect ??
                         PresentationFramework.Message.GetAvailabilityEffect(Message.Source.UIElement);

            if(effect != null && effect == AvailabilityEffect.Disable)
                return _canExecute;

            return true;
        }
    }
}

#endif