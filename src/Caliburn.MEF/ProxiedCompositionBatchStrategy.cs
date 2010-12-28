namespace Caliburn.MEF
{
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using Core;
	using Core.Behaviors;
	using Core.InversionOfControl;
	using Enumerable = System.Linq.Enumerable;

	public class ProxiedCompositionBatchStrategy : CompositionBatchStrategy
	{
		public ProxiedCompositionBatchStrategy()
			: base() { }

		public override void HandleSingleton(CompositionBatch batch, PerRequest perRequest)
		{
			batch.AddPart(new ProxyPart(perRequest, CreatePart(perRequest)));
		}

		public override void HandleSingleton(CompositionBatch batch, Singleton singleton)
		{
			batch.AddPart(new ProxyPart(singleton, CreatePart(singleton)));
		}

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