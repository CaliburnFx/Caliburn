namespace CompositeCommands
{
    using System.Threading;
    using System.Windows;
    using Caliburn.PresentationFramework.Actions;
    using Caliburn.PresentationFramework.Filters;

    public class ShowTitledMessageCommand
    {
        public bool CanExecute(string title, string message)
        {
            return !string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(message);
        }

        //Note: A command must have a method named 'Execute'
        //Note: The 'Execute' method inherits all the features available to actions.
        [Preview("CanExecute")]
        [AsyncAction(Callback = "Callback", BlockInteraction = true)]
        public MessageInfo Execute(string title, string message)
        {
            Thread.Sleep(4000);
            return new MessageInfo {Title = title, Message = message};
        }

        public void Callback(MessageInfo message)
        {
            MessageBox.Show(message.Message, message.Title, MessageBoxButton.OK);
        }

        public class MessageInfo
        {
            public string Title { get; set; }
            public string Message { get; set; }
        }
    }
}