using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Tests.Caliburn.Fakes.UI
{
    public class UIBoundToCustomerWithContentControlAndMultiTriggers : UserControl
    {
        public UIBoundToCustomerWithContentControlAndMultiTriggers()
        {
            var stack = new StackPanel();
            Content = stack;

            var textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("FirstName"));
            stack.Children.Add(textBlock);

            textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("LastName"));
            stack.Children.Add(textBlock);

            var contentControl = new ContentControl {ContentTemplate = CreateItemTemplate()};
            contentControl.SetBinding(ContentControl.ContentProperty, new Binding("MailingAddress"));
            stack.Children.Add(contentControl);
        }

        private DataTemplate CreateItemTemplate()
        {
            var template = new DataTemplate();

            template.VisualTree = new FrameworkElementFactory(typeof(StackPanel));

            var txtBlock = new FrameworkElementFactory(typeof(TextBlock));
            txtBlock.SetBinding(TextBlock.TextProperty, new Binding("Street1")); 
            template.VisualTree.AppendChild(txtBlock);

            txtBlock = new FrameworkElementFactory(typeof(TextBlock));
            txtBlock.SetBinding(TextBlock.TextProperty, new Binding("City")); 
            template.VisualTree.AppendChild(txtBlock);

            var trigger = new MultiDataTrigger();

            trigger.Conditions.Add(
                new Condition(
                    new Binding("Cty"), //misspelling
                    "Tallahassee"
                    )
                );

            trigger.Conditions.Add(
                new Condition(
                    new Binding("StateOrProvince"),
                    "Florida"
                    )
                );

            template.Triggers.Add(trigger);

            return template;
        }
    }
}
