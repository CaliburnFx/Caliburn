namespace Caliburn.Unity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Core.Behaviors;
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;

    public class ProxyBuilderStrategy : BuilderStrategy
    {
        private readonly IUnityContainer _container;
        private readonly IEnumerable<Type> _registry;

        public ProxyBuilderStrategy(IUnityContainer container, IEnumerable<Type> registry)
        {
            _container = container;
            _registry = registry;
        }

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
            var greedyConstructor = (from c in implementation.GetConstructors()
                                     orderby c.GetParameters().Length descending
                                     select c).FirstOrDefault();

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