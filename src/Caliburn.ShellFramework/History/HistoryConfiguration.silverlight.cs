#if SILVERLIGHT

namespace Caliburn.ShellFramework.History
{
    using System;
    using PresentationFramework.Screens;

    /// <summary>
    /// The configuration for the histroy coordinator.
    /// </summary>
    public class HistoryConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryConfiguration"/> class.
        /// </summary>
        public HistoryConfiguration()
        {
            StateName = "Default";
            ItemNotFound = delegate { };
            AlterTitle = (oldTitle, item) => item.DetermineDisplayName();
        }

        /// <summary>
        /// Gets or sets the name of the state being coordinated.
        /// </summary>
        /// <value>The name of the state.</value>
        public string StateName { get; set; }

        /// <summary>
        /// Gets or sets the conductor being coordinated.
        /// </summary>
        /// <value>The conductor.</value>
        public IConductActiveItem Conductor { get; set; }

        /// <summary>
        /// Gets or sets the history key used to persist values.
        /// </summary>
        /// <value>The history key.</value>
        public string HistoryKey { get; set; }

        /// <summary>
        /// Gets or sets the determine item callback.
        /// </summary>
        /// <value>The determine item.</value>
        public Func<string, object> DetermineItem { get; set; }

        /// <summary>
        /// Gets or sets the alter title callback.
        /// </summary>
        /// <value>The alter title.</value>
        public Func<string, object, string> AlterTitle { get; set; }

        /// <summary>
        /// Gets or sets the item not found callback.
        /// </summary>
        /// <value>The item not found.</value>
        public Action<string> ItemNotFound { get; set; }
    }
}

#endif