namespace Caliburn.Windsor
{
    using Castle.MicroKernel;
    using Castle.MicroKernel.Facilities;

    /// <summary>
    /// An implementation of <see cref="IFacility"/> that adds proxy capabilities.
    /// </summary>
    public class ProxyBehaviorFacility : AbstractFacility
    {
        /// <summary>
        /// The custom initialization for the Facility.
        /// </summary>
        /// <remarks>It must be overriden.</remarks>
        protected override void Init()
        {
            Kernel.ComponentModelBuilder.AddContributor(new ProxyBehaviorContributor());
        }
    }
}