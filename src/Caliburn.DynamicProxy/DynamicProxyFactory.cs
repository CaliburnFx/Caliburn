namespace Caliburn.DynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Interceptor;
    using Castle.DynamicProxy;
    using Core.Behaviors;
    using Core.IoC;
    using Microsoft.Practices.ServiceLocation;
    using PresentationFramework.Behaviors;

    /// <summary>
    /// An implementation of <see cref="IProxyFactory"/> using DynamicProxy2.
    /// </summary>
    public class DynamicProxyFactory : IProxyFactory
    {
        private readonly ProxyGenerator proxyGenerator = new ProxyGenerator();

        private readonly Dictionary<Type, IBehaviorConfiguration> _configrations =
            new Dictionary<Type, IBehaviorConfiguration>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicProxyFactory"/> class.
        /// </summary>
        /// <param name="registry">The registry.</param>
        public DynamicProxyFactory(IRegistry registry)
        {
            AddConfiguration<NotifyPropertyChangedAttribute, NotifyPropertyChangedConfiguration>();

            registry.Register(new IComponentRegistration[]
            {
                new PerRequest
                {
                    Service = typeof(NotifyPropertyChangedWithInterfaceInterceptor),
                    Implementation = typeof(NotifyPropertyChangedWithInterfaceInterceptor)
                },
                new PerRequest
                {
                    Service = typeof(NotifyPropertyChangedNoInterfaceInterceptor),
                    Implementation = typeof(NotifyPropertyChangedNoInterfaceInterceptor)
                },
                new Singleton
                {
                    Service = typeof(ProxyInterceptor),
                    Implementation = typeof(ProxyInterceptor)
                }
            });
        }

        /// <summary>
        /// Adds a behavior configuration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        public void AddConfiguration<T, K>()
            where K : IBehaviorConfiguration, new()
            where T : IBehavior
        {
            _configrations[typeof(T)] = new K();
        }

        /// <summary>
        /// Creates a proxy.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="behaviors">The proxy behaviors.</param>
        /// <param name="constructorArgs">The constructor args.</param>
        /// <returns>The proxy.</returns>
        public object CreateProxy(Type type, IBehavior[] behaviors, object[] constructorArgs)
        {
            var interfaces = behaviors.SelectMany(x => x.GetInterfaces(type))
                .Distinct()
                .ToArray();

            var interceptors = behaviors.Select(x => _configrations[x.GetType()])
                .SelectMany(x => x.GetInterceptors(type))
                .Distinct()
                .Select(x => ServiceLocator.Current.GetInstance(x) as IInterceptor)
                .ToArray();

            return proxyGenerator.CreateClassProxy(
                type,
                interfaces,
                ProxyGenerationOptions.Default,
                constructorArgs,
                interceptors
                );
        }
    }
}