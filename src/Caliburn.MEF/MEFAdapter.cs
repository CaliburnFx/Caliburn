namespace Caliburn.MEF
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.Linq;
	using Core;
	using Core.Behaviors;
	using Core.Configuration;
	using Core.InversionOfControl;

	/// <summary>
	/// An adapter allowing a <see cref="CompositionContainer"/> to plug into Caliburn via <see cref="IServiceLocator"/> and <see cref="IRegistry"/>.
	/// </summary>
	public class MEFAdapter : ContainerBase
	{
		readonly CompositionContainer container;
		CompositionBatch batch;
		CompositionBatchStrategy batchStrategy;

		/// <summary>
		/// Initializes a new instance of the <see cref="MEFAdapter"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		public MEFAdapter(CompositionContainer container) 
		{
			this.container = container;
			batchStrategy = new CompositionBatchStrategy();

		    CaliburnModule<CoreConfiguration>.Instance.SelectConstructorsWith(
		        type =>{
		            return (from c in type.GetConstructors()
		                    where c.GetAttributes<ImportingConstructorAttribute>(false)
		                        .FirstOrDefault() != null
		                    select c).FirstOrDefault() ??
		                        (from c in type.GetConstructors()
		                         orderby c.GetParameters().Length descending
		                         select c).FirstOrDefault();
		        });

			var batch = new CompositionBatch();

			batch.AddExportedValue<IServiceLocator>(this);
			batch.AddExportedValue<IRegistry>(this);
			batch.AddExportedValue<IContainer>(this);
			batch.AddExportedValue<CompositionContainer>(this.container);

			this.container.Compose(batch);

			AddRegistrationHandler<Singleton>(HandleSingleton);
			AddRegistrationHandler<PerRequest>(HandlePerRequest);
			AddRegistrationHandler<Instance>(HandleInstance);
		}

		/// <summary>
		/// Gets the container.
		/// </summary>
		/// <value>The container.</value>
		public CompositionContainer Container
		{
			get { return container; }
		}

		/// <summary>
		/// Gets or sets the batch strategy used when registering components. Defaults to <see cref="CompositionBatchStrategy"/>.
		/// </summary>
		/// <remarks>A call to <see cref="WithProxyFactory{T}"/> sets this to <see cref="ProxiedCompositionBatchStrategy"/></remarks>
		/// <value>The batch strategy.</value>
		public CompositionBatchStrategy BatchStrategy
		{
			get { return batchStrategy; }
			set { batchStrategy = value; }
		}

		/// <summary>
		/// When implemented by inheriting classes, this method will do the actual work of resolving
		/// the requested service instance.
		/// </summary>
		/// <param name="serviceType">Type of instance requested.</param>
		/// <param name="key">Name of registered service you want. May be null.</param>
		/// <returns>The requested service instance.</returns>
		public override object GetInstance(Type serviceType, string key)
		{
			string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
			var exports = container.GetExportedValues<object>(contract);
			
            if(exports.Any())
				return exports.First();

			return null;
		}

		/// <summary>
		/// When implemented by inheriting classes, this method will do the actual work of
		/// resolving all the requested service instances.
		/// </summary>
		/// <param name="serviceType">Type of service requested.</param>
		/// <returns>Sequence of service instance objects.</returns>
		public override IEnumerable<object> GetAllInstances(Type serviceType)
		{
			return container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
		}

        /// <summary>
        /// Injects dependencies into the object.
        /// </summary>
        /// <param name="instance">The instance to build up.</param>
	    public override void BuildUp(object instance)
        {
            container.SatisfyImportsOnce(instance);
        }

	    /// <summary>
		/// Configures the container using the provided component registrations.
		/// </summary>
		/// <param name="registrations">The component registrations.</param>
		public override void Register(IEnumerable<IComponentRegistration> registrations)
		{
			batch = new CompositionBatch();

			base.Register(registrations);

			container.Compose(batch);
			batch = null;
		}

		/// <summary>
		/// Installs a proxy factory.
		/// </summary>
		/// <typeparam name="T">The type of the proxy factory.</typeparam>
		/// <returns>
		/// A container with an installed proxy factory.
		/// </returns>
		public override IContainer WithProxyFactory<T>()
		{
		    Register(new IComponentRegistration[]
		    {
		        new Singleton
		        {
		            Service = typeof(IProxyFactory), 
                    Implementation = typeof(T)
		        }
		    });

			BatchStrategy = new ProxiedCompositionBatchStrategy();

			return this;
		}

		private void HandleSingleton(Singleton singleton)
		{
			BatchStrategy.HandleSingleton(batch, singleton);
		}

        private void HandlePerRequest(PerRequest perRequest)
		{
            BatchStrategy.HandlePerRequest(batch, perRequest);
		}

		private void HandleInstance(Instance instance)
		{
			BatchStrategy.HandleInstance(batch, instance);
		}
	}
}