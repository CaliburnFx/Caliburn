namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using System.Windows;

    /// <summary>
    /// A collection of triggers for routing messages through the UI.
    /// </summary>
    public class RoutedMessageTriggerCollection : FreezableCollection<BaseMessageTrigger>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:System.Windows.FreezableCollection`1"/>.
        /// </summary>
        /// <returns>The new instance.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new RoutedMessageTriggerCollection();
        }
    }
}
