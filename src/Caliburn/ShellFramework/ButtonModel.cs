namespace Caliburn.ShellFramework
{
    /// <summary>
    /// A view model representing a button.
    /// </summary>
    public class ButtonModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonModel"/> class.
        /// </summary>
        public ButtonModel() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonModel"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ButtonModel(string name)
        {
            Content = ToolTip = Action = name;
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content { get; set; }

        /// <summary>
        /// Gets or sets the tool tip.
        /// </summary>
        /// <value>The tool tip.</value>
        public object ToolTip { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        public string Action { get; set; }
    }
}