namespace Caliburn.PresentationFramework
{
    using System.Windows;

#if SILVERLIGHT
    using System.Collections.Generic;
#endif

#if !SILVERLIGHT
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
#else
    /// <summary>
    /// A collection of triggers for routing messages through the UI.
    /// </summary>
    public class RoutedMessageTriggerCollection : List<BaseMessageTrigger>
    {
    }
#endif
}