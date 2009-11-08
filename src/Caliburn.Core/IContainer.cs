namespace Caliburn.Core
{
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// An abstraction that represents a configurable service locator by composing <see cref="IServiceLocator"/> and <see cref="IConfigurator"/>.
    /// </summary>
    public interface IContainer : IServiceLocator, IConfigurator
    {
        
    }
}