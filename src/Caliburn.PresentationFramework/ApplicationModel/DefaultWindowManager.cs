#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// An implementation of <see cref="IWindowManager"/>.
    /// </summary>
    public class DefaultWindowManager : IWindowManager
    {
        private readonly IViewStrategy _viewStrategy;
        private readonly IBinder _binder;
        private bool _actuallyClosing;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWindowManager"/> class.
        /// </summary>
        /// <param name="viewStrategy">The view strategy.</param>
        /// <param name="binder">The default binder.</param>
        public DefaultWindowManager(IViewStrategy viewStrategy, IBinder binder)
        {
            _viewStrategy = viewStrategy;
            _binder = binder;
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        /// <param name="handleShutdownModel">The handle shutdown model.</param>
        /// <returns></returns>
        public bool? ShowDialog(object rootModel, object context, Action<ISubordinate, Action> handleShutdownModel)
        {
            var window = CreateWindow(rootModel, context, handleShutdownModel);
            return window.ShowDialog();
        }

        /// <summary>
        /// Shows a window for the specified model.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The context.</param>
        /// <param name="handleShutdownModel">The handle shutdown model.</param>
        public void Show(object rootModel, object context, Action<ISubordinate, Action> handleShutdownModel)
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
        protected Window CreateWindow(object rootModel, object context, Action<ISubordinate, Action> handleShutdownModel)
        {
            var view = EnsureWindow(rootModel, _viewStrategy.GetView(rootModel, null, context));

            _binder.Bind(rootModel, view, context);

            var presenter = rootModel as IPresenter;
            if (presenter != null)
            {
                presenter.Initialize();
                presenter.Activate();

                view.Closing += (s, e) => OnShutdownAttempted(presenter, view, handleShutdownModel, e);

                view.Closed += delegate
                {
                    presenter.Deactivate();
                    presenter.Shutdown();
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
        protected virtual Window EnsureWindow(object model, object view)
        {
            var window = view as Window;

            if (window == null)
            {
                window = new Window
                {
                    Content = view,
                    SizeToContent = SizeToContent.WidthAndHeight
                };

                if (Application.Current != null
                   && Application.Current.MainWindow != null
                    && Application.Current.MainWindow != window)
                {
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    window.Owner = Application.Current.MainWindow;
                }
                else window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                var presenter = model as IPresenter;
                if (presenter != null)
                {
                    var binding = new Binding("DisplayName") { Mode = BindingMode.TwoWay };
                    window.SetBinding(Window.TitleProperty, binding);
                }
            }
            else if (Application.Current != null
                   && Application.Current.MainWindow != null)
            {
                if (Application.Current.MainWindow != window)
                    window.Owner = Application.Current.MainWindow;
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
        protected virtual void OnShutdownAttempted(IPresenter rootModel, Window view, Action<ISubordinate, Action> handleShutdownModel, CancelEventArgs e)
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