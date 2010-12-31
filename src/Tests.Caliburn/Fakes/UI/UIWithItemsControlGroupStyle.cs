namespace Tests.Caliburn.Fakes.UI
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class UIWithItemsControlGroupStyle : UserControl
    {
        public UIWithItemsControlGroupStyle()
        {
            var stack = new StackPanel();
            Content = stack;

            var itemsControl = new ItemsControl();

            itemsControl.GroupStyle.Add(
                new GroupStyle {
                    ContainerStyle = CreateStyle()
                });

            stack.Children.Add(itemsControl);
        }

        Style CreateStyle()
        {
            var style = new Style();
            style.TargetType = typeof(GroupItem);

            style.Setters.Add(
                new Setter(
                    TextBlock.FontFamilyProperty,
                    new Binding("FontFamily") //property does not exist
                    )
                );

            style.Setters.Add(
                new Setter(
                    TextBlock.FontFamilyProperty,
                    new Binding("Name")
                    )
                );

            style.Setters.Add(
                new Setter(
                    TextBlock.FontFamilyProperty,
                    new Binding("ItemCount")
                    )
                );

            return style;
        }
    }
}