namespace Caliburn.Unity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Core.Behaviors;
    using Core.IoC;
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// A <see cref="BuilderStrategy"/> for proxies.
    /// </summary>
    public class ProxyBuilderStrategy : BuilderStrategy
    {
        private readonly IUnityContainer _container;
        private readonly IEnumerable<Type> _registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyBuilderStrategy"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="registry">The registry.</param>
        public ProxyBuilderStrategy(IUnityContainer container, IEnumerable<Type> registry)
        {
            _container = container;
            _registry = registry;
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PreBuildUp(IBuilderContext context)
        {
            var key = BuildKey.GetType(context.BuildKey);

            if(!_registry.Contains(key))
                base.PreBuildUp(context);
            else
            {
                var proxyFactory = _container.Resolve<IProxyFactory>();

                context.Existing = proxyFactory.CreateProxy(
                    key,
                    key.GetAttributes<IBehavior>(true).ToArray(),
                    DetermineConstructorArgs(key)
                    );
            }
        }

        private object[] DetermineConstructorArgs(Type implementation)
        {
            var args = new List<object>();
            var greedyConstructor = implementation.SelectEligibleConstructor();

            if(greedyConstructor != null)
            {
                foreach(var info in greedyConstructor.GetParameters())
                {
                    var arg = _container.Resolve(info.ParameterType);
                    args.Add(arg);
                }
            }

            return args.ToArray();
        }
    }
}