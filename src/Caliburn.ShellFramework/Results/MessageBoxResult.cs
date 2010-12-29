namespace Caliburn.ShellFramework.Results
{
    using System;
    using Core.InversionOfControl;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;
    using Questions;

    /// <summary>
    /// An <see cref="IResult"/> for showing custom message boxes.
    /// </summary>
    public class MessageBoxResult : IResult
    {
        private readonly string _text;
        private readonly string _caption = "Shell";
        private readonly Action<Answer> _handleResult = delegate { };
        private readonly Answer[] _possibleAnswers = new[] {Answer.Ok};

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxResult"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public MessageBoxResult(string text)
        {
            _text = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxResult"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="caption">The caption.</param>
        public MessageBoxResult(string text, string caption)
        {
            _text = text;
            _caption = caption;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxResult"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="handleResult">The result callback.</param>
        public MessageBoxResult(string text, string caption, Action<Answer> handleResult)
        {
            _text = text;
            _caption = caption;
            _handleResult = handleResult;
            _possibleAnswers = new[] {Answer.Cancel, Answer.Ok};
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxResult"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="handleResult">The handle result.</param>
        /// <param name="possibleAnswers">The possible answers.</param>
        public MessageBoxResult(string text, string caption, Action<Answer> handleResult, params Answer[] possibleAnswers)
        {
            _text = text;
            _caption = caption;
            _handleResult = handleResult;
            _possibleAnswers = possibleAnswers;
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return _text; }
        }

        /// <summary>
        /// Gets the caption.
        /// </summary>
        /// <value>The caption.</value>
        public string Caption
        {
            get { return _caption; }
        }

        /// <summary>
        /// Gets the possible answers.
        /// </summary>
        /// <value>The possible answers.</value>
        public Answer[] PossibleAnswers
        {
            get { return _possibleAnswers; }
        }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(ResultExecutionContext context)
        {
            var questionDialog = context.ServiceLocator.GetInstance<IQuestionDialog>();
            var windowManager = context.ServiceLocator.GetInstance<IWindowManager>();

            var question = new Question(
                Text,
                _possibleAnswers
                );

            questionDialog.Setup(
                Caption,
                new[] {question}
                );

            questionDialog.Deactivated += (s,e) => {
                if (!e.WasClosed)
                    return;

                if(_handleResult != null)
                    _handleResult(question.Answer);
                else if(question.Answer == Answer.No || question.Answer == Answer.Cancel)
                {
                    Completed(this, new ResultCompletionEventArgs {WasCancelled = true});
                    return;
                }

                Completed(this, new ResultCompletionEventArgs());
            };

            windowManager.ShowDialog(questionDialog, null);
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}