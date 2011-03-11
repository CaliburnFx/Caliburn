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

    /// <summary>
    /// The default implementation of <see cref="IBusyService"/>.
    /// </summary>
    public class DefaultBusyService : IBusyService
    {
        /// <summary>
        /// The log.
        /// </summary>
        protected static readonly ILog Log = LogManager.GetLog(typeof(DefaultBusyService));

        /// <summary>
        /// The Indicators lock.
        /// </summary>
        protected readonly object IndicatorLock = new object();

        /// <summary>
        /// The currently active busy indicator, keyed by ViewModel.
        /// </summary>
        protected readonly Dictionary<object, BusyInfo> Indicators = new Dictionary<object, BusyInfo>();

        /// <summary>
        /// The window manager.
        /// </summary>
        protected readonly IWindowManager WindowManager;

        readonly object defaultKey = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBusyService"/> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        public DefaultBusyService(IWindowManager windowManager)
        {
            WindowManager = windowManager;
        }

        /// <summary>
        /// Marks a ViewModel as busy.
        /// </summary>
        /// <param name="sourceViewModel">The ViewModel to mark as busy.</param>
        /// <param name="busyViewModel">The busy content ViewModel.</param>
        public void MarkAsBusy(object sourceViewModel, object busyViewModel)
        {
            sourceViewModel = sourceViewModel ?? defaultKey;

            if(Indicators.ContainsKey(sourceViewModel))
            {
                var info = Indicators[sourceViewModel];

                info.BusyViewModel = busyViewModel;
                var indicator = TryFindBusyIndicator(sourceViewModel);

                if(info.BusyIndicator != indicator) {
                    info.BusyIndicator = indicator;
                    ToggleBusyIndicator(info, true);
                }

                UpdateIndicator(info);
            }
            else
            {
                var busyIndicator = TryFindBusyIndicator(sourceViewModel);

                if(busyIndicator == null)
                    NoBusyIndicatorFound(sourceViewModel, busyViewModel);
                else
                    BusyIndicatorFound(sourceViewModel, busyViewModel, busyIndicator);
            }
        }

        /// <summary>
        /// Marks a ViewModel as not busy.
        /// </summary>
        /// <param name="sourceViewModel">The ViewModel to mark as not busy.</param>
        public void MarkAsNotBusy(object sourceViewModel)
        {
            sourceViewModel = sourceViewModel ?? defaultKey;
            BusyInfo info;

            if(!Indicators.TryGetValue(sourceViewModel, out info))
                return;

            lock(IndicatorLock)
            {
                info.Depth--;

                if(info.Depth == 0)
                {
                    Indicators.Remove(sourceViewModel);
                    ToggleBusyIndicator(info, false);
                }
            }
        }

        /// <summary>
        /// Called when the busy indicator is found.
        /// </summary>
        /// <param name="sourceViewModel">The source view model.</param>
        /// <param name="busyViewModel">The busy view model.</param>
        /// <param name="busyIndicator">The busy indicator.</param>
        protected virtual void BusyIndicatorFound(object sourceViewModel, object busyViewModel, UIElement busyIndicator)
        {
            var info = new BusyInfo {
                BusyIndicator = busyIndicator,
                BusyViewModel = busyViewModel
            };

            Indicators[sourceViewModel] = info;

            ToggleBusyIndicator(info, true);
            UpdateIndicator(info);
        }

        /// <summary>
        /// Called when no busy indicator can be found.
        /// </summary>
        /// <param name="sourceViewModel">The source view model.</param>
        /// <param name="busyViewModel">The busy view model.</param>
        protected virtual void NoBusyIndicatorFound(object sourceViewModel, object busyViewModel)
        {
            var activator = busyViewModel as IActivate;
            if(activator == null)
                return;

            activator.Activated += (s, e) =>{
                if(!e.WasInitialized)
                    return;

                var info = new BusyInfo {
                    BusyViewModel = busyViewModel
                };
                Indicators[sourceViewModel] = info;
                UpdateIndicator(info);
            };

            Log.Warn("No busy indicator was found in the UI hierarchy. Using modal dialog.");
            WindowManager.ShowDialog(busyViewModel, null);
        }

        /// <summary>
        /// Updates the indicator.
        /// </summary>
        /// <param name="info">The info.</param>
        protected virtual void UpdateIndicator(BusyInfo info)
        {
            lock(IndicatorLock)
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

        /// <summary>
        /// Toggles the busy indicator.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="isBusy">if set to <c>true</c> should be busy.</param>
        protected void ToggleBusyIndicator(BusyInfo info, bool isBusy)
        {
            if(info.BusyIndicator != null)
            {
                var busyProperty = info.BusyIndicator.GetType().GetProperty("IsBusy");
                if(busyProperty != null)
                    busyProperty.SetValue(info.BusyIndicator, isBusy, null);
                else
                    info.BusyIndicator.Visibility = isBusy ? Visibility.Visible : Visibility.Collapsed;
            }
            else if(!isBusy)
            {
                var close = info.BusyViewModel.GetType().GetMethod("Close", Type.EmptyTypes);
                if(close != null)
                    close.Invoke(info.BusyViewModel, null);
            }
        }

        /// <summary>
        /// Finds the busy indicator for the provided view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The busy indicator, or null if not found.</returns>
        protected UIElement FindBusyIndicatorCore(DependencyObject view)
        {
            UIElement busyIndicator = null;

            while(view != null && busyIndicator == null)
            {
                busyIndicator = view.FindName("busyIndicator") as UIElement;
                view = view.GetParent();
            }

            return busyIndicator;
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        protected virtual UIElement GetView(object viewModel)
        {
            var viewAware = viewModel as IViewAware;
            if(viewAware == null)
                return null;

            return viewAware.GetView(null) as UIElement;
        }

        UIElement TryFindBusyIndicator(object viewModel)
        {
            DependencyObject view = GetView(viewModel);
            if(view == null)
            {
                Log.Warn("Could not find view for {0}.", viewModel);
                return null;
            }

            view = View.GetFirstNonGeneratedView(view);
            return FindBusyIndicatorCore(view);
        }

        /// <summary>
        /// Stores information on currently active busy indicators.
        /// </summary>
        protected class BusyInfo
        {
            /// <summary>
            /// Gets or sets the busy indicator.
            /// </summary>
            /// <value>The busy indicator.</value>
            public UIElement BusyIndicator { get; set; }

            /// <summary>
            /// Gets or sets the busy view model.
            /// </summary>
            /// <value>The busy view model.</value>
            public object BusyViewModel { get; set; }

            /// <summary>
            /// Gets or sets the depth.
            /// </summary>
            /// <value>The depth.</value>
            public int Depth { get; set; }
        }
    }
}