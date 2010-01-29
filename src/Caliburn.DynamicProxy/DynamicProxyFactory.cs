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
        private readonly ProxyGenerator _proxyGenerator;

        private readonly Dictionary<Type, IBehaviorConfiguration> _configrations =
            new Dictionary<Type, IBehaviorConfiguration>();

        //private ModuleScope _scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicProxyFactory"/> class.
        /// </summary>
        public DynamicProxyFactory()
        {
            AddConfiguration<NotifyPropertyChangedAttribute, NotifyPropertyChangedConfiguration>();
            AddConfiguration<ScreenAttribute, ScreenConfiguration>();
            AddConfiguration<ValidateAttribute, ValidateConfiguration>();

            //var savePhysicalAssembly = true; 
            //var strongAssemblyName = ModuleScope.DEFAULT_ASSEMBLY_NAME; 
            //var strongModulePath = ModuleScope.DEFAULT_FILE_NAME; 
            //var weakAssemblyName = "Caliburn.Proxies";
            //var weakModulePath = "Caliburn.Proxies.dll"; 
            //_scope = new ModuleScope(savePhysicalAssembly, strongAssemblyName, strongModulePath, weakAssemblyName, weakModulePath); 
            //var builder = new DefaultProxyBuilder(_scope);
            //_proxyGenerator = new ProxyGenerator(builder);

            _proxyGenerator = new ProxyGenerator();
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
        /// Creates the proxy using the specified target.
        /// </summary>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <param name="target">The target.</param>
        /// <param name="behaviors">The behaviors.</param>
        /// <returns>The proxy.</returns>
        public object CreateProxyWithTarget(Type interfaceType, object target, IEnumerable<IBehavior> behaviors)
        {
            var targetType = target.GetType();
            var interfaces = behaviors.SelectMany(x => x.GetInterfaces(targetType))
                .Distinct()
                .ToArray();

            var interceptors = (from behavior in behaviors
                                from intercepor in _configrations[behavior.GetType()]
                                    .GetInterceptors(targetType, behavior)
                                select intercepor).Distinct().ToArray();

            var proxy = _proxyGenerator.CreateInterfaceProxyWithTarget(
                interfaceType,
                interfaces,
                target,
                ProxyGenerationOptions.Default,
                interceptors
                );

            interceptors.OfType<IInitializableInterceptor>()
                .Apply(x => x.Initialize(proxy));

            //_scope.SaveAssembly(false);

            return proxy;
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

            var proxy = _proxyGenerator.CreateClassProxy(
                type,
                interfaces,
                ProxyGenerationOptions.Default,
                constructorArgs == null ? new object[] {} : constructorArgs.ToArray(),
                interceptors
                );

            interceptors.OfType<IInitializableInterceptor>()
                .Apply(x => x.Initialize(proxy));

            //_scope.SaveAssembly(false);

            return proxy;
        }
    }
}