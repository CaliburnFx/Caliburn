namespace Caliburn.ShellFramework.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Markup;
    using Core;
    using Core.Logging;
    using PresentationFramework;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.Screens;
    using PresentationFramework.Views;

    public class DefaultBusyService : IBusyService
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(DefaultBusyService));

        public static string BusyIndicatorName = "busyIndicator";

        private class BusyInfo
        {
            public UIElement BusyIndicator { get; set; }
            public object BusyViewModel { get; set; }
            public int Depth { get; set; }
        }

        private readonly Dictionary<object, BusyInfo> loaders = new Dictionary<object, BusyInfo>();
        private readonly object lockObject = new object();
        private readonly object defaultKey = new object();

        private readonly IWindowManager windowManager;

        public DefaultBusyService(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
        }

        public void MarkAsBusy(object sourceViewModel, object busyViewModel)
        {
            sourceViewModel = sourceViewModel ?? defaultKey;

            if (loaders.ContainsKey(sourceViewModel))
            {
                var info = loaders[sourceViewModel];
                info.BusyViewModel = busyViewModel;
                UpdateLoader(info);
            }
            else
            {
                var busyIndicator = TryFindBusyIndicator(sourceViewModel);

                if (busyIndicator == null)
                {
                    var activator = busyViewModel as IActivate;
                    if (activator == null)
                        return;

                    activator.Activated += (s,e) =>{
                        if (!e.WasInitialized)
                            return;

                        var info = new BusyInfo { BusyViewModel = busyViewModel };
                        loaders[sourceViewModel] = info;
                        UpdateLoader(info);
                    };

                    Log.Warn("No busy indicator with name '" + BusyIndicatorName + "' was found in the UI hierarchy. Using modal.");
                    windowManager.ShowDialog(busyViewModel, null);
                }
                else
                {
                    var info = new BusyInfo { BusyIndicator = busyIndicator, BusyViewModel = busyViewModel };
                    loaders[sourceViewModel] = info;
                    ToggleBusyIndicator(info, true);
                    UpdateLoader(info);
                }
            }
        }

        public void MarkAsNotBusy(object sourceViewModel)
        {
            sourceViewModel = sourceViewModel ?? defaultKey;

            var info = loaders[sourceViewModel];

            lock (lockObject)
            {
                info.Depth--;

                if(info.Depth == 0)
                {
                    loaders.Remove(sourceViewModel);
                    ToggleBusyIndicator(info, false);
                }
            }
        }

        private void UpdateLoader(BusyInfo info)
        {
            lock(lockObject)
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
            if (view == null)
            {
                Log.Warn("Could not find view for {0}.", viewModel);
                return null;
            }

            UIElement busyIndicator = null;
            view = DefaultWindowManager.GetSignificantView(view);

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