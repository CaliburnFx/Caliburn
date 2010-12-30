namespace Caliburn.Unity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Core.Behaviors;
    using Core.InversionOfControl;
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// A <see cref="BuilderStrategy"/> for proxies.
    /// </summary>
    public class ProxyBuilderStrategy : BuilderStrategy
    {
        readonly IUnityContainer container;
        readonly IEnumerable<Type> registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyBuilderStrategy"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="registry">The registry.</param>
        public ProxyBuilderStrategy(IUnityContainer container, IEnumerable<Type> registry)
        {
            this.container = container;
            this.registry = registry;
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PreBuildUp(IBuilderContext context)
        {
#if SILVERLIGHT_30
            var key = BuildKey.GetType(context.BuildKey);
#else
            var key = context.BuildKey.Type;
#endif

            if(!registry.Contains(key))
                base.PreBuildUp(context);
            else
            {
                var proxyFactory = container.Resolve<IProxyFactory>();

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
                    var arg = container.Resolve(info.ParameterType);
                    args.Add(arg);
                }
            }

            return args.ToArray();
        }
    }
}