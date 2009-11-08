using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Tests.Caliburn.Fakes.UI
{
    public class UIBoundToCustomerWithContentControl : UserControl
    {
        public UIBoundToCustomerWithContentControl()
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
            txtBlock.SetBinding(TextBlock.TextProperty, new Binding("Street")); //misspelling
            template.VisualTree.AppendChild(txtBlock);

            txtBlock = new FrameworkElementFactory(typeof(TextBlock));
            txtBlock.SetBinding(TextBlock.TextProperty, new Binding("City")); 
            template.VisualTree.AppendChild(txtBlock);

            return template;
        }
    }
}