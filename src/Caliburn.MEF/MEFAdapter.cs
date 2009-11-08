namespace Caliburn.MEF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using Core;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// An adapter allowing a <see cref="CompositionContainer"/> to plug into Caliburn via <see cref="IServiceLocator"/> and <see cref="IConfigurator"/>.
    /// </summary>
    public class MEFAdapter : ServiceLocatorImplBase, IContainer
    {
        private readonly CompositionContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="MEFAdapter"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public MEFAdapter(CompositionContainer container)
        {
            _container = container;

            var batch = new CompositionBatch();

            batch.AddExportedValue(AttributedModelServices.GetContractName(typeof(IServiceLocator)), this);
            batch.AddExportedValue(AttributedModelServices.GetContractName(typeof(IConfigurator)), this);
            batch.AddExportedValue(AttributedModelServices.GetContractName(typeof(IContainer)), this);
            batch.AddExportedValue(AttributedModelServices.GetContractName(typeof(CompositionContainer)), _container);

            _container.Compose(batch);
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
        /// Configures the container with the provided components.
        /// </summary>
        /// <param name="components">The components.</param>
        public void ConfigureWith(IEnumerable<ComponentInfo> components)
        {
            var batch = new CompositionBatch();

            foreach(var componentInfo in components)
            {
                batch.AddPart(new ComponentPart(componentInfo));
            }

            _container.Compose(batch);
        }
    }
}