namespace Tests.Caliburn.Fakes.UI
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class UIBoundToCustomerWithItemsControl : UserControl
    {
        public UIBoundToCustomerWithItemsControl()
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

            var listBox = new ListBox {
                ItemTemplate = CreateItemTemplate()
            };
            listBox.SetBinding(ListBox.ItemsSourceProperty, new Binding("Orders"));
            stack.Children.Add(listBox);
        }

        DataTemplate CreateItemTemplate()
        {
            var template = new DataTemplate();

            template.VisualTree = new FrameworkElementFactory(typeof(StackPanel));

            var txtBlock = new FrameworkElementFactory(typeof(TextBlock));
            txtBlock.SetBinding(TextBlock.TextProperty, new Binding("Quantity"));
            template.VisualTree.AppendChild(txtBlock);

            txtBlock = new FrameworkElementFactory(typeof(TextBlock));
            txtBlock.SetBinding(TextBlock.TextProperty, new Binding("Totl")); //misspelling
            template.VisualTree.AppendChild(txtBlock);

            return template;
        }
    }
}