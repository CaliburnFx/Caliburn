namespace Caliburn.PresentationFramework.ViewModels
{
    using Screens;

    /// <summary>
    /// Implemented by services that create view models.
    /// </summary>
    public interface IViewModelFactory
    {
        /// <summary>
        /// Creates a view model.
        /// </summary>
        /// <typeparam name="T">The view model's type.</typeparam>
        /// <returns>The view model.</returns>
        T Create<T>();

        /// <summary>
        /// Creates a screen for the given subject.
        /// </summary>
        /// <typeparam name="T">The subject's type.</typeparam>
        /// <param name="subject">The subject.</param>
        /// <returns>The screen.</returns>
        IScreen<T> CreateFor<T>(T subject);
    }
}