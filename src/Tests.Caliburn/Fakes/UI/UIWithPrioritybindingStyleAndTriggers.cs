namespace Tests.Caliburn.Fakes.UI
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class UIWithPrioritybindingStyleAndTriggers : UserControl
    {
        public UIWithPrioritybindingStyleAndTriggers()
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

            var multibinding = new PriorityBinding();
            multibinding.Bindings.Add(new Binding("FirstName"));
            multibinding.Bindings.Add(new Binding("LstName")); //purposefully misspelled

            var trigger = new DataTrigger();
            trigger.Binding = multibinding;
            trigger.Value = "Rob Eisenberg";

            style.Triggers.Add(trigger);

            return style;
        }
    }
}