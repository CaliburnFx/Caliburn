namespace Caliburn.Ninject
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Core;
    using Core.Behaviors;
    using global::Ninject;
    using global::Ninject.Injection;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// An <see cref="IInjectorFactory"/> which adds proxy capabilities.
    /// </summary>
    public class ProxyInjectorFactory : IInjectorFactory
    {
        private readonly DynamicMethodInjectorFactory _inner = new DynamicMethodInjectorFactory();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _inner.Dispose();
        }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public INinjectSettings Settings
        {
            get { return _inner.Settings; }
            set { _inner.Settings = value; }
        }

        /// <summary>
        /// Creates the specified constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns></returns>
        public ConstructorInjector Create(ConstructorInfo constructor)
        {
            if (typeof(IProxyFactory).IsAssignableFrom(constructor.DeclaringType))
                return _inner.Create(constructor);

            var factory = ServiceLocator.Current.GetInstance<IProxyFactory>();

            return constructor.DeclaringType.ShouldCreateProxy()
                       ? new ProxyHelper(factory, constructor.DeclaringType).CreateConstructor
                       : _inner.Create(constructor);
        }

        /// <summary>
        /// Creates the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public PropertyInjector Create(PropertyInfo property)
        {
            return _inner.Create(property);
        }

        /// <summary>
        /// Creates the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public MethodInjector Create(MethodInfo method)
        {
            return _inner.Create(method);
        }

        private class ProxyHelper
        {
            private readonly IProxyFactory _factory;
            private readonly Type _implementation;
            private readonly IBehavior[] _behaviors;

            public ProxyHelper(IProxyFactory factory, Type implementation)
            {
                _factory = factory;
                _implementation = implementation;
                _behaviors = implementation.GetAttributes<IBehavior>(true).ToArray();
            }

            public object CreateConstructor(params object[] args)
            {
                return _factory.CreateProxy(
                    _implementation,
                    _behaviors,
                    args
                    );
            }
        }
    }
}