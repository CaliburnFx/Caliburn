using System.Windows.Controls;
using System.Windows.Data;

namespace Tests.Caliburn.Fakes.UI
{
    public class UIBoundToCustomerWithNesting : UserControl
    {
        public UIBoundToCustomerWithNesting()
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
            stack.Children.Add(addressStack);

            textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("MailingAddress.Street")); //misspelling
            addressStack.Children.Add(textBlock);

            textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("MailingAddress.City"));
            addressStack.Children.Add(textBlock);

            textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("BllingAddress.Street2")); //misspelling
            addressStack.Children.Add(textBlock);
        }
    }
}