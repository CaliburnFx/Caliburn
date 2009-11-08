namespace Caliburn.PresentationFramework.Commands
{
    using System.ComponentModel;

    /// <summary>
    /// A command that executes multiple child commands.
    /// </summary>
    public interface ICompositeCommand : INotifyPropertyChanged
    {
        /// <summary>
        /// Adds or updates the child command.
        /// </summary>
        /// <param name="child">The child.</param>
        /// <param name="availability">if set to <c>true</c> the child can execute.</param>
        void AddOrUpdateChild(CommandMessage child, bool availability);

        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="child">The child.</param>
        void RemoveChild(CommandMessage child);
    }
}