namespace CaliBrism.Modules.Calculator
{
    using System.Windows.Controls;
    using Caliburn.Core;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Composite.Regions;
    using Microsoft.Practices.ServiceLocation;
    using ViewModels;

    public class CalculatorModule : IModule
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly IRegionManager _regionManager;
        private readonly IBinder _binder;
        private readonly IViewStrategy _viewStrategy;

        public CalculatorModule(IServiceLocator serviceLocator,
                                IRegionManager regionManager,
                                IBinder binder,
                                IViewStrategy viewStrategy
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
            var view = _viewStrategy.GetView(model, null, null) as UserControl;

            _binder.Bind(model, view, null);

            _regionManager.RegisterViewWithRegion("MainRegion", () => view);
        }
    }
}