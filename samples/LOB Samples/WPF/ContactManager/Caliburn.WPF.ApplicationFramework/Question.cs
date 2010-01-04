namespace Caliburn.WPF.ApplicationFramework
{
    using Core;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.Screens;

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
        }

        public IScreen Master { get; private set; }
        public string Text { get; private set; }
        public BindableEnumCollection<Answer> PossibleAnswers { get; set; }

        public Answer Answer
        {
            get { return _answer; }
            set
            {
                _answer = value;
                NotifyOfPropertyChange(() => Answer);
            }
        }
    }
}