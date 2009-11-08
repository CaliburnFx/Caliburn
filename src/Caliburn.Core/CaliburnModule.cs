namespace Caliburn.Core
{
    using System.Collections.Generic;
    using Microsoft.Practices.ServiceLocation;
    using System;

    /// <summary>
    /// A base class used by Caliburn modules to tie into the configuration process.
    /// </summary>
    public abstract class CaliburnModule : IConfigurationHook
    {
        private readonly IConfigurationHook _hook;

        /// <summary>
        /// Initializes a new instance of the <see cref="CaliburnModule"/> class.
        /// </summary>
        /// <param name="hook">The hook.</param>
        protected CaliburnModule(IConfigurationHook hook)
        {
            _hook = hook;
            _hook.Core.Add(this);
        }

		/// <summary>
		/// Expose the core configuration to inheritors
		/// </summary>
		/// <value>The core configuration.</value>
		protected CoreConfiguration Core
		{
			get { return _hook.Core; }
		}

        /// <summary>
        /// Gets the core configuration.
        /// </summary>
        /// <value>The core configuration.</value>
        CoreConfiguration IConfigurationHook.Core
        {
            get { return _hook.Core; }
        }

        /// <summary>
        /// Gets the service locator.
        /// </summary>
        /// <value>The service locator.</value>
        protected IServiceLocator ServiceLocator
        {
            get { return _hook.Core.ServiceLocator; }
        }

        /// <summary>
        /// Starts the application if it has not already been started.
        /// </summary>
        public void Start()
        {
            _hook.Core.Start();
        }

        /// <summary>
        /// Gets the component information for this module.
        /// </summary>
        protected internal abstract IEnumerable<ComponentInfo> GetComponents();

        /// <summary>
        /// Initializes this module.
        /// </summary>
        protected internal abstract void Initialize();

        /// <summary>
        /// Creates a <see cref="ComponentInfo"/> with a Singleton lifetime.
        /// </summary>
        protected ComponentInfo Singleton(Type service, Type implementation)
        {
            return new ComponentInfo
            {
                Service = service, 
                Implementation = implementation, 
                Lifetime = ComponentLifetime.Singleton
            };
        }

        /// <summary>
        /// Creates a <see cref="ComponentInfo"/> with a PerRequest lifetime.
        /// </summary>
        protected ComponentInfo PerRequest(Type service, Type implementation)
        {
            return new ComponentInfo
            {
                Service = service,
                Implementation = implementation,
                Lifetime = ComponentLifetime.PerRequest
            };
        }
    }
}