namespace Caliburn.MEF
{
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.ComponentModel.Composition.Primitives;
	using Core.IoC;

	public class CompositionBatchStrategy
	{
		bool _useSetterInjection;
		public CompositionBatchStrategy(bool useSetterInjection) {
			_useSetterInjection = useSetterInjection;
		}

		public virtual void HandleSingleton(CompositionBatch batch, Singleton singleton)
		{
			batch.AddPart(new ComponentPart(singleton, _useSetterInjection));
		}

		public virtual void HandleSingleton(CompositionBatch batch, PerRequest perRequest)
		{
			batch.AddPart(new ComponentPart(perRequest, _useSetterInjection));
		}

		public virtual void HandleInstance(CompositionBatch batch, Instance instance)
		{
			if (!instance.HasName())
				batch.AddExportedValue(AttributedModelServices.GetContractName(instance.Service), instance.Implementation);
			else batch.AddExportedValue(instance.Name, instance.Implementation);
		}

		protected virtual ComposablePart CreatePart(ComponentRegistrationBase registration)
		{
			return new ComponentPart(registration, _useSetterInjection);
		}
	}
}