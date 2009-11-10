namespace Caliburn.Core.IoC
{
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// An abstraction that represents a configurable service locator by composing <see cref="IServiceLocator"/> and <see cref="IRegistry"/>.
    /// </summary>
    public interface IContainer : IServiceLocator, IRegistry
    {
        
    }
}