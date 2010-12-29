namespace Caliburn.ShellFramework.Results
{
    using System;
    using Core.InversionOfControl;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;

    /// <summary>
    /// An <see cref="IResult"/> capable of directing focus to a control based on the property it is bound to.
    /// </summary>
    public class FocusResult : IResult
    {
        private readonly object _model;
        private readonly string _property;

        /// <summary>
        /// Initializes a new instance of the <see cref="FocusResult"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="property">The property.</param>
        public FocusResult(object model, string property)
        {
            _model = model;
            _property = property;
        }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(ResultExecutionContext context)
        {
            var inputManager = context.ServiceLocator.GetInstance<IInputManager>();

            if(string.IsNullOrEmpty(_property))
                inputManager.Focus(_model);
            else inputManager.Focus(_model, _property);

            Completed(this, new ResultCompletionEventArgs());
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}