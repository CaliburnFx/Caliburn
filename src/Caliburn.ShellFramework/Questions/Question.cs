namespace Caliburn.ShellFramework.Questions
{
    using System.Collections.Generic;
    using System.Linq;
    using PresentationFramework;
    using PresentationFramework.ViewModels;

    public class Question : PropertyChangedBase
    {
        private Answer answer = Answer.No;

        public Question(string text)
            : this(text, Answer.No, Answer.Yes, Answer.Cancel) {}

        public Question(string text, params Answer[] possibleAnswers)
        {
            Text = text;
            PossibleAnswers = new BindableEnumCollection<Answer>(possibleAnswers);
            Buttons = ConvertToButtons(possibleAnswers);
        }

        public string Text { get; private set; }
        public BindableEnumCollection<Answer> PossibleAnswers { get; set; }
        public IObservableCollection<ButtonModel> Buttons { get; set; }

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
                           ? new BindableCollection<ButtonModel>{new ButtonModel("Yes"), new ButtonModel("No"), new ButtonModel("Cancel")}
                           : new BindableCollection<ButtonModel> {new ButtonModel("Yes"), new ButtonModel("No")};
            }

            return possibleAnswers.Contains(Answer.Cancel)
                       ? new BindableCollection<ButtonModel> {new ButtonModel("Ok"), new ButtonModel("Cancel")}
                       : new BindableCollection<ButtonModel> {new ButtonModel("Ok")};
        }
    }
}