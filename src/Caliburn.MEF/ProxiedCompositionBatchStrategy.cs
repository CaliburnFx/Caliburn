namespace Caliburn.MEF
{
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using Core;
	using Core.Behaviors;
	using Core.InversionOfControl;
	using Enumerable = System.Linq.Enumerable;

    /// <summary>
    /// A special batch strategy used for creating proxied parts.
    /// </summary>
	public class ProxiedCompositionBatchStrategy : CompositionBatchStrategy
	{
        /// <summary>
        /// Registers a per request.
        /// </summary>
        /// <param name="batch">The batch.</param>
        /// <param name="perRequest">The per request.</param>
		public override void HandlePerRequest(CompositionBatch batch, PerRequest perRequest)
		{
			batch.AddPart(new ProxyPart(perRequest, CreatePart(perRequest)));
		}

        /// <summary>
        /// Registers a singleton.
        /// </summary>
        /// <param name="batch">The batch.</param>
        /// <param name="singleton">The singleton.</param>
		public override void HandleSingleton(CompositionBatch batch, Singleton singleton)
		{
			batch.AddPart(new ProxyPart(singleton, CreatePart(singleton)));
		}

        /// <summary>
        /// Registers an instance.
        /// </summary>
        /// <param name="batch">The batch.</param>
        /// <param name="instance">The instance.</param>
		public override void HandleInstance(CompositionBatch batch, Instance instance)
		{
			if (!instance.Service.IsInterface)
			{
				base.HandleInstance(batch, instance);
				return;
			}

			var factory = IoC.Get<IProxyFactory>();
		    var impl = factory.CreateProxyWithTarget(
		        instance.Service,
		        instance.Implementation,
		        Enumerable.ToArray(instance.GetType().GetAttributes<IBehavior>(true))
		        );

			if (!instance.HasName())
				batch.AddExportedValue(AttributedModelServices.GetContractName(instance.Service), impl);
			else batch.AddExportedValue(instance.Name, impl);
		}
	}
}