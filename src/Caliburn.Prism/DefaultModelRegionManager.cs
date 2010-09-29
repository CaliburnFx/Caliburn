namespace Caliburn.Prism
{
    using Microsoft.Practices.Composite.Regions;
    using PresentationFramework.ViewModels;
    using PresentationFramework.Views;

    /// <summary>
    /// The default implemenation of <see cref="IModelRegionManager"/>.
    /// </summary>
    public class DefaultModelRegionManager : IModelRegionManager
    {
        private readonly IRegionManager _regionManager;
        private readonly IViewModelBinder _viewModelBinder;
        private readonly IViewLocator _viewLocator;
        private readonly IViewModelFactory _viewModelFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultModelRegionManager"/> class.
        /// </summary>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="viewModelBinder">The view model binder.</param>
        /// <param name="viewLocator">The view locator.</param>
        /// <param name="viewModelFactory">The view model factory.</param>
        public DefaultModelRegionManager(IRegionManager regionManager, IViewModelBinder viewModelBinder, IViewLocator viewLocator, IViewModelFactory viewModelFactory)
        {
            _regionManager = regionManager;
            _viewModelBinder = viewModelBinder;
            _viewLocator = viewLocator;
            _viewModelFactory = viewModelFactory;
        }

        /// <summary>
        /// Locates a screen for the subject and registers it with with region.
        /// </summary>
        /// <typeparam name="TSubject">The type of the subject.</typeparam>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="subject">The subject.</param>
        /// <returns>The manager.</returns>
        public IModelRegionManager RegisterScreenSubjectWithRegion<TSubject>(string regionName, TSubject subject)
        {
            _regionManager.RegisterViewWithRegion(regionName, () =>{
                var screen = _viewModelFactory.CreateFor(subject);
                var view = _viewLocator.LocateForModel(screen, null, null);
                _viewModelBinder.Bind(screen, view, null);
                return view;
            });

            return this;
        }

        /// <summary>
        /// Registers the model with the region.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="regionName">Name of the region.</param>
        /// <returns>The manager.</returns>
        public IModelRegionManager RegisterModelWithRegion<TViewModel>(string regionName)
        {
            _regionManager.RegisterViewWithRegion(regionName, () =>{
                var viewModel = _viewModelFactory.Create<TViewModel>();
                var view = _viewLocator.LocateForModel(viewModel, null, null);
                _viewModelBinder.Bind(viewModel, view, null);
                return view;
            });

            return this;
        }

        /// <summary>
        /// Registers the model with the region.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="viewModel">The view model.</param>
        /// <returns>The manager.</returns>
        public IModelRegionManager RegisterModelWithRegion(string regionName, object viewModel)
        {
            _regionManager.RegisterViewWithRegion(regionName, () =>{
                var view = _viewLocator.LocateForModel(viewModel, null, null);
                _viewModelBinder.Bind(viewModel, view, null);
                return view;
            });

            return this;
        }
    }
}