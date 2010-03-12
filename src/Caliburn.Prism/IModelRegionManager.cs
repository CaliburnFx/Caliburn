namespace Caliburn.Prism
{
    using Microsoft.Practices.Composite.Regions;

    /// <summary>
    /// Enables working with Prism's <see cref="IRegionManager"/> using a model or screen subject first approach.
    /// </summary>
    public interface IModelRegionManager
    {
        /// <summary>
        /// Locates a screen for the subject and registers it with with region.
        /// </summary>
        /// <typeparam name="TSubject">The type of the subject.</typeparam>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="subject">The subject.</param>
        /// <returns>The manager.</returns>
        IModelRegionManager RegisterScreenSubjectWithRegion<TSubject>(string regionName, TSubject subject);

        /// <summary>
        /// Registers the model with the region.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="regionName">Name of the region.</param>
        /// <returns>The manager.</returns>
        IModelRegionManager RegisterModelWithRegion<TViewModel>(string regionName);

        /// <summary>
        /// Registers the model with the region.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns>The manager.</returns>
        IModelRegionManager RegisterModelWithRegion(string regionName, object viewModel);
    }
}