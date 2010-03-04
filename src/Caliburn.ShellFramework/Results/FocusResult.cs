namespace Caliburn.ShellFramework.Results
{
    using System;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;

    public class FocusResult : IResult
    {
        private readonly object _model;
        private readonly string _property;

        public FocusResult(object model, string property)
        {
            _model = model;
            _property = property;
        }

        public void Execute(ResultExecutionContext context)
        {
            var inputManager = context.ServiceLocator.GetInstance<IInputManager>();

            if(string.IsNullOrEmpty(_property))
                inputManager.Focus(_model);
            else inputManager.Focus(_model, _property);

            Completed(this, new ResultCompletionEventArgs());
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}