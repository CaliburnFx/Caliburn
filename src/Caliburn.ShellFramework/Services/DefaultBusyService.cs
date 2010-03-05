namespace Caliburn.ShellFramework.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Markup;
    using Core;
    using PresentationFramework;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.Views;

    public class DefaultBusyService : IBusyService
    {
        public static string BusyIndicatorName = "busyIndicator";

        private class BusyInfo
        {
            public UIElement BusyIndicator { get; set; }
            public object BusyViewModel { get; set; }
            public int Depth { get; set; }
        }

        private readonly Dictionary<object, BusyInfo> _loaders = new Dictionary<object, BusyInfo>();
        private readonly object _lockObject = new object();
        private readonly object _defaultKey = new object();
        private readonly IWindowManager _windowManager;

        public DefaultBusyService(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        public void MarkAsBusy(object sourceViewModel, object busyViewModel)
        {
            sourceViewModel = sourceViewModel ?? _defaultKey;

            if (_loaders.ContainsKey(sourceViewModel))
            {
                var info = _loaders[sourceViewModel];
                info.BusyViewModel = busyViewModel;
                UpdateLoader(info);
            }
            else
            {
                var busyIndicator = TryFindBusyIndicator(sourceViewModel);

                if (busyIndicator == null)
                {
                    var notifier = busyViewModel as ILifecycleNotifier;
                    if (notifier == null)
                        return;

                    notifier.Initialized += delegate
                    {
                        var info = new BusyInfo { BusyViewModel = busyViewModel };
                        _loaders[sourceViewModel] = info;
                        UpdateLoader(info);
                    };

                    _windowManager.ShowDialog(busyViewModel, null, null);
                }
                else
                {
                    var info = new BusyInfo { BusyIndicator = busyIndicator, BusyViewModel = busyViewModel };
                    _loaders[sourceViewModel] = info;
                    ToggleBusyIndicator(info, true);
                    UpdateLoader(info);
                }
            }
        }

        public void MarkAsNotBusy(object sourceViewModel)
        {
            sourceViewModel = sourceViewModel ?? _defaultKey;

            var info = _loaders[sourceViewModel];

            lock (_lockObject)
            {
                info.Depth--;

                if(info.Depth == 0)
                {
                    _loaders.Remove(sourceViewModel);
                    ToggleBusyIndicator(info, false);
                }
            }
        }

        private void UpdateLoader(BusyInfo info)
        {
            lock(_lockObject)
            {
                info.Depth++;
            }

            if(info.BusyViewModel == null || info.BusyIndicator == null)
                return;

            var indicatorType = info.BusyIndicator.GetType();
            var content = indicatorType.GetProperty("BusyContent");

            if(content == null)
            {
                var contentProperty = indicatorType.GetAttributes<ContentPropertyAttribute>(true)
                    .FirstOrDefault();

                if(contentProperty == null)
                    return;

                content = indicatorType.GetProperty(contentProperty.Name);
            }

            content.SetValue(info.BusyIndicator, info.BusyViewModel, null);
        }

        private void ToggleBusyIndicator(BusyInfo info, bool isBusy)
        {
            if (info.BusyIndicator != null)
            {
                var busyProperty = info.BusyIndicator.GetType().GetProperty("IsBusy");
                if (busyProperty != null)
                    busyProperty.SetValue(info.BusyIndicator, isBusy, null);
                else info.BusyIndicator.Visibility = isBusy ? Visibility.Visible : Visibility.Collapsed;
            }
            else if(!isBusy)
            {
                var close = info.BusyViewModel.GetType().GetMethod("Close", Type.EmptyTypes);
                if (close != null)
                    close.Invoke(info.BusyViewModel, null);
            }
        }

        private UIElement TryFindBusyIndicator(object viewModel)
        {
            DependencyObject view = GetView(viewModel);
            if(view == null)
                return null;

            UIElement busyIndicator = null;

            while (view != null && busyIndicator == null)
            {
                busyIndicator = view.FindName(BusyIndicatorName) as UIElement;
                view = view.GetParent();
            }

            return busyIndicator;
        }

        private UIElement GetView(object viewModel) 
        {
            var viewAware = viewModel as IViewAware;
            if (viewAware == null)
                return null;

            return viewAware.GetView(null) as UIElement;
        }
    }
}