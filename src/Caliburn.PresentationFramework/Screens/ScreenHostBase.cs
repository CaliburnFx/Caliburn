namespace Caliburn.PresentationFramework.Screens
{
    using System;
    using System.Linq;
    using ApplicationModel;

    /// <summary>
    /// An baseclass for implementations of <see cref="IScreenHost"/>.
    /// </summary>
    /// <typeparam name="T">A type of <see cref="IScreen"/>.</typeparam>
    public abstract class ScreenHostBase<T> : ScreenBase, IScreenHost<T>, ISupportCustomShutdown
        where T : class, IScreen
    {
        /// <summary>
        /// Gets the screens that are currently managed.
        /// </summary>
        /// <value>The screens.</value>
        public abstract IObservableCollection<T> Screens { get; }

        /// <summary>
        /// Gets the screens that are currently managed.
        /// </summary>
        /// <value>The screens.</value>
        IObservableCollection<IScreen> IScreenHost.Screens
        {
            get { return new BindableCollection<IScreen>(Screens.OfType<IScreen>()); }
        }

        /// <summary>
        /// Opens the specified screen.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <param name="completed">Called when the open action is finished.</param>
        public abstract void OpenScreen(T screen, Action<bool> completed);

        /// <summary>
        /// Opens the specified screen.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <param name="completed">Called when the open action is finished.</param>
        void IScreenHost.OpenScreen(IScreen screen, Action<bool> completed)
        {
            OpenScreen((T)screen, completed);
        }

        /// <summary>
        /// Shuts down the specified screen.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <param name="completed">Called when the open action is finished.</param>
        public abstract void ShutdownScreen(T screen, Action<bool> completed);

        /// <summary>
        /// Shuts down the specified screen.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <param name="completed">Called when the open action is finished.</param>
        void IScreenHost.ShutdownScreen(IScreen screen, Action<bool> completed)
        {
            ShutdownScreen((T)screen, completed);
        }

        /// <summary>
        /// Creates the shutdown model.
        /// </summary>
        /// <returns></returns>
        public abstract ISubordinate CreateShutdownModel();

        /// <summary>
        /// Determines whether this instance can shutdown based on the evaluated shutdown model.
        /// </summary>
        /// <param name="shutdownModel">The shutdown model.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can shutdown; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool CanShutdown(ISubordinate shutdownModel);

        /// <summary>
        /// Determines if the specified screen can be shut down.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <param name="completed">Called when the shutdown action is finished.</param>
        protected virtual void CanShutdownScreen(IScreen screen, Action<bool> completed)
        {
            var canShutdown = screen.CanShutdown();

            if (!canShutdown)
            {
                var custom = screen as ISupportCustomShutdown;

                if (custom != null)
                {
                    var model = custom.CreateShutdownModel();

                    if (model != null)
                    {
                        ExecuteShutdownModel(
                            model,
                            () => completed(custom.CanShutdown(model))
                            );

                        return;
                    }
                }
            }

            completed(canShutdown);
        }

        /// <summary>
        /// Inheritors should override this method if they intend to handle advanced shutdown scenarios.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="completed">Called when the shutdown model is finished executing.</param>
        protected virtual void ExecuteShutdownModel(ISubordinate model, Action completed)
        {
            completed();
        }
    }
}