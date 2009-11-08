namespace Commands
{
    using System.Windows;
    using Caliburn.PresentationFramework.Filters;

    public class ShowTitledMessageCommand
    {
        //Note: A command must have a method named 'Execute'
        //Note: The 'Execute' method inherits all the features available to actions.
        [Preview("CanExecute")]
        public void Execute(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK);
        }

        public bool CanExecute(string title, string message)
        {
            return !string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(message);
        }
    }
}