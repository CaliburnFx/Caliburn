namespace Caliburn.ShellFramework.Questions
{
    using System.Collections.Generic;
    using PresentationFramework;
    using PresentationFramework.Screens;

    /// <summary>
    /// The default implementation of <see cref="IQuestionDialog"/>.
    /// </summary>
    public class QuestionDialogViewModel : Screen, IQuestionDialog
    {
        /// <summary>
        /// Gets a value indicating whether this instance has one question.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has one question; otherwise, <c>false</c>.
        /// </value>
        public bool HasOneQuestion
        {
            get { return Questions.Count == 1; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has multiple questions.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has multiple questions; otherwise, <c>false</c>.
        /// </value>
        public bool HasMultipleQuestions
        {
            get { return Questions.Count > 1; }
        }

        /// <summary>
        /// Gets the first question.
        /// </summary>
        /// <value>The first question.</value>
        public Question FirstQuestion
        {
            get { return Questions[0]; }
        }

        /// <summary>
        /// Gets or sets the questions.
        /// </summary>
        /// <value>The questions.</value>
        public IObservableCollection<Question> Questions { get; private set; }

        /// <summary>
        /// Gets the buttons.
        /// </summary>
        /// <value>The buttons.</value>
        public IObservableCollection<ButtonModel> Buttons
        {
            get
            {
                return HasMultipleQuestions
                           ? new BindableCollection<ButtonModel> {new ButtonModel("Ok")}
                           : FirstQuestion.Buttons;
            }
        }

        /// <summary>
        /// Prepares the dialog with a title and a series of questions.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="questions">The questions.</param>
        public void Setup(string title, IEnumerable<Question> questions)
        {
            DisplayName = title;
            Questions = new BindableCollection<Question>(questions);
        }

        /// <summary>
        /// Answers with Ok.
        /// </summary>
        public void Ok()
        {
            SelectAnswer(Answer.Ok);
        }

        /// <summary>
        /// Answers with Cancel.
        /// </summary>
        public void Cancel()
        {
            SelectAnswer(Answer.Cancel);
        }

        /// <summary>
        /// Answers with Yes.
        /// </summary>
        public void Yes()
        {
            SelectAnswer(Answer.Yes);
        }

        /// <summary>
        /// Answers with No.
        /// </summary>
        public void No()
        {
            SelectAnswer(Answer.No);
        }

        private void SelectAnswer(Answer answer)
        {
            FirstQuestion.Answer = answer;
            TryClose();
        }
    }
}