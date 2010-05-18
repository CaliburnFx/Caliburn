namespace Caliburn.MEF
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.Linq;
	using Core;
	using Core.Behaviors;
	using Core.IoC;
	using Microsoft.Practices.ServiceLocation;

	/// <summary>
	/// An adapter allowing a <see cref="CompositionContainer"/> to plug into Caliburn via <see cref="IServiceLocator"/> and <see cref="IRegistry"/>.
	/// </summary>
	public class MEFAdapter : ContainerBase
	{
		private CompositionContainer _container;
		private CompositionBatch _batch;
		private CompositionBatchStrategy _batchStrategy = new CompositionBatchStrategy();

		/// <summary>
		/// Initializes a new instance of the <see cref="MEFAdapter"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		public MEFAdapter(CompositionContainer container)
		{
			_container = container;

		    IoCExtensions.SelectEligibleConstructorImplementation = type =>{
		        return (from c in type.GetConstructors()
		                where c.GetAttributes<ImportingConstructorAttribute>(false)
		                    .FirstOrDefault() != null
		                select c).FirstOrDefault() ??
		                    (from c in type.GetConstructors()
		                     orderby c.GetParameters().Length descending
		                     select c).FirstOrDefault();
		    };

			var batch = new CompositionBatch();

			batch.AddExportedValue<IServiceLocator>(this);
			batch.AddExportedValue<IRegistry>(this);
			batch.AddExportedValue<IContainer>(this);
			batch.AddExportedValue<CompositionContainer>(_container);

			_container.Compose(batch);

			AddRegistrationHandler<Singleton>(HandleSingleton);
			AddRegistrationHandler<PerRequest>(HandleSingleton);
			AddRegistrationHandler<Instance>(HandleInstance);
		}

		/// <summary>
		/// Gets the container.
		/// </summary>
		/// <value>The container.</value>
		public CompositionContainer Container
		{
			get { return _container; }
		}

		/// <summary>
		/// Gets or sets the batch strategy used when registering components. Defaults to <see cref="CompositionBatchStrategy"/>.
		/// </summary>
		/// <remarks>A call to <see cref="WithProxyFactory{T}"/> sets this to <see cref="ProxiedCompositionBatchStrategy"/></remarks>
		/// <value>The batch strategy.</value>
		public CompositionBatchStrategy BatchStrategy
		{
			get { return _batchStrategy; }
			set { _batchStrategy = value; }
		}

		/// <summary>
		/// When implemented by inheriting classes, this method will do the actual work of resolving
		/// the requested service instance.
		/// </summary>
		/// <param name="serviceType">Type of instance requested.</param>
		/// <param name="key">Name of registered service you want. May be null.</param>
		/// <returns>The requested service instance.</returns>
		protected override object DoGetInstance(Type serviceType, string key)
		{
			string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
			var exports = _container.GetExportedValues<object>(contract);

            if(exports.Count() > 0)
				return exports.First();

			throw new ActivationException(string.Format("Could not locate any instances of contract {0}.", contract));
		}

		/// <summary>
		/// When implemented by inheriting classes, this method will do the actual work of
		/// resolving all the requested service instances.
		/// </summary>
		/// <param name="serviceType">Type of service requested.</param>
		/// <returns>Sequence of service instance objects.</returns>
		protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
		{
			return _container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
		}

		/// <summary>
		/// Configures the container using the provided component registrations.
		/// </summary>
		/// <param name="registrations">The component registrations.</param>
		public override void Register(IEnumerable<IComponentRegistration> registrations)
		{
			_batch = new CompositionBatch();

			base.Register(registrations);

			_container.Compose(_batch);
			_batch = null;
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
			BatchStrategy.HandleSingleton(_batch, singleton);
		}

		private void HandleSingleton(PerRequest perRequest)
		{
			BatchStrategy.HandleSingleton(_batch, perRequest);
		}

		private void HandleInstance(Instance instance)
		{
			BatchStrategy.HandleInstance(_batch, instance);
		}
	}
}