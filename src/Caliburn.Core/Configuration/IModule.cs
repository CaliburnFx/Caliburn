namespace Caliburn.Core.Configuration
{
    using System.Collections.Generic;
    using IoC;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Implemented by modules.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Gets the component information for this module.
        /// </summary>
        IEnumerable<IComponentRegistration> GetComponents();

        /// <summary>
        /// Initializes this module.
        /// </summary>
        void Initialize(IServiceLocator serviceLocator);
    }
}