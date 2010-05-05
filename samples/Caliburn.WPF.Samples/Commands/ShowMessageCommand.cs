namespace Commands
{
    using System.Windows;
    using Caliburn.PresentationFramework.Commands;

    //Note: Registering a command by key, using the class name by default (minus the word 'Command').
    //NOTE: Use the CommandAttribute to specify a different key.
    [Command]
    public class ShowMessageCommand
    {
        //Note: A command must have an entry point.  A method named 'Execute' is searched for by default.  
        //Note: Use the CommandAttribute.ExecuteMethod to specify a different method.
        //Note: The 'Execute' method inherits all the features available to actions.
        public void Execute(string message)
        {
            //Note: This is for demo purposes only.
            //Note: It is not a good practice to call MessageBox.Show from a non-View class.
            //Note: Consider implementing a MessageBoxService.
            MessageBox.Show(message);
        }

        //NOTE:  This is picked up automatically by the ActionFactory based on its naming convention.
        public bool CanExecute(string message)
        {
            return !string.IsNullOrEmpty(message);
        }
    }
}