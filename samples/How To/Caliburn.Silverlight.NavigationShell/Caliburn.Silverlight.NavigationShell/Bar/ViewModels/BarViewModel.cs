namespace Caliburn.Silverlight.NavigationShell.Bar.ViewModels
{
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.Screens;
    using ShellFramework.History;
    using ShellFramework.Questions;

    [HistoryKey("Bar", typeof(BarViewModel))]
    public class BarViewModel : Screen<Message>, ISupportCustomShutdown
    {
        public override bool CanShutdown()
        {
            return false;
        }

        public ISubordinate CreateShutdownModel()
        {
            return new Question(this, "Are you sure you want to navigate away from this page?", Answer.Yes, Answer.No);
        }

        public bool CanShutdown(ISubordinate shutdownModel)
        {
            var question = (Question)shutdownModel;
            return question.Answer == Answer.Yes;
        }

        //The following overrides insure that all instances of this screen are treated as
        //equal by the screen activation mechanism without forcing a singleton registration
        //in the container.
        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == GetType();
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }
    }
}