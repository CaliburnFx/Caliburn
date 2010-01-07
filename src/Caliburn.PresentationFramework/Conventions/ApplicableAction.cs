namespace Caliburn.PresentationFramework.Conventions
{
    using System.Windows;
    using ViewModels;

    /// <summary>
    /// An <see cref="IViewApplicable"/> that attaches an action to an element.
    /// </summary>
    public class ApplicableAction : IViewApplicable
    {
        private readonly string _elementName;
        private readonly string _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicableAction"/> class.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="message">The message.</param>
        public ApplicableAction(string elementName, string message)
        {
            _elementName = elementName;
            _message = message;
        }

        /// <summary>
        /// Applies the behavior to the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        public void ApplyTo(DependencyObject view)
        {
            var element = view.FindName(_elementName);
            Message.SetAttach(element, _message);
        }
    }
}