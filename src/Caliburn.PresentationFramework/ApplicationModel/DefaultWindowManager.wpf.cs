#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Navigation;
    using Screens;
    using ViewModels;

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
        private readonly IViewModelBinder _viewModelBinder;
        private bool _actuallyClosing;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWindowManager"/> class.
        /// </summary>
        /// <param name="viewLocator">The view locator.</param>
        /// <param name="viewModelBinder">The view model binder.</param>
        public DefaultWindowManager(IViewLocator viewLocator, IViewModelBinder viewModelBinder)
        {
            _viewLocator = viewLocator;
            _viewModelBinder = viewModelBinder;
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        /// <param name="handleShutdownModel">The handle shutdown model.</param>
        /// <returns></returns>
        public virtual bool? ShowDialog(object rootModel, object context, Action<ISubordinate, Action> handleShutdownModel)
        {
            var window = CreateWindow(rootModel, true, context, handleShutdownModel);
            return window.ShowDialog();
        }

        /// <summary>
        /// Shows a window for the specified model.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        /// <param name="handleShutdownModel">The handle shutdown model.</param>
        public virtual void Show(object rootModel, object context, Action<ISubordinate, Action> handleShutdownModel)
        {
            var navWindow = Application.Current.MainWindow as NavigationWindow;

            if (navWindow != null)
            {
                var window = CreatePage(rootModel, context, handleShutdownModel);
                navWindow.Navigate(window);
            }
            else
            {
                var window = CreateWindow(rootModel, false, context, handleShutdownModel);
                window.Show();
            }
        }

        /// <summary>
        /// Creates the window.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="isDialog">Indicates it is a dialog window.</param>
        /// <param name="context">The context.</param>
        /// <param name="handleShutdownModel">The handle shutdown model.</param>
        /// <returns></returns>
        protected virtual Window CreateWindow(object rootModel, bool isDialog, object context, Action<ISubordinate, Action> handleShutdownModel)
        {
            var view = EnsureWindow(rootModel, _viewLocator.Locate(rootModel, null, context), isDialog);

            _viewModelBinder.Bind(rootModel, view, context);

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
        /// <param name="isDialog">Indicates we are insuring a dialog window.</param>
        /// <returns></returns>
        protected virtual Window EnsureWindow(object model, object view, bool isDialog)
        {
            var window = view as Window;

            if (window == null)
            {
                window = new Window
                {
                    Content = view,
                    SizeToContent = SizeToContent.WidthAndHeight
                };

                window.SetValue(IsElementGeneratedProperty, true);

                if (Application.Current != null
                   && Application.Current.MainWindow != null
                    && Application.Current.MainWindow != window)
                {
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    window.Owner = Application.Current.MainWindow;
                }
                else window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                var screen = model as IScreen;
                if (screen != null)
                {
                    var binding = new Binding("DisplayName") { Mode = BindingMode.TwoWay };
                    window.SetBinding(Window.TitleProperty, binding);
                }
            }
            else if (Application.Current != null
                   && Application.Current.MainWindow != null)
            {
                if (Application.Current.MainWindow != window && isDialog)
                    window.Owner = Application.Current.MainWindow;
            }

            return window;
        }

        /// <summary>
        /// Creates the page.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        /// <param name="handleShutdownModel">The handle shutdown model.</param>
        /// <returns></returns>
        public Page CreatePage(object rootModel, object context, Action<ISubordinate, Action> handleShutdownModel)
        {
            var view = EnsurePage(rootModel, _viewLocator.Locate(rootModel, null, context));

            _viewModelBinder.Bind(rootModel, view, context);

            var screen = rootModel as IScreen;
            if (screen != null)
            {
                view.Unloaded += delegate
                {
                    screen.Deactivate();
                    screen.Shutdown();
                };
            }

            return view;
        }

        /// <summary>
        /// Ensures the view is a page or provides one.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        protected Page EnsurePage(object model, object view)
        {
            var page = view as Page;

            if (page == null)
            {
                page = new Page
                {
                    Content = view
                };

                page.SetValue(IsElementGeneratedProperty, true);
            }

            return page;
        }

        /// <summary>
        /// Called when shutdown attempted.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="view">The view.</param>
        /// <param name="handleShutdownModel">The handler for the shutdown model.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        protected virtual void OnShutdownAttempted(IScreen rootModel, Window view, Action<ISubordinate, Action> handleShutdownModel, CancelEventArgs e)
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