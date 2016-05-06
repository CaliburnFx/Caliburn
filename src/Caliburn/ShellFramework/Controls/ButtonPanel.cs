namespace Caliburn.ShellFramework.Controls
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using Core;
    using PresentationFramework;

    /// <summary>
    /// An <see cref="ItemsControl"/> that hosts instances of <see cref="ButtonModel"/>.
    /// </summary>
    public class ButtonPanel : ItemsControl
    {
        /// <summary>
        /// The Buttons dependency property.
        /// </summary>
        public static DependencyProperty ButtonsProperty =
            DependencyProperty.Register(
                "Buttons",
                typeof(IObservableCollection<ButtonModel>),
                typeof(ButtonPanel),
                new PropertyMetadata(ButtonsChanged)
                );

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonPanel"/> class.
        /// </summary>
        public ButtonPanel()
        {
            Visibility = Visibility.Collapsed;
            DefaultStyleKey = typeof(ButtonPanel);
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

        private static void ButtonsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var list = e.NewValue as IObservableCollection<ButtonModel>;
            var panel = (ItemsControl)d;

            panel.Items.Clear();

            if(list != null)
                list.Apply(x => panel.Items.Add(x));

            panel.Visibility = panel.Items.Count < 1
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }
}