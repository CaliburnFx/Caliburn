namespace Caliburn.Core.Configuration
{
    using Invocation;
    using IoC;
    using Threading;

    /// <summary>
    /// Describes the services requred for the core framework to function.
    /// </summary>
    public interface ICoreServicesDescription
    {
        /// <summary>
        /// Customizes the thread pool used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The thread pool type.</typeparam>
        Singleton ThreadPool<T>() where T : IThreadPool;

        /// <summary>
        /// Customizes the method factory used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The method factory type.</typeparam>
        Singleton MethodFactory<T>() where T : IMethodFactory;

        /// <summary>
        /// Customizes the event handler factory.
        /// </summary>
        /// <typeparam name="T">The event handler factory type.</typeparam>
        Singleton EventHandlerFactory<T>() where T : IEventHandlerFactory;

        /// <summary>
        /// Customizes the dispatcher implementation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        Singleton Dispatcher<T>() where T : IDispatcher;

        /// <summary>
        /// Usings the assembly source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        Singleton AssemblySource<T>() where T : IAssemblySource;
    }
}