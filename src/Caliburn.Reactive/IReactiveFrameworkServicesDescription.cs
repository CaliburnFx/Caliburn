namespace Caliburn.Reactive
{
    using Core.Configuration;
    using Core.IoC;

    /// <summary>
    /// Describes the services required for Caliburn's Reactive Framework module.
    /// </summary>
    public interface IReactiveFrameworkServicesDescription
    {
        /// <summary>
        /// Customizes the event publisher used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The method factory type.</typeparam>
        IConfiguredRegistration<Singleton, T> EventPublisher<T>() where T : IEventPublisher;
    }
}