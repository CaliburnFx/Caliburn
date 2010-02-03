namespace Caliburn.Spring
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Core.Behaviors;
    using Core.IoC;
    using global::Spring.Objects.Factory.Config;

    /// <summary>
    /// An <see cref="IObjectPostProcessor"/> which proxies objects.
    /// </summary>
    public class ProxyPostProcessor : IObjectPostProcessor
    {
        private readonly SpringAdapter _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyPostProcessor"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public ProxyPostProcessor(SpringAdapter container)
        {
            _container = container;
        }

        /// <summary>
        /// Apply this <see cref="T:Spring.Objects.Factory.Config.IObjectPostProcessor"/>
        /// to the given new object instance <i>before</i> any object initialization callbacks.
        /// </summary>
        /// <param name="instance">The new object instance.</param>
        /// <param name="name">The name of the object.</param>
        /// <returns>
        /// The object instance to use, either the original or a wrapped one.
        /// </returns>
        /// <remarks>
        /// 	<p>
        /// The object will already be populated with property values.
        /// The returned object instance may be a wrapper around the original.
        /// </p>
        /// </remarks>
        /// <exception cref="T:Spring.Objects.ObjectsException">
        /// In case of errors.
        /// </exception>
        public object PostProcessBeforeInitialization(object instance, string name)
        {
            var type = instance.GetType();

            if(!type.ShouldCreateProxy())
                return instance;

            var factory = _container.GetInstance<IProxyFactory>();

            return factory.CreateProxy(
                type,
                type.GetAttributes<IBehavior>(true).ToArray(),
                DetermineConstructorArgs(type)
                );
        }

        /// <summary>
        /// Apply this <see cref="T:Spring.Objects.Factory.Config.IObjectPostProcessor"/> to the
        /// given new object instance <i>after</i> any object initialization callbacks.
        /// </summary>
        /// <param name="instance">The new object instance.</param>
        /// <param name="objectName">The name of the object.</param>
        /// <returns>
        /// The object instance to use, either the original or a wrapped one.
        /// </returns>
        /// <remarks>
        /// 	<p>
        /// The object will already be populated with property values. The returned object
        /// instance may be a wrapper around the original.
        /// </p>
        /// </remarks>
        /// <exception cref="T:Spring.Objects.ObjectsException">
        /// In case of errors.
        /// </exception>
        public object PostProcessAfterInitialization(object instance, string objectName)
        {
            return instance;
        }

        private object[] DetermineConstructorArgs(Type implementation)
        {
            var args = new List<object>();
            var greedyConstructor = implementation.SelectEligibleConstructor();

            if(greedyConstructor != null)
            {
                foreach(var info in greedyConstructor.GetParameters())
                {
                    var arg = _container.GetInstance(info.ParameterType);
                    args.Add(arg);
                }
            }

            return args.ToArray();
        }
    }
}