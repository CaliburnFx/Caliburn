namespace Caliburn.ShellFramework.Questions
{
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using PresentationFramework;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;

    public class Question : PropertyChangedBase, ISubordinate
    {
        private Answer _answer = Answer.No;

        public Question(IScreen master, string text)
            : this(master, text, Answer.No, Answer.Yes, Answer.Cancel) {}

        public Question(IScreen master, string text, params Answer[] possibleAnswers)
        {
            Master = master;
            Text = text;
            PossibleAnswers = new BindableEnumCollection<Answer>(possibleAnswers);
            Buttons = ConvertToButtons(possibleAnswers);
        }

        public IScreen Master { get; private set; }
        public string Text { get; private set; }
        public BindableEnumCollection<Answer> PossibleAnswers { get; set; }
        public IObservableCollection<ButtonModel> Buttons { get; set; }

        public Answer Answer
        {
            get { return _answer; }
            set
            {
                _answer = value;
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