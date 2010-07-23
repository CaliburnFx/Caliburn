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
        static readonly ILog Log = LogManager.GetLog(typeof(DefaultViewModelFactory));
        readonly IServiceLocator serviceLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewModelFactory"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public DefaultViewModelFactory(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        /// <summary>
        /// Creates a view model.
        /// </summary>
        /// <typeparam name="T">The view model's type.</typeparam>
        /// <returns>The view model.</returns>
        public T Create<T>()
        {
            return serviceLocator.GetInstance<T>();
        }

        /// <summary>
        /// Creates a screen for the given subject.
        /// </summary>
        /// <typeparam name="T">The subject's type.</typeparam>
        /// <param name="subject">The subject.</param>
        /// <returns>The screen.</returns>
        public IHaveSubject<T> CreateFor<T>(T subject)
        {
            var subjectHost = serviceLocator.GetAllInstances<IHaveSubject<T>>()
                .FirstOrDefault();

            if(subjectHost == null)
            {
                subjectHost = new Screen<T>();
                Log.Warn("Screen not found for subject {0}.  Created default Screen<T>.", subject);
            }

            return subjectHost.WithSubject(subject);
        }
    }
}