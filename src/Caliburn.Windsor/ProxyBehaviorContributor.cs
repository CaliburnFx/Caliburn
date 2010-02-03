namespace Caliburn.Windsor
{
    using Castle.Core;
    using Castle.MicroKernel;
    using Castle.MicroKernel.ModelBuilder;
    using Core.Behaviors;

    /// <summary>
    /// An <see cref="IContributeComponentModelConstruction"/> that adds proxy capabilities.
    /// </summary>
    public class ProxyBehaviorContributor : IContributeComponentModelConstruction
    {
        /// <summary>
        /// Usually the implementation will look in the configuration property
        /// of the model or the service interface, or the implementation looking for
        /// something.
        /// </summary>
        /// <param name="kernel">The kernel instance</param>
        /// <param name="model">The component model</param>
        public void ProcessModel(IKernel kernel, ComponentModel model)
        {
            if (model.Implementation.ShouldCreateProxy())
                model.CustomComponentActivator = typeof(ProxyActivator);
        }
    }
}