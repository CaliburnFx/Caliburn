namespace Caliburn.PresentationFramework.Conventions
{
    using System.Windows;
    using ViewModels;

    public class ApplicableAction : IViewApplicable
    {
        private readonly string _elementName;
        private readonly string _message;

        public ApplicableAction(string elementName, string message)
        {
            _elementName = elementName;
            _message = message;
        }

        public void ApplyTo(DependencyObject view)
        {
            var element = view.FindControl(_elementName);
            Message.SetAttach(element, _message);
        }
    }
}