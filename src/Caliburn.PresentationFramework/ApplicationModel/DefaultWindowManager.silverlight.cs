#if SILVERLIGHT_30 || SILVERLIGHT_40

namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using PresentationFramework.ApplicationModel;
    using Screens;
    using ViewModels;
    using Views;

    /// <summary>
    /// An implementation of <see cref="IWindowManager"/>.
    /// </summary>
    public class DefaultWindowManager : IWindowManager
    {
        private static readonly DependencyProperty IsElementGeneratedProperty =
            DependencyProperty.RegisterAttached(
                "IsElementGenerated",
                typeof(bool),
                typeof(DefaultWindowManager),
                new PropertyMetadata(false, null)
                );

        /// <summary>
        /// Gets the significant view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The non-generated view that was wrapped by Caliburn.</returns>
        public static DependencyObject GetSignificantView(DependencyObject view)
        {
            if ((bool)view.GetValue(IsElementGeneratedProperty))
                return (DependencyObject)((ContentControl)view).Content;

            return view;
        }

        private readonly IViewLocator _viewLocator;
        private readonly IViewModelBinder _binder;
        private bool _actuallyClosing;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWindowManager"/> class.
        /// </summary>
        /// <param name="viewLocator">The view locator.</param>
        /// <param name="binder">The binder.</param>
        public DefaultWindowManager(IViewLocator viewLocator, IViewModelBinder binder)
        {
            _viewLocator = viewLocator;
            _binder = binder;
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        /// <param name="handleShutdownModel">The handle shutdown model.</param>
        /// <returns></returns>
        public virtual void ShowDialog(object rootModel, object context, Action<ISubordinate, Action> handleShutdownModel)
        {
            var window = CreateWindow(rootModel, context, handleShutdownModel);
            window.Show();
        }

        /// <summary>
        /// Creates the window.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        /// <param name="handleShutdownModel">The handle shutdown model.</param>
        /// <returns></returns>
        protected virtual ChildWindow CreateWindow(object rootModel, object context, Action<ISubordinate, Action> handleShutdownModel)
        {
            var view = EnsureWindow(rootModel, _viewLocator.Locate(rootModel, null, context));

            _binder.Bind(rootModel, view, context);

            var screen = rootModel as IScreen;
            if (screen != null)
            {
                screen.Initialize();
                screen.Activate();

                view.Closing += (s, e) => OnShutdownAttempted(screen, view, handleShutdownModel, e);

                view.Closed += delegate
                {
                    screen.Deactivate();
                    screen.Shutdown();
                };
            }

            return view;
        }

        /// <summary>
        /// Ensures the that the view is a window or provides one.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        protected virtual ChildWindow EnsureWindow(object model, object view)
        {
            var window = view as ChildWindow;

            if (window == null)
            {
                window = new ChildWindow
                {
                    Content = view,
                };

                window.SetValue(IsElementGeneratedProperty, true);

                var screen = model as IScreen;
                if (screen != null)
                {
                    var binding = new Binding("DisplayName") { Mode = BindingMode.TwoWay };
                    window.SetBinding(ChildWindow.TitleProperty, binding);
                }
            }

            return window;
        }

        /// <summary>
        /// Called when shutdown attempted.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="view">The view.</param>
        /// <param name="handleShutdownModel">The handler for the shutdown model.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        protected virtual void OnShutdownAttempted(IScreen rootModel, ChildWindow view, Action<ISubordinate, Action> handleShutdownModel, CancelEventArgs e)
        {
            if (_actuallyClosing || rootModel.CanShutdown())
            {
                _actuallyClosing = false;
                return;
            }

            bool runningAsync = false;

            var custom = rootModel as ISupportCustomShutdown;
            if (custom != null && handleShutdownModel != null)
            {
                var shutdownModel = custom.CreateShutdownModel();
                var shouldEnd = false;

                handleShutdownModel(
                    shutdownModel,
                    () =>
                    {
                        var canShutdown = custom.CanShutdown(shutdownModel);
                        if (runningAsync && canShutdown)
                        {
                            _actuallyClosing = true;
                            view.Close();
                        }
                        else e.Cancel = !canShutdown;

                        shouldEnd = true;
                    });

                if (shouldEnd)
                    return;
            }

            runningAsync = e.Cancel = true;
        }
    }
}

#endif