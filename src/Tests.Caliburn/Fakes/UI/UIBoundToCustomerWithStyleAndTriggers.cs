namespace Tests.Caliburn.Fakes.UI
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class UIBoundToCustomerWithStyleAndTriggers : UserControl
    {
        public UIBoundToCustomerWithStyleAndTriggers()
        {
            var stack = new StackPanel();
            Content = stack;

            var textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("FirstName"));
            stack.Children.Add(textBlock);

            textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("LastName"));
            textBlock.Style = CreateStyle();

            stack.Children.Add(textBlock);
        }

        Style CreateStyle()
        {
            var style = new Style();

            var trigger = new DataTrigger();
            trigger.Binding = new Binding("Cty"); //misspelling
            trigger.Value = "Tallahassee";

            style.Triggers.Add(trigger);

            return style;
        }
    }
}