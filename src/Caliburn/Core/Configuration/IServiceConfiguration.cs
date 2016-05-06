namespace Caliburn.Core.Configuration
{
    using InversionOfControl;

    /// <summary>
    /// Represents a service configuration for a module.
    /// </summary>
    public interface IServiceConfiguration
    {
        /// <summary>
        /// Creates the registration.
        /// </summary>
        /// <returns></returns>
        IComponentRegistration CreateRegistration();

        /// <summary>
        /// Configures the service.
        /// </summary>
        /// <param name="locator">The locator.</param>
        void ConfigureService(IServiceLocator locator);
    }
}