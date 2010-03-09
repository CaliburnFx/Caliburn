namespace Caliburn.PresentationFramework.Views
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// A default view provided by Caliburn when an <see cref="IViewLocator"/> is unable to locate one.
    /// </summary>
    public class NotFoundView : TextBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundView"/> class.
        /// </summary>
        public NotFoundView(string message)
        {
            Text = message;
            TextWrapping = TextWrapping.Wrap;
            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            IsReadOnly = true;
        }
    }
}