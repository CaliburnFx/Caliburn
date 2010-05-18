namespace Caliburn.MEF
{
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.ComponentModel.Composition.Primitives;
	using Core.IoC;

	public class CompositionBatchStrategy
	{
		public virtual void HandleSingleton(CompositionBatch batch, Singleton singleton)
		{
			batch.AddPart(new ComponentPart(singleton));
		}

		public virtual void HandleSingleton(CompositionBatch batch, PerRequest perRequest)
		{
			batch.AddPart(new ComponentPart(perRequest));
		}

		public virtual void HandleInstance(CompositionBatch batch, Instance instance)
		{
			if (!instance.HasName())
				batch.AddExportedValue(AttributedModelServices.GetContractName(instance.Service), instance.Implementation);
			else batch.AddExportedValue(instance.Name, instance.Implementation);
		}

		protected virtual ComposablePart CreatePart(ComponentRegistrationBase registration)
		{
			return new ComponentPart(registration);
		}
	}
}