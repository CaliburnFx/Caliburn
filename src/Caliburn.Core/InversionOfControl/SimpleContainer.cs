namespace Caliburn.Core.InversionOfControl
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Behaviors;
	using Core;

	/// <summary>
	/// A simple dependency injection container.
	/// </summary>
	public class SimpleContainer : ContainerBase
	{
		readonly Dictionary<string, Func<object>> typeToHandler
			= new Dictionary<string, Func<object>>();

		bool inspectingForBehaviors;
		readonly bool performPropertyInjection;

		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleContainer"/> class.
		/// </summary>
		public SimpleContainer() : this(false) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleContainer"/> class.
		/// </summary>
		/// <param name="performPropertyInjection">Specify if null settable properties of resolved instance should be injected with dependencies</param>
		public SimpleContainer(bool performPropertyInjection)
		{
			AddHandler(typeof(IServiceLocator), () => this);
			AddHandler(typeof(SimpleContainer), () => this);
			AddHandler(typeof(IContainer), () => this);
			AddHandler(typeof(IRegistry), () => this);
			AddHandler(typeof(IBuilder), () => this);

			AddRegistrationHandler<Singleton>(HandleSingleton);
			AddRegistrationHandler<PerRequest>(HandlePerRequest);
			AddRegistrationHandler<Instance>(HandleInstance);

			this.performPropertyInjection = performPropertyInjection;
		}


		private void HandleSingleton(Singleton singleton)
		{
			if (singleton.HasName())
				RegisterSingleton(singleton.Name, singleton.Implementation);
			else RegisterSingleton(singleton.Service, singleton.Implementation);
		}

		private void HandlePerRequest(PerRequest perRequest)
		{
			if (perRequest.HasName())
				Register(perRequest.Name, perRequest.Implementation);
			else Register(perRequest.Service, perRequest.Implementation);
		}

		private void HandleInstance(Instance instance)
		{
			if (instance.HasName())
				AddHandler(instance.Name, () => instance.Implementation);
			else AddHandler(instance.Service, () => instance.Implementation);
		}

		/// <summary>
		/// Registers the specified service.
		/// </summary>
		/// <typeparam name="TService">The type of the service.</typeparam>
		/// <typeparam name="TImplementation">The type of the implementation.</typeparam>
		public void Register<TService, TImplementation>()
			where TImplementation : TService
		{
			Register(typeof(TService), typeof(TImplementation));
		}

		/// <summary>
		/// Registers the specified service.
		/// </summary>
		/// <param name="service">The service type.</param>
		/// <param name="implementation">The implementation.</param>
		public void Register(Type service, Type implementation)
		{
			if (service.IsGenericType && implementation.IsGenericType)
				AddHandler(service, () => implementation);
			else AddHandler(service, () => CreateInstance(implementation));
		}

		/// <summary>
		/// Registers the specified service.
		/// </summary>
		/// <param name="key">The service key.</param>
		/// <param name="implementation">The implementation.</param>
		public void Register(string key, Type implementation)
		{
			AddHandler(key, () => CreateInstance(implementation));
		}

		/// <summary>
		/// Registers the specified service.
		/// </summary>
		/// <param name="service">The service type.</param>
		/// <param name="implementation">The implementation.</param>
		/// <param name="key">The service key.</param>
		public void Register(Type service, Type implementation, string key)
		{
			if (service.IsGenericType && implementation.IsGenericType)
				AddHandler(key, () => implementation);
			else AddHandler(key, () => CreateInstance(implementation));
		}

		/// <summary>
		/// Registers the specified service.
		/// </summary>
		/// <param name="key">The service key.</param>
		/// <typeparam name="TImplementation">The type of the implementation.</typeparam>
		public void Register<TImplementation>(string key)
		{
			Register(key, typeof(TImplementation));
		}

		/// <summary>
		/// Registers the service as a singleton.
		/// </summary>
		/// <typeparam name="TService">The type of the service.</typeparam>
		/// <typeparam name="TImplementation">The type of the implementation.</typeparam>
		public void RegisterSingleton<TService, TImplementation>()
			where TImplementation : TService
		{
			RegisterSingleton(typeof(TService), typeof(TImplementation));
		}

		/// <summary>
		/// Registers the service as a singleton.
		/// </summary>
		/// <param name="service">The service type.</param>
		/// <param name="implementation">The implementation.</param>
		public void RegisterSingleton(Type service, Type implementation)
		{
			RegisterSingleton(service.FullName, implementation);
		}

		/// <summary>
		/// Registers the service as a singleton.
		/// </summary>
		/// <param name="key">The service key.</param>
		/// <param name="implementation">The implementation.</param>
		public void RegisterSingleton(string key, Type implementation)
		{
			object singleton = null;

		    AddHandler(
		        key,
		        () => singleton ?? (singleton = CreateInstance(implementation))
		        );
		}

		/// <summary>
		/// Registers the service as a singleton.
		/// </summary>
		/// <param name="key">The service key.</param>
		/// <typeparam name="TImplementation">The type of the implementation.</typeparam>
		public void RegisterSingleton<TImplementation>(string key)
		{
			RegisterSingleton(key, typeof(TImplementation));
		}

		/// <summary>
		/// Determines whether the specified service is already registered.
		/// </summary>
		/// <param name="fullName">The service type full name.</param>
		/// <returns>
		/// 	<c>true</c> if the specified type is registered; otherwise, <c>false</c>.
		/// </returns>
		public bool IsRegistered(string fullName)
		{
			return typeToHandler.ContainsKey(fullName);
		}

		/// <summary>
		/// Determines whether the specified service is already registered.
		/// </summary>
		/// <param name="type">The service type.</param>
		/// <returns>
		/// 	<c>true</c> if the specified type is registered; otherwise, <c>false</c>.
		/// </returns>
		public bool IsRegistered(Type type)
		{
			return typeToHandler.ContainsKey(type.FullName);
		}

		/// <summary>
		/// Adds a handler for the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="handler">The handler.</param>
		public void AddHandler(Type type, Func<object> handler)
		{
			AddHandler(type.FullName, handler);
		}

		/// <summary>
		/// Adds a handler for the specified type.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="handler">The handler.</param>
		public void AddHandler(string key, Func<object> handler)
		{
			if (IsRegistered(key))
			{
				var exception = new ArgumentException(key + " was already registered in the container.");
				Log.Error(exception);
				throw exception;
			}

			typeToHandler.Add(key, handler);
		}

		/// <summary>
		/// Gets the handler for the specified type.
		/// </summary>
		/// <param name="type">The type to locate the handler for.</param>
		/// <returns>The type handler.</returns>
		public Func<object> GetHandler(Type type)
		{
			if (type.IsGenericType && !IsRegistered(type))
			{
				var genericType = type.GetGenericTypeDefinition();

				if (!IsRegistered(genericType))
				{
					var exception = new ArgumentException(type + " is not a registered component.");
					Log.Error(exception);
					throw exception;
				}

				return () => CreateInstance(InternalGetHandler(genericType)() as Type, type.GetGenericArguments());
			}

			return InternalGetHandler(type);
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
			RegisterSingleton<IProxyFactory, T>();
			inspectingForBehaviors = true;
			return this;
		}

		private Func<Object> InternalGetHandler(Type type)
		{
			Func<object> handler;

			if (!typeToHandler.TryGetValue(type.FullName, out handler))
			{
				if (!type.IsAbstract)
				{
					AddHandler(type, () => CreateInstance(type));
					return GetHandler(type);
				}

				var exception = new ArgumentException(type + " is not a registered component.");
				Log.Error(exception);
				throw exception;
			}

			return handler;
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
		    return key == null
		        ? GetHandler(serviceType)()
		        : (serviceType == null)
		            ? GetInstance(key)
		            : GetInstance(key, serviceType.GetGenericArguments());
		}

		/// <summary>
		/// When implemented by inheriting classes, this method will do the actual work of
		/// resolving all the requested service instances.
		/// </summary>
		/// <param name="serviceType">Type of service requested.</param>
		/// <returns>Sequence of service instance objects.</returns>
        public override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            Func<object> handler;

            return typeToHandler.TryGetValue(serviceType.FullName, out handler)
                       ? new[] { handler() }
                       : new object[] { };
        }

        /// <summary>
        /// Injects dependencies into the object.
        /// </summary>
        /// <param name="instance">The instance to build up.</param>
        public override void BuildUp(object instance)
        {
            var injectables = from property in instance.GetType().GetProperties()
                              where property.CanRead && property.CanWrite && property.PropertyType.IsInterface
                              select property;

            injectables.Apply(x =>{
                var injection = GetAllInstances(x.PropertyType);
                if(injection.Any())
                    x.SetValue(instance, injection.First(), null);
            });
        }

		/// <summary>
		/// Resolves the service by key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="typeArguments">The type arguments for to inject into an open generic type.</param>
		/// <returns>
		/// A configured instance of the resolved service.
		/// </returns>
		public object GetInstance(string key, params Type[] typeArguments)
		{
			Func<object> handler;

			if (!typeToHandler.TryGetValue(key, out handler))
			{
				var exception = new ArgumentException(key + " is not a registered component key.");
				Log.Error(exception);
				throw exception;
			}

			var instance = handler();

			// TODO: That was hacky, must have some clever way to do this.
			var type = instance as Type;
			return type == null ? instance : CreateInstance(type, typeArguments);
		}

		private object CreateInstance(Type type, params Type[] typeArguments)
		{
			// TODO: That was hacky, must have some clever way to do this.
			if (typeArguments != null && typeArguments.Length > 0)
				type = type.MakeGenericType(typeArguments);

			var args = DetermineConstructorArgs(type);

			object instance = null;
			if (inspectingForBehaviors && type.ShouldCreateProxy())
			{
				var factory = this.GetInstance<IProxyFactory>();

				instance = factory.CreateProxy(
					type,
					type.GetAttributes<IBehavior>(true),
					args
					);
			}
			else
			{
				instance = args.Length > 0 ? Activator.CreateInstance(type, args) : Activator.CreateInstance(type);
			}

			if (performPropertyInjection)
				PerformPropertiesInjection(instance);
			return instance;
		}

		private bool TryGetDefaultInstance(Type serviceType, out object instance)
		{
			try
			{
				instance = this.GetInstance(serviceType);
				return instance != null;
			}
			catch
			{
				instance = null;
				return false;
			}
		}

		private void PerformPropertiesInjection(object instance)
		{
			var instanceType = instance.GetType();
			instanceType.GetProperties()
				.Where(p => p.CanWrite)
				.Where(p => p.PropertyType.IsClass || p.PropertyType.IsInterface)
				.Where(p => !p.GetIndexParameters().Any())
				.Apply(p =>
				{
					try
					{
						var val = p.GetValue(instance, new object[] { });
						if (val == null)
						{
							if (TryGetDefaultInstance(p.PropertyType, out val))
							{
								p.SetValue(instance, val, new object[] { });
							}
						}
					}
					catch (Exception ex)
					{
						throw new CaliburnException(
							string.Format("Error injecting the dependency {0} into property {1} of type {2}",
								p.PropertyType.FullName,
								p.Name,
								instanceType.FullName
							), ex);
					}
				});
		}
	}
}