namespace Caliburn.PresentationFramework.Conventions
{
    using ViewModels;
    using Views;

    /// <summary>
    /// A convention that applies to a view.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IViewConvention<T>
    {
        /// <summary>
        /// Tries the create an application of the convention.
        /// </summary>
        /// <param name="conventionManager">Convention manager.</param>
        /// <param name="description">The description.</param>
        /// <param name="element">The element.</param>
        /// <param name="target">The target.</param>
        /// <returns>The application or null if not applicable.</returns>
        IViewApplicable TryCreateApplication(IConventionManager conventionManager, IViewModelDescription description, ElementDescription element, T target);
    }
}