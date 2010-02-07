namespace Caliburn.ShellFramework.Services
{
    using System.Collections.Generic;
    using System.Windows;
    using Core.Metadata;
    using PresentationFramework;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.Metadata;
    using PresentationFramework.ViewModels;

    public class DefaultLoader : ILoader
    {
        private readonly Dictionary<object, LoadScreenViewModel> _loaders = 
            new Dictionary<object, LoadScreenViewModel>();

        private readonly IViewModelBinder _viewModelBinder;
        private readonly IWindowManager _windowManager;
        private readonly object _key = new object();

        public DefaultLoader(IViewModelBinder viewModelBinder, IWindowManager windowManager)
        {
            _windowManager = windowManager;
            _viewModelBinder = viewModelBinder;
        }

        public void StartLoading(object viewModel, string message)
        {
            viewModel = viewModel ?? _key;

            if (_loaders.ContainsKey(viewModel))
                _loaders[viewModel].StartLoading(message);
            else
            {
                var loader = new LoadScreenViewModel();
                loader.StartLoading(message);
                _loaders[viewModel] = loader;

                var view = TryFindViewInTree(viewModel);

                if (view == null)
                    _windowManager.ShowDialog(loader, null, null);
                else
                {
                    _viewModelBinder.Bind(loader, view, null);
                    loader.SetView(view, null, false);
                    ((UIElement)view).Visibility = Visibility.Visible;
                }
            }
        }

        public void StopLoading(object viewModel)
        {
            viewModel = viewModel ?? _key;

            var loader = _loaders[viewModel];
            loader.StopLoading();

            if(loader.LoadDepth == 0)
            {
                _loaders.Remove(viewModel);

                var view = loader.GetView<object>(null);
                var method = view.GetType().GetMethod("Close");

                if (method != null)
                    method.Invoke(view, null);
                else ((UIElement)view).Visibility = Visibility.Collapsed;
            }
        }

        private DependencyObject TryFindViewInTree(object model)
        {
            var metadataContainer = model as IMetadataContainer;
            if (metadataContainer == null) return null;

            var view = metadataContainer.GetView<DependencyObject>(null);
            if (view == null) return null;

            DependencyObject loaderView = null;

            while(view != null && loaderView == null)
            {
                loaderView = view.FindName("loaderView");
                view = view.GetParent();
            }

            return loaderView;
        }
    }
}