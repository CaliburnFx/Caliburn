using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Tests.Caliburn.Fakes.UI
{
    public class UIBoundToCustomerWithStyleAndMultiTriggers : UserControl
    {
        public UIBoundToCustomerWithStyleAndMultiTriggers()
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

        private Style CreateStyle()
        {
            var style = new Style();

            var trigger = new MultiDataTrigger();

            trigger.Conditions.Add(
                new Condition(
                    new Binding("Cty"), //misspelling
                    "Tallahassee"
                    )
                );

            trigger.Conditions.Add(
                new Condition(
                    new Binding("Age"),
                    21
                    )
                );

            style.Triggers.Add(trigger);

            return style;
        }
    }
}
