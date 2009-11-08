namespace BoundCommands
{
    using System.Windows;
    using Caliburn.PresentationFramework.Filters;

    public class MyCommand
    {
        //Note: A command must have a method named 'Execute'
        //Note: The 'Execute' method inherits all the features available to actions.
        [Preview("CanExecute")]
        public void Execute(string message)
        {
            MessageBox.Show(message);
        }

        public bool CanExecute(string message)
        {
            return !string.IsNullOrEmpty(message);
        }
    }
}