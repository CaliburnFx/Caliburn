namespace Tests.Caliburn.Fakes.UI
{
    using System.Windows.Controls;
    using System.Windows.Data;

    public class SimpleUIBoundToCustomerByAttachedPorperty : UserControl
    {
        public SimpleUIBoundToCustomerByAttachedPorperty()
        {
            var stack = new StackPanel();
            Content = stack;

            var textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("FirstName"));
            stack.Children.Add(textBlock);

            textBlock = new TextBlock();
            textBlock.SetBinding(Grid.ZIndexProperty, new Binding("Ag")); //purposefully misspelled
            stack.Children.Add(textBlock);

            textBlock = new TextBlock();
            textBlock.SetBinding(Grid.ColumnSpanProperty, new Binding("IQ"));
            stack.Children.Add(textBlock);
        }
    }
}