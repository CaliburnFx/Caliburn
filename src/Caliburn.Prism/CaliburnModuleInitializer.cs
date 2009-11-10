namespace Caliburn.Prism
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Core.IoC;
    using Microsoft.Practices.Composite.Logging;
    using Microsoft.Practices.Composite.Modularity;
    using Core;

    /// <summary>
    /// Custom implementation of the ModuleInitializer used to register the module with the container
    /// and load the modules assembly into Caliburn
    /// </summary>
    public class CaliburnModuleInitializer : ModuleInitializer
    {
        private readonly IAssemblySource _assemblySource;
        private readonly IContainer _caliburnContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CaliburnModuleInitializer"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="loggerFacade">The logger facade.</param>
        /// <param name="assemblySource">The assembly source.</param>
        public CaliburnModuleInitializer(IContainer serviceLocator, ILoggerFacade loggerFacade, IAssemblySource assemblySource) 
            : base(serviceLocator, loggerFacade)
        {
            _assemblySource = assemblySource;
            _caliburnContainer = serviceLocator;
        }

        /// <summary>
        /// Override the default implemtation in order to register the module with the container and load it's assembly
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        protected override IModule CreateModule(string typeName)
        {
            if (_caliburnContainer != null)
            {
                var type = Type.GetType(typeName);

                _caliburnContainer.Register(new[]
                {
                    new PerRequest
                    {
                        Service = type,
                        Implementation = type
                    },
                });

                AddAssemblyIfMissing(type.Assembly);
            }

            return base.CreateModule(typeName);
        }

        private void AddAssemblyIfMissing(Assembly assembly)
        {
            if(!_assemblySource.Contains(assembly))
                _assemblySource.Add(assembly);
        }
    }
}