#if !WP7

namespace Caliburn.ShellFramework.Controls
{
    using System.ComponentModel;
    using System.Windows;
    using PresentationFramework;

#if SILVERLIGHT
    using System.Windows.Controls;
#endif

    public class DialogChrome 
#if SILVERLIGHT
        : ChildWindow
#else
        : Window
#endif
    {
        public static DependencyProperty ButtonsProperty =
            DependencyProperty.Register(
                "Buttons",
                typeof(IObservableCollection<ButtonModel>),
                typeof(DialogChrome),
                null
                );

        public DialogChrome()
        {
            DefaultStyleKey = typeof(DialogChrome);
        }

        [TypeConverter(typeof(ButtonConverter))]
        public IObservableCollection<ButtonModel> Buttons
        {
            get { return GetValue(ButtonsProperty) as IObservableCollection<ButtonModel>; }
            set { SetValue(ButtonsProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var chrome = GetTemplateChild("Chrome") as UIElement;
            if (chrome != null && Title == null) chrome.Visibility = Visibility.Collapsed;
        }
    }
}

#endif