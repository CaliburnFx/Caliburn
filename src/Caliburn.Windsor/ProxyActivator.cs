namespace Caliburn.Windsor
{
    using System;
    using System.Linq;
    using Castle.Core;
    using Castle.MicroKernel;
    using Castle.MicroKernel.ComponentActivator;
    using Core;
    using Core.Behaviors;

    /// <summary>
    /// An <see cref="IComponentActivator"/> that adds proxy capabilities.
    /// </summary>
    public class ProxyActivator : DefaultComponentActivator
    {
        public ProxyActivator(ComponentModel model, IKernel kernel, ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction)
            : base(model, kernel, onCreation, onDestruction) {}

        /// <summary>
        /// Instantiates the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected override object Instantiate(CreationContext context)
        {
            var candidate = SelectEligibleConstructor(context);

            Type[] signature;
            var arguments = CreateConstructorArguments(candidate, context, out signature);

            return Kernel.Resolve<Core.Behaviors.IProxyFactory>()
                .CreateProxy(
                Model.Implementation,
                Model.Implementation.GetAttributes<IBehavior>(true).ToArray(),
                arguments
                );
        }
    }
}