namespace Caliburn.PresentationFramework.ViewModels
{
    using Microsoft.Practices.ServiceLocation;
    using Screens;

    /// <summary>
    /// The default implementation of <see cref="IViewModelFactory"/>.
    /// </summary>
    public class DefaultViewModelFactory : IViewModelFactory
    {
        private readonly IServiceLocator _serviceLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewModelFactory"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public DefaultViewModelFactory(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        /// <summary>
        /// Creates a view model.
        /// </summary>
        /// <typeparam name="T">The view model's type.</typeparam>
        /// <returns>The view model.</returns>
        public T Create<T>()
        {
            return _serviceLocator.GetInstance<T>();
        }

        /// <summary>
        /// Creates a screen for the given subject.
        /// </summary>
        /// <typeparam name="T">The subject's type.</typeparam>
        /// <param name="subject">The subject.</param>
        /// <returns>The screen.</returns>
        public IScreen<T> CreateFor<T>(T subject)
        {
            return _serviceLocator.GetInstance<IScreen<T>>()
                .WithSubject(subject);
        }
    }
}