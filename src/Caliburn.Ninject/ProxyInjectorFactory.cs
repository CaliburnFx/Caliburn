namespace Caliburn.Ninject
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Core;
    using Core.Behaviors;
    using Core.InversionOfControl;
    using global::Ninject;
    using global::Ninject.Injection;

    /// <summary>
    /// An <see cref="IInjectorFactory"/> which adds proxy capabilities.
    /// </summary>
    public class ProxyInjectorFactory : IInjectorFactory
    {
        private readonly DynamicMethodInjectorFactory inner = new DynamicMethodInjectorFactory();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            inner.Dispose();
        }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public INinjectSettings Settings
        {
            get { return inner.Settings; }
            set { inner.Settings = value; }
        }

        /// <summary>
        /// Creates the specified constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns></returns>
        public ConstructorInjector Create(ConstructorInfo constructor)
        {
            if (typeof(IProxyFactory).IsAssignableFrom(constructor.DeclaringType))
                return inner.Create(constructor);

            var factory = IoC.Get<IProxyFactory>();

            return constructor.DeclaringType.ShouldCreateProxy()
                       ? new ProxyHelper(factory, constructor.DeclaringType).CreateConstructor
                       : inner.Create(constructor);
        }

        /// <summary>
        /// Creates the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public PropertyInjector Create(PropertyInfo property)
        {
            return inner.Create(property);
        }

        /// <summary>
        /// Creates the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public MethodInjector Create(MethodInfo method)
        {
            return inner.Create(method);
        }

        private class ProxyHelper
        {
            private readonly IProxyFactory factory;
            private readonly Type implementation;
            private readonly IBehavior[] behaviors;

            public ProxyHelper(IProxyFactory factory, Type implementation)
            {
                this.factory = factory;
                this.implementation = implementation;
                behaviors = implementation.GetAttributes<IBehavior>(true).ToArray();
            }

            public object CreateConstructor(params object[] args)
            {
                return factory.CreateProxy(
                    implementation,
                    behaviors,
                    args
                    );
            }
        }
    }
}