namespace Caliburn.PresentationFramework.Commands
{
    using System.Windows;

    /// <summary>
    /// Hosts attached properties related to commands.
    /// </summary>
    public static class Command
    {
        /// <summary>
        /// A property representing the availability effect of a given message.
        /// </summary>
        public static readonly DependencyProperty ParentProperty =
            DependencyProperty.RegisterAttached(
                "Parent",
                typeof(ICompositeCommand),
                typeof(Command),
                null
                );

        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="parent">The parent.</param>
        public static void SetParent(DependencyObject d, ICompositeCommand parent)
        {
            d.SetValue(ParentProperty, parent);
        }

        /// <summary>
        /// Gets the triggers.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static ICompositeCommand GetParent(DependencyObject d)
        {
            return d.GetValue(ParentProperty) as ICompositeCommand;
        }
    }
}