namespace Caliburn.PresentationFramework
{
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// Describes how a UI is affected by the availability of a message.
    /// </summary>
    [TypeConverter(typeof(AvailabilityEffectConverter))]
    public interface IAvailabilityEffect
    {
        /// <summary>
        /// Applies the effect to the target.
        /// </summary>
        /// <param name="target">The element.</param>
        /// <param name="isAvailable">Determines how the effect will be applied to the target.</param>
        void ApplyTo(DependencyObject target, bool isAvailable);
    }
}