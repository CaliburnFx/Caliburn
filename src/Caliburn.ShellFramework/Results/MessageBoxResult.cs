namespace Caliburn.ShellFramework.Results
{
    using System;
    using Core.InversionOfControl;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;
    using Questions;

    /// <summary>
    /// An <see cref="IResult"/> for showing custom message boxes.
    /// </summary>
    public class MessageBoxResult : IResult
    {
        readonly string text;
        readonly string caption = "Info";
        readonly Answer[] possibleAnswers = new[] {Answer.Ok};

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxResult"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public MessageBoxResult(string text)
        {
            this.text = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxResult"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="caption">The caption.</param>
        public MessageBoxResult(string text, string caption)
        {
            this.text = text;
            this.caption = caption;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxResult"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="possibleAnswers">The possible answers.</param>
        public MessageBoxResult(string text, string caption, params Answer[] possibleAnswers)
        {
            this.text = text;
            this.caption = caption;
            this.possibleAnswers = possibleAnswers;
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return text; }
        }

        /// <summary>
        /// Gets the caption.
        /// </summary>
        /// <value>The caption.</value>
        public string Caption
        {
            get { return caption; }
        }

        /// <summary>
        /// Gets the possible answers.
        /// </summary>
        /// <value>The possible answers.</value>
        public Answer[] PossibleAnswers
        {
            get { return possibleAnswers; }
        }

        /// <summary>
        /// Gets or sets the answer.
        /// </summary>
        /// <value>The answer.</value>
        public Answer Answer { get; set; }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(ResultExecutionContext context)
        {
            var questionDialog = context.ServiceLocator.GetInstance<IQuestionDialog>();
            var question = new Question(Text, possibleAnswers);
            questionDialog.Setup(Caption, new[]{question});

            EventHandler<DeactivationEventArgs> handler = null;
            handler = (s, e) =>{
                if(!e.WasClosed)
                    return;

                questionDialog.Deactivated -= handler;
                Answer = question.Answer;

                if (question.Answer == Answer.No || question.Answer == Answer.Cancel)
                {
                    Completed(this, new ResultCompletionEventArgs { WasCancelled = true });
                    return;
                }

                Completed(this, new ResultCompletionEventArgs());
            };
            questionDialog.Deactivated += handler;

            var windowManager = context.ServiceLocator.GetInstance<IWindowManager>();
            windowManager.ShowDialog(questionDialog, null);
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}