namespace Caliburn.PresentationFramework.ViewModels
{
    using System.Linq;
    using Core.Logging;
    using Microsoft.Practices.ServiceLocation;
    using Screens;

    /// <summary>
    /// The default implementation of <see cref="IViewModelFactory"/>.
    /// </summary>
    public class DefaultViewModelFactory : IViewModelFactory
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(DefaultViewModelFactory));
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
            var screen = _serviceLocator.GetAllInstances<IScreen<T>>()
                             .FirstOrDefault();

            if(screen == null)
            {
                screen = new Screen<T>();
                Log.Warn("Screen not found for subject {0}.  Created default Screen<T>.", subject);
            }

            return screen.WithSubject(subject);
        }
    }
}