namespace Caliburn.MEF
{
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using Core;
	using Core.Behaviors;
	using Core.IoC;
	using Microsoft.Practices.ServiceLocation;
	using Enumerable = System.Linq.Enumerable;

	public class ProxiedCompositionBatchStrategy : CompositionBatchStrategy
	{
		public ProxiedCompositionBatchStrategy(bool useSetterInjection)
			: base(useSetterInjection) { }

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
			var factory = ServiceLocator.Current.GetInstance<IProxyFactory>();
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