namespace Caliburn.WPF.ApplicationFramework
{
    using Core;
    using PresentationFramework.ApplicationModel;

    public class Question : PropertyChangedBase, ISubordinate
    {
        private Answer _answer = Answer.No;

        public Question(IPresenter master, string text)
            : this(master, text, Answer.No, Answer.Yes, Answer.Cancel) {}

        public Question(IPresenter master, string text, params Answer[] possibleAnswers)
        {
            Master = master;
            Text = text;
            PossibleAnswers = new BindableEnumCollection<Answer>(possibleAnswers);
        }

        public IPresenter Master { get; private set; }
        public string Text { get; private set; }
        public BindableEnumCollection<Answer> PossibleAnswers { get; set; }

        public Answer Answer
        {
            get { return _answer; }
            set
            {
                _answer = value;
                NotifyOfPropertyChange("Answer");
            }
        }
    }
}