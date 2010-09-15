#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Navigation;
    using Conventions;
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

        private readonly IViewLocator viewLocator;
        private readonly IViewModelBinder viewModelBinder;
        private bool actuallyClosing;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWindowManager"/> class.
        /// </summary>
        /// <param name="viewLocator">The view locator.</param>
        /// <param name="viewModelBinder">The view model binder.</param>
        public DefaultWindowManager(IViewLocator viewLocator, IViewModelBinder viewModelBinder)
        {
            this.viewLocator = viewLocator;
            this.viewModelBinder = viewModelBinder;
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public virtual bool? ShowDialog(object rootModel, object context)
        {
            var window = CreateWindow(rootModel, true, context);
            return window.ShowDialog();
        }

        /// <summary>
        /// Shows a window for the specified model.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        public virtual void Show(object rootModel, object context)
        {
            var navWindow = Application.Current.MainWindow as NavigationWindow;

            if (navWindow != null)
            {
                var window = CreatePage(rootModel, context);
                navWindow.Navigate(window);
            }
            else
            {
                var window = CreateWindow(rootModel, false, context);
                window.Show();
            }
        }

        /// <summary>
        /// Creates the window.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="isDialog">Indicates it is a dialog window.</param>
        /// <param name="context">The context.</param>
        /// <returns>The window.</returns>
        protected virtual Window CreateWindow(object rootModel, bool isDialog, object context)
        {
            var view = EnsureWindow(rootModel, viewLocator.Locate(rootModel, null, context), isDialog);
            viewModelBinder.Bind(rootModel, view, context);

            var haveDisplayName = rootModel as IHaveDisplayName;
            if (haveDisplayName != null && !view.HasBinding(Window.TitleProperty))
            {
                var binding = new Binding("DisplayName") { Mode = BindingMode.TwoWay };
                view.SetBinding(Window.TitleProperty, binding);
            }

            var activatable = rootModel as IActivate;
            if (activatable != null)
                activatable.Activate();

            var deactivatable = rootModel as IDeactivate;
            if (deactivatable != null)
                view.Closed += (s, e) => deactivatable.Deactivate(true);

            var guard = rootModel as IGuardClose;
            if (guard != null)
                view.Closing += (s, e) => OnShutdownAttempted(guard, view, e);

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
        /// <returns></returns>
        public Page CreatePage(object rootModel, object context)
        {
            var view = EnsurePage(rootModel, viewLocator.Locate(rootModel, null, context));
            viewModelBinder.Bind(rootModel, view, context);

            var haveDisplayName = rootModel as IHaveDisplayName;
            if (haveDisplayName != null)
            {
                var binding = new Binding("DisplayName") { Mode = BindingMode.TwoWay };
                view.SetBinding(Page.TitleProperty, binding);
            }

            var activatable = rootModel as IActivate;
            if (activatable != null)
                activatable.Activate();

            var deactivatable = rootModel as IDeactivate;
            if (deactivatable != null)
                view.Unloaded += (s, e) => deactivatable.Deactivate(true);

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
        /// <param name="guard">The guard.</param>
        /// <param name="view">The view.</param>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        void OnShutdownAttempted(IGuardClose guard, Window view, CancelEventArgs e)
        {
            if (actuallyClosing)
            {
                actuallyClosing = false;
                return;
            }

            bool runningAsync = false, shouldEnd = false;

            guard.CanClose(canClose =>{
                if(runningAsync && canClose)
                {
                    actuallyClosing = true;
                    view.Close();
                }
                else e.Cancel = !canClose;

                shouldEnd = true;
            });

            if (shouldEnd)
                return;

            runningAsync = e.Cancel = true;
        }
    }
}

#endif