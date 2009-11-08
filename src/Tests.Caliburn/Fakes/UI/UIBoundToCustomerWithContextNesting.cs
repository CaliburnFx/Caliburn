using System.Windows.Controls;
using System.Windows.Data;

namespace Tests.Caliburn.Fakes.UI
{
    public class UIBoundToCustomerWithContextNesting : UserControl
    {
        public UIBoundToCustomerWithContextNesting()
        {
            var stack = new StackPanel();
            Content = stack;

            var textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("FirstName"));
            stack.Children.Add(textBlock);

            textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("LastName"));
            stack.Children.Add(textBlock);

            var addressStack = new StackPanel();
            addressStack.SetBinding(StackPanel.DataContextProperty, new Binding("MailingAddress"));
            stack.Children.Add(addressStack);

            textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("Street")); //misspelling
            addressStack.Children.Add(textBlock);

            textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("Something")); //does not exist
            addressStack.Children.Add(textBlock);

            textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("Street2"));
            addressStack.Children.Add(textBlock);




            addressStack = new StackPanel();
            addressStack.SetBinding(StackPanel.DataContextProperty, new Binding("BllingAddress")); //misspelled
            stack.Children.Add(addressStack);

            textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("Street")); //misspelling
            addressStack.Children.Add(textBlock);

            textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("Something")); //does not exist
            addressStack.Children.Add(textBlock);

            textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("Street2"));
            addressStack.Children.Add(textBlock);
        }
    }
}