namespace CompositeCommands.Framework
{
    using System;
    using System.Collections.Generic;
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.Commands;

    /// <summary>
    /// An <see cref="ICompositeCommand"/> whose children must all be available in order to execute.
    /// </summary>
    public class AllCommand : PropertyChangedBase, ICompositeCommand
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

            var commands = new List<CommandMessage>(_children.Keys);

            foreach(var command in commands)
            {
                command.Completed += Command_Completed;
                _children[command] = null;
            }

            DetermineAvailability();

            foreach(var command in commands)
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
            _canExecute = true;

            foreach(var state in _children.Values)
            {
                if(!state.GetValueOrDefault(false))
                {
                    _canExecute = false;
                    break;
                }
            }

            NotifyOfPropertyChange(() => CanExecute);
        }
    }
}