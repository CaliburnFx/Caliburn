namespace Caliburn.ShellFramework.Results
{
    using System;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;
    using Questions;

    public class MessageBoxResult : IResult
    {
        private readonly string _text;
        private readonly string _caption = "Shell";
        private readonly Action<Answer> _handleResult = delegate { };
        private readonly Answer[] _possibleAnswers = new[] {Answer.Ok};

        public MessageBoxResult(string text)
        {
            _text = text;
        }

        public MessageBoxResult(string text, string caption)
        {
            _text = text;
            _caption = caption;
        }

        public MessageBoxResult(string text, string caption, Action<Answer> handleResult)
        {
            _text = text;
            _caption = caption;
            _handleResult = handleResult;
            _possibleAnswers = new[] {Answer.Cancel, Answer.Ok};
        }

        public MessageBoxResult(string text, string caption, Action<Answer> handleResult, params Answer[] possibleAnswers)
        {
            _text = text;
            _caption = caption;
            _handleResult = handleResult;
            _possibleAnswers = possibleAnswers;
        }

        public string Text
        {
            get { return _text; }
        }

        public string Caption
        {
            get { return _caption; }
        }

        public Answer[] PossibleAnswers
        {
            get { return _possibleAnswers; }
        }

        public void Execute(ResultExecutionContext context)
        {
            var questionDialog = context.ServiceLocator.GetInstance<IQuestionDialog>();
            var windowManager = context.ServiceLocator.GetInstance<IWindowManager>();

            var question = new Question(
                null,
                Text,
                _possibleAnswers
                );

            questionDialog.Setup(
                Caption,
                new[] {question}
                );

            questionDialog.WasShutdown += delegate{
                if(_handleResult != null)
                    _handleResult(question.Answer);
                else if(question.Answer == Answer.No || question.Answer == Answer.Cancel)
                {
                    Completed(this, new ResultCompletionEventArgs {WasCancelled = true});
                    return;
                }

                Completed(this, new ResultCompletionEventArgs());
            };

            windowManager.ShowDialog(questionDialog, null, null);
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}