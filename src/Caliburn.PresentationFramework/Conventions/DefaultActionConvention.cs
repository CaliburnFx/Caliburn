namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using Actions;
    using Microsoft.Practices.ServiceLocation;
    using ViewModels;

    public class DefaultActionConvention : IActionConvention
    {
        private static IMessageBinder _messageBinder;
        private static IMessageBinder MessageBinder
        {
            get
            {
                if (_messageBinder == null)
                    _messageBinder = ServiceLocator.Current.GetInstance<IMessageBinder>();
                return _messageBinder;
            }
        }

        public bool Matches(IViewModelDescription viewModelDescription, IElementDescription element, IAction action)
        {
            return string.Compare(element.Name, action.Name, StringComparison.CurrentCultureIgnoreCase) == 0;
        }

        public IViewApplicable CreateApplication(IViewModelDescription description, IElementDescription element, IAction action)
        {
            var message = action.Name;

            if (action.Requirements.Count > 0)
            {
                message += "(";

                foreach (var requirement in action.Requirements)
                {
                    var paramName = requirement.Name;
                    var specialValue = "$" + paramName;

                    if (MessageBinder.IsSpecialValue(specialValue))
                        paramName = specialValue;

                    message += paramName + ",";
                }

                message = message.Remove(message.Length - 1, 1);

                message += ")";
            }

            return new ApplicableAction(element.Name, message);
        }
    }
}