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
        /// Creates an item to host the given subject.
        /// </summary>
        /// <typeparam name="T">The subject's type.</typeparam>
        /// <param name="subject">The subject.</param>
        /// <returns>The item which hosts the subject.</returns>
        IHaveSubject<T> CreateFor<T>(T subject);
    }
}