namespace Caliburn.ShellFramework.Questions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PresentationFramework;
    using PresentationFramework.ViewModels;

    /// <summary>
    /// Represents a question.
    /// </summary>
    public class Question : PropertyChangedBase
    {
        internal static Func<Answer, string> LocalizeAnswer = answer => answer.ToString();

        private Answer answer = Answer.No;

        /// <summary>
        /// Initializes a new instance of the <see cref="Question"/> class with the default no, yes and cancel answers.
        /// </summary>
        /// <param name="text">The text.</param>
        public Question(string text)
            : this(text, Answer.No, Answer.Yes, Answer.Cancel) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="Question"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="possibleAnswers">The possible answers.</param>
        public Question(string text, params Answer[] possibleAnswers)
        {
            Text = text;
            PossibleAnswers = new BindableEnumCollection<Answer>(possibleAnswers);
            Buttons = ConvertToButtons(possibleAnswers);
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; private set; }

        /// <summary>
        /// Gets or sets the possible answers.
        /// </summary>
        /// <value>The possible answers.</value>
        public BindableEnumCollection<Answer> PossibleAnswers { get; set; }

        /// <summary>
        /// Gets or sets the buttons.
        /// </summary>
        /// <value>The buttons.</value>
        public IObservableCollection<ButtonModel> Buttons { get; set; }

        /// <summary>
        /// Gets or sets the answer.
        /// </summary>
        /// <value>The answer.</value>
        public Answer Answer
        {
            get { return answer; }
            set
            {
                answer = value;
                NotifyOfPropertyChange("Answer");
            }
        }

        private static BindableCollection<ButtonModel> ConvertToButtons(IEnumerable<Answer> possibleAnswers)
        {
            if(possibleAnswers.Contains(Answer.Yes))
            {
                return possibleAnswers.Contains(Answer.Cancel)
                           ? new BindableCollection<ButtonModel>{new ButtonModel(LocalizeAnswer(Answer.Yes)), new ButtonModel(LocalizeAnswer(Answer.No)), new ButtonModel(LocalizeAnswer(Answer.Cancel))}
                           : new BindableCollection<ButtonModel> {new ButtonModel(LocalizeAnswer(Answer.Yes)), new ButtonModel(LocalizeAnswer(Answer.No))};
            }

            return possibleAnswers.Contains(Answer.Cancel)
                       ? new BindableCollection<ButtonModel> {new ButtonModel(LocalizeAnswer(Answer.Ok)), new ButtonModel(LocalizeAnswer(Answer.Cancel))}
                       : new BindableCollection<ButtonModel> {new ButtonModel(LocalizeAnswer(Answer.Ok))};
        }
    }
}