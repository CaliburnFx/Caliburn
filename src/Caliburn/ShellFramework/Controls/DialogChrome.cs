namespace Caliburn.ShellFramework.Controls
{
    using System.ComponentModel;
    using System.Windows;
    using PresentationFramework;

    /// <summary>
    /// A custom chrome for dialogs.
    /// </summary>
    public class DialogChrome
        : Window
    {
        /// <summary>
        /// The Buttons dependency property.
        /// </summary>
        public static DependencyProperty ButtonsProperty =
            DependencyProperty.Register(
                "Buttons",
                typeof(IObservableCollection<ButtonModel>),
                typeof(DialogChrome),
                null
                );

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogChrome"/> class.
        /// </summary>
        public DialogChrome()
        {
            DefaultStyleKey = typeof(DialogChrome);
        }

        /// <summary>
        /// Gets or sets the buttons.
        /// </summary>
        /// <value>The buttons.</value>
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
