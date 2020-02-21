namespace Caliburn.PresentationFramework.Conventions
{
    using System.Windows;
    using System.Windows.Data;
    using Actions;
    using Core.Logging;
    using RoutedMessaging;
    using Views;

    /// <summary>
    /// An <see cref="IViewApplicable"/> that attaches an action to an element.
    /// </summary>
    public class ApplicableAction : IViewApplicable
    {
        static readonly ILog Log = LogManager.GetLog(typeof(ApplicableAction));

        readonly string elementName;
        readonly string message;
        readonly string actionTargetPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicableAction"/> class.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="actionTargetPath">The path to the action target.</param>
        /// <param name="message">The message.</param>
        public ApplicableAction(string elementName, string actionTargetPath, string message)
        {
            this.elementName = elementName;
            this.actionTargetPath = actionTargetPath;
            this.message = message;
        }

        /// <summary>
        /// Applies the behavior to the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="isLoaded">Indicates whether the view element should be marked as loaded.</param>
        public void ApplyTo(DependencyObject view, object isLoaded)
        {
            var element = view.FindName(elementName);

            if(!string.IsNullOrEmpty(actionTargetPath))
            {
                var binding = new Binding(actionTargetPath);
                element.SetBinding(Action.TargetProperty, binding);
            }

            Message.SetAttach(element, message);
            Log.Info("Attached message {0} to {1}.", message, view);
        }
    }
}
