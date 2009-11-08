namespace CompositeCommands
{
    using System.Threading;
    using System.Windows;
    using Caliburn.PresentationFramework.Actions;
    using Caliburn.PresentationFramework.Filters;

    public class ShowMessageCommand
    {
        public bool CanExecute(string message)
        {
            return !string.IsNullOrEmpty(message);
        }

        //Note: A command must have a method named 'Execute'
        //Note: The 'Execute' method inherits all the features available to actions.
        [Preview("CanExecute")]
        [AsyncAction(Callback = "Callback", BlockInteraction = true)]
        public string Execute(string message)
        {
            Thread.Sleep(2000);
            return "Your message: " + message;
        }

        public void Callback(string message)
        {
            MessageBox.Show(message);
        }
    }
}