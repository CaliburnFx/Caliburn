namespace Caliburn.Unity
{
    using System;
    using System.Collections.Generic;
    using Core.Behaviors;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.ObjectBuilder;

    /// <summary>
    /// A <see cref="UnityContainerExtension"/> for proxies.
    /// </summary>
    public class ProxyExtension : UnityContainerExtension
    {
        private readonly IList<Type> registry = new List<Type>();

        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        /// <remarks>
        /// When overridden in a derived class, this method will modify the given
        /// <see cref="T:Microsoft.Practices.Unity.ExtensionContext"/> by adding strategies, policies, etc. to
        /// install it's functions into the container.</remarks>
        protected override void Initialize()
        {
            Context.Registering += (s, e) =>{
                if(!e.TypeTo.ShouldCreateProxy())
                    return;

                registry.Add(e.TypeTo);
            };

            var strategy = new ProxyBuilderStrategy(Container, registry);
            Context.Strategies.Add(strategy, UnityBuildStage.PreCreation);
        }
    }
}