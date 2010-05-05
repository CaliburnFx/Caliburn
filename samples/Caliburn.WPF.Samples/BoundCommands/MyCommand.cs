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
            //Note: This is for demo purposes only.
            //Note: It is not a good practice to call MessageBox.Show from a non-View class.
            //Note: Consider implementing a MessageBoxService.
            MessageBox.Show(message);
        }

        public bool CanExecute(string message)
        {
            return !string.IsNullOrEmpty(message);
        }
    }
}