using System.Windows.Controls;
using System.Windows.Data;

namespace Tests.Caliburn.Fakes.UI
{
    public class UIBoundToCustomerIndexer: UserControl
    {
        public UIBoundToCustomerIndexer()
        {
            var stack = new StackPanel();
            Content = stack;

            var textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("FirstName"));
            stack.Children.Add(textBlock);

            var contentControl = new ContentControl();
            contentControl.SetBinding(ContentControl.ContentProperty, new Binding("[0].Quantity"));
            stack.Children.Add(contentControl);

            contentControl = new ContentControl();
            contentControl.SetBinding(ContentControl.ContentProperty, new Binding("[text].Quaity")); //purposefully misspelled
            stack.Children.Add(contentControl);
        }
    }
}