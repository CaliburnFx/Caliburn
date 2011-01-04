namespace Caliburn.MEF
{
	using System.ComponentModel.Composition.Hosting;
	using System.ComponentModel.Composition.Primitives;
	using Core.InversionOfControl;

    /// <summary>
    /// Used to help in batching component registrations to the container.
    /// </summary>
	public class CompositionBatchStrategy
	{
        /// <summary>
        /// Registers a singleton.
        /// </summary>
        /// <param name="batch">The batch.</param>
        /// <param name="singleton">The singleton.</param>
		public virtual void HandleSingleton(CompositionBatch batch, Singleton singleton)
		{
			batch.AddPart(new ComponentPart(singleton));
		}

        /// <summary>
        /// Registers a per request.
        /// </summary>
        /// <param name="batch">The batch.</param>
        /// <param name="perRequest">The per request.</param>
        public virtual void HandlePerRequest(CompositionBatch batch, PerRequest perRequest)
		{
			batch.AddPart(new ComponentPart(perRequest));
		}

        /// <summary>
        /// Registers an instance.
        /// </summary>
        /// <param name="batch">The batch.</param>
        /// <param name="instance">The instance.</param>
		public virtual void HandleInstance(CompositionBatch batch, Instance instance)
		{
			batch.AddPart(new ComponentPart(instance));
		}

        /// <summary>
        /// Creates the part.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
		protected virtual ComposablePart CreatePart(ComponentRegistrationBase registration)
		{
			return new ComponentPart(registration);
		}
	}
}