namespace Caliburn.Unity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Core.Behaviors;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.ObjectBuilder;

    public class ProxyExtension : UnityContainerExtension
    {
        private readonly IList<Type> registry = new List<Type>();

        protected override void Initialize()
        {
            Context.Registering += (s, e) =>{
                if(!e.TypeTo.GetAttributes<IBehavior>(true).Any())
                    return;

                registry.Add(e.TypeTo);
            };

            var strategy = new ProxyBuilderStrategy(Container, registry);
            Context.Strategies.Add(strategy, UnityBuildStage.PreCreation);
        }
    }
}