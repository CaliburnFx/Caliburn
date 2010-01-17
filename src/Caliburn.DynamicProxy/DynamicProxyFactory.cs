namespace Caliburn.DynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.DynamicProxy;
    using Configuration;
    using Core;
    using Core.Behaviors;
    using Interceptors;
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
        public DynamicProxyFactory()
        {
            AddConfiguration<NotifyPropertyChangedAttribute, NotifyPropertyChangedConfiguration>();
            AddConfiguration<ScreenAttribute, ScreenConfiguration>();
        }

        /// <summary>
        /// Adds a behavior configuration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        public void AddConfiguration<T, K>()
            where K : IBehaviorConfiguration<T>, new()
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
        public object CreateProxy(Type type, IEnumerable<IBehavior> behaviors, IEnumerable<object> constructorArgs)
        {
            var interfaces = behaviors.SelectMany(x => x.GetInterfaces(type))
                .Distinct()
                .ToArray();

            var interceptors = (from behavior in behaviors
                                from intercepor in _configrations[behavior.GetType()]
                                    .GetInterceptors(type, behavior)
                                select intercepor).Distinct().ToArray();

            var proxy = proxyGenerator.CreateClassProxy(
                type,
                interfaces,
                ProxyGenerationOptions.Default,
                constructorArgs.ToArray(),
                interceptors
                );

            interceptors.OfType<IInitializableInterceptor>()
                .Apply(x => x.Initialize(proxy));

            return proxy;
        }
    }
}