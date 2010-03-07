namespace CompositeCommands.Framework
{
    using System;
    using System.Collections.Generic;
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.Commands;

    /// <summary>
    /// An <see cref="ICompositeCommand"/> that can execute when any of its children are available.
    /// </summary>
    public class AnyCommand : PropertyChangedBase, ICompositeCommand
    {
        private readonly WeakKeyedDictionary<CommandMessage, bool?> _children =
            new WeakKeyedDictionary<CommandMessage, bool?>();

        private bool _canExecute = true;

        /// <summary>
        /// Gets a value indicating whether this instance is available.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is available; otherwise, <c>false</c>.
        /// </value>
        public bool CanExecute
        {
            get { return _canExecute; }
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public void Execute()
        {
            _children.RemoveCollectedEntries();

            var allCommands = new List<CommandMessage>(_children.Keys);
            var commandsToExecute = new List<CommandMessage>();

            foreach(var command in allCommands)
            {
                if(_children[command].GetValueOrDefault(false))
                {
                    command.Completed += Command_Completed;
                    _children[command] = null;
                    commandsToExecute.Add(command);
                }
            }

            DetermineAvailability();

            foreach(var command in commandsToExecute)
            {
                command.Process(command, null);
            }
        }

        private void Command_Completed(object sender, EventArgs e)
        {
            var message = (CommandMessage)sender;

            message.Completed -= Command_Completed;

            if(_children[message] == null)
                _children[message] = true;

            DetermineAvailability();
        }

        /// <summary>
        /// Adds or updates the child command.
        /// </summary>
        /// <param name="child">The child.</param>
        /// <param name="availability">if set to <c>true</c> the child can execute.</param>
        public void AddOrUpdateChild(CommandMessage child, bool availability)
        {
            _children[child] = availability;

            DetermineAvailability();
        }

        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="child">The child.</param>
        public void RemoveChild(CommandMessage child)
        {
            _children.Remove(child);

            DetermineAvailability();
        }

        private void DetermineAvailability()
        {
            _children.RemoveCollectedEntries();
            _canExecute = false;

            foreach(var state in _children.Values)
            {
                if(state.GetValueOrDefault(false))
                {
                    _canExecute = true;
                    break;
                }
            }

            NotifyOfPropertyChange(() => CanExecute);
        }
    }
}