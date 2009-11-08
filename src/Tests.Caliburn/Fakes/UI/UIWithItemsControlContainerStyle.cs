using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Tests.Caliburn.Fakes.UI
{
    public class UIWithItemsControlContainerStyle : UserControl
    {
        public UIWithItemsControlContainerStyle()
        {
            var stack = new StackPanel();
            Content = stack;

            var itemsControl = new ItemsControl();
            itemsControl.ItemContainerStyle = CreateStyle();

            stack.Children.Add(itemsControl);
        }

        private Style CreateStyle()
        {
            var style = new Style();

            style.Setters.Add(
                new Setter(
                    TextBlock.FontFamilyProperty,
                    new Binding("FontFamily") //property does not exist
                    )
                );

            return style;
        }
    }
}
