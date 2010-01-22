namespace CaliBrism.Modules.Calculator
{
    using System.Windows.Controls;
    using Caliburn.Core;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Composite.Regions;
    using Microsoft.Practices.ServiceLocation;
    using ViewModels;
    using Caliburn.PresentationFramework.ViewModels;

    public class CalculatorModule : IModule
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly IRegionManager _regionManager;
        private readonly IViewModelBinder _binder;
        private readonly IViewLocator _viewStrategy;

        public CalculatorModule(IServiceLocator serviceLocator,
                                IRegionManager regionManager,
                                IViewModelBinder binder,
                                IViewLocator viewStrategy
            )
        {
            _serviceLocator = serviceLocator;
            _regionManager = regionManager;
            _binder = binder;
            _viewStrategy = viewStrategy;
        }

        public void Initialize()
        {

            var model = _serviceLocator.GetInstance<ICalculatorViewModel>();
            var view = _viewStrategy.Locate(model, null, null);

            _binder.Bind(model, view, null);

            _regionManager.RegisterViewWithRegion("MainRegion", () => view);
        }
    }
}