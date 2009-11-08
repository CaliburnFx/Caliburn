using System.Windows.Controls;
using System.Windows.Data;

namespace Tests.Caliburn.Fakes.UI
{
    public class SimpleUIBoundToCustomer : UserControl
    {
        public SimpleUIBoundToCustomer()
        {
            var stack = new StackPanel();
            Content = stack;

            var textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("FirstName"));
            stack.Children.Add(textBlock);

            textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("LstName")); //purposefully misspelled
            stack.Children.Add(textBlock);

            textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding()); //context
            stack.Children.Add(textBlock);

            var checkbox = new CheckBox();
            checkbox.SetBinding(CheckBox.IsCheckedProperty, new Binding("asdfsdf")); //does not exist
            stack.Children.Add(checkbox);

            var tooltip = new ToolTip();
            tooltip.SetBinding(System.Windows.Controls.ToolTip.ContentProperty, new Binding("asdfasdasdf")); // does not exist
            stack.ToolTip = tooltip;

            var childUserControl = new SimpleUIBoundToCustomerByAttachedPorperty();
            stack.Children.Add(childUserControl);
        }
    }
}