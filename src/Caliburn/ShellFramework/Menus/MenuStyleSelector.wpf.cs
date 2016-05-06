#if !SILVERLIGHT

namespace Caliburn.ShellFramework.Menus
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// A <see cref="StyleSelector"/> for use with the <see cref="MenuModel"/>.
    /// </summary>
    public class MenuStyleSelector : StyleSelector
    {
        /// <summary>
        /// When overridden in a derived class, returns a <see cref="T:System.Windows.Style"/> based on custom logic.
        /// </summary>
        /// <param name="item">The content.</param>
        /// <param name="container">The element to which the style will be applied.</param>
        /// <returns>
        /// Returns an application-specific style to apply; otherwise, null.
        /// </returns>
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if(((MenuModel)item).IsSeparator)
                return (Style)((FrameworkElement)container).FindResource("menuSeparatorStyle");
            return (Style)((FrameworkElement)container).FindResource("menuStyle");
        }
    }
}

#endif