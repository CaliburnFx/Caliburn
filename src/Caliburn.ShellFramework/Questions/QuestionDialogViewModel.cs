namespace Caliburn.ShellFramework.Questions
{
    using System.Collections.Generic;
    using PresentationFramework;
    using PresentationFramework.Screens;

    public class QuestionDialogViewModel : Screen, IQuestionDialog
    {
        private string _caption = "Shell";

        public bool HasOneQuestion
        {
            get { return Questions.Count == 1; }
        }

        public bool HasMultipleQuestions
        {
            get { return Questions.Count > 1; }
        }

        public Question FirstQuestion
        {
            get { return Questions[0]; }
        }

        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                NotifyOfPropertyChange("Caption");
            }
        }

        public IObservableCollection<Question> Questions { get; private set; }

        public IObservableCollection<ButtonModel> Buttons
        {
            get
            {
                return HasMultipleQuestions
                           ? new BindableCollection<ButtonModel> {new ButtonModel("Ok")}
                           : FirstQuestion.Buttons;
            }
        }

        public void Setup(string caption, IEnumerable<Question> questions)
        {
            Caption = caption;
            Questions = new BindableCollection<Question>(questions);
        }

        public void Ok()
        {
            SelectAnswer(Answer.Ok);
        }

        public void Cancel()
        {
            SelectAnswer(Answer.Cancel);
        }

        public void Yes()
        {
            SelectAnswer(Answer.Yes);
        }

        public void No()
        {
            SelectAnswer(Answer.No);
        }

        private void SelectAnswer(Answer answer)
        {
            FirstQuestion.Answer = answer;
            Close();
        }
    }
}