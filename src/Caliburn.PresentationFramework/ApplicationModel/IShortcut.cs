namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System.Collections.Generic;
    using System.Windows.Input;
    using RoutedMessaging;
    using Screens;

    /// <summary>
    /// Represents a shortcut.
    /// </summary>
    public interface IShortcut : IHaveDisplayName
    {
        /// <summary>
        /// Gets or sets the modifers.
        /// </summary>
        /// <value>The modifers.</value>
        ModifierKeys Modifers { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        Key Key { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance can execute.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can execute; otherwise, <c>false</c>.
        /// </value>
        bool CanExecute { get; }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IResult> Execute();
    }
}