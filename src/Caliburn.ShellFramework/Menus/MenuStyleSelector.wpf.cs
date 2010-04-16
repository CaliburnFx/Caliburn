#if !SILVERLIGHT

namespace Caliburn.ShellFramework.Menus
{
    using System.Windows;
    using System.Windows.Controls;

    public class MenuStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if(((MenuModel)item).IsSeparator)
                return (Style)((FrameworkElement)container).FindResource("menuSeparatorStyle");
            return (Style)((FrameworkElement)container).FindResource("menuStyle");
        }
    }
}

#endif