namespace Tests.Caliburn.Fakes.UI
{
    using System.Windows.Controls;
    using System.Windows.Data;

    public class UIBoundToCustomerIndexer : UserControl
    {
        public UIBoundToCustomerIndexer()
        {
            var stack = new StackPanel();
            Content = stack;

            var textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("FirstName"));
            stack.Children.Add(textBlock);

            var contentControl = new ContentControl();
            contentControl.SetBinding(ContentProperty, new Binding("[0].Quantity"));
            stack.Children.Add(contentControl);

            contentControl = new ContentControl();
            contentControl.SetBinding(ContentProperty, new Binding("[text].Quaity")); //purposefully misspelled
            stack.Children.Add(contentControl);
        }
    }
}