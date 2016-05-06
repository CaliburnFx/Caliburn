namespace Caliburn.Core.InversionOfControl
{
    using System.Collections.Generic;

    /// <summary>
    /// Implemented by a class that can register components with an IoC container.
    /// </summary>
    public interface IRegistry
    {
        /// <summary>
        /// Configures the container using the provided component registrations.
        /// </summary>
        /// <param name="registrations">The component registrations.</param>
        void Register(IEnumerable<IComponentRegistration> registrations);
    }
}