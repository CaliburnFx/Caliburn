namespace Caliburn.Core.IoC
{
    using Behaviors;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// An abstraction that represents a configurable service locator by composing <see cref="IServiceLocator"/> and <see cref="IRegistry"/>.
    /// </summary>
    public interface IContainer : IServiceLocator, IRegistry
    {
        /// <summary>
        /// Installs a proxy factory.
        /// </summary>
        /// <typeparam name="T">The type of the proxy factory.</typeparam>
        /// <returns>A container with an installed proxy factory.</returns>
        IContainer WithProxyFactory<T>()
            where T : IProxyFactory;
    }
}