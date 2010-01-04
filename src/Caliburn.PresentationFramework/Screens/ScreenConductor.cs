namespace Caliburn.PresentationFramework.Screens
{
    using System;
    using ApplicationModel;

    /// <summary>
    /// A base implementation of <see cref="IScreenConductor"/>.
    /// </summary>
    public partial class ScreenConductor<T> : ScreenConductorBase<T>
        where T : class, IScreen
    {
        private T _activeScreen;
        private bool _changingThroughProperty;

        /// <summary>
        /// Gets the screens that are currently conducted.
        /// </summary>
        /// <value>The screens.</value>
        public override IObservableCollection<T> Screens
        {
            get { return new BindableCollection<T> { _activeScreen }; }
        }

        /// <summary>
        /// Determines whether this instance can shutdown.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can shutdown; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanShutdown()
        {
            if (_activeScreen != null)
                return _activeScreen.CanShutdown();

            return true;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
            if (!IsInitialized)
            {
                OnInitialize();

                if (_activeScreen != null)
                    _activeScreen.Initialize();

                IsInitialized = true;
            }
        }

        /// <summary>
        /// Shuts down this instance.
        /// </summary>
        public override void Shutdown()
        {
            if (_activeScreen != null)
                _activeScreen.Shutdown();

            OnShutdown();
        }

        /// <summary>
        /// Activates this instance.
        /// </summary>
        public override void Activate()
        {
            if (!IsActive)
            {
                OnActivate();

                if (_activeScreen != null)
                    _activeScreen.Activate();

                IsActive = true;
            }
        }

        /// <summary>
        /// Deactivates this instance.
        /// </summary>
        public override void Deactivate()
        {
            if (IsActive)
            {
                OnDeactivate();

                if (_activeScreen != null)
                    _activeScreen.Deactivate();

                IsActive = false;
            }
        }

        /// <summary>
        /// Gets or sets the active screen.
        /// </summary>
        /// <value>The active screen.</value>
        public override T ActiveScreen
        {
            get { return _activeScreen; }
            set
            {
                _changingThroughProperty = true;

                ShutdownActiveScreen(
                    isShutdownSuccess =>
                    {
                        if (isShutdownSuccess)
                        {
                            OpenScreen(
                                value,
                                isOpenSuccess =>
                                {
                                    _changingThroughProperty = false;
                                    NotifyOfPropertyChange(() => ActiveScreen);
                                });
                        }
                        else
                        {
                            _changingThroughProperty = false;
                            NotifyOfPropertyChange(() => ActiveScreen);
                        }
                    });
            }
        }

        /// <summary>
        /// Opens the specified screen.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <param name="completed">Called when the open action is finished.</param>
        public override void OpenScreen(T screen, Action<bool> completed)
        {
            if (screen == null)
            {
                completed(false);
                return;
            }

            Action successfulCompletion =
                () =>{
                    var node = screen as IHierarchicalScreen;
                    if(node != null) node.Parent = this;

                    screen.Initialize();
                    screen.Activate();

                    ChangeActiveScreenCore(screen);

                    completed(true);
                };

            if (_activeScreen != null)
            {
                CanShutdownScreen(
                    _activeScreen,
                    isSuccess =>
                    {
                        if (!isSuccess)
                        {
                            completed(false);
                            return;
                        }

                        _activeScreen.Deactivate();
                        _activeScreen.Shutdown();

                        var node = _activeScreen as IHierarchicalScreen;
                        if (node != null) node.Parent = null;

                        successfulCompletion();
                    });
            }
            else successfulCompletion();
        }

        /// <summary>
        /// Shuts down the specified screen.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <param name="completed">Called when the open action is finished.</param>
        public override void ShutdownScreen(T screen, Action<bool> completed)
        {
            ShutdownActiveScreen(completed);
        }

        /// <summary>
        /// Shuts down the active screen.
        /// </summary>
        /// <param name="completed">Called when the shutdown action is finished.</param>
        public override void ShutdownActiveScreen(Action<bool> completed)
        {
            if (_activeScreen == null)
            {
                completed(true);
                return;
            }

            CanShutdownScreen(
                _activeScreen,
                isSuccess =>
                {
                    if (!isSuccess)
                    {
                        completed(false);
                        return;
                    }

                    _activeScreen.Deactivate();
                    _activeScreen.Shutdown();

                    var node = _activeScreen as IHierarchicalScreen;
                    if (node != null) node.Parent = null;

                    ChangeActiveScreenCore(null);

                    completed(true);
                });
        }

        /// <summary>
        /// Changes the active screen.
        /// </summary>
        /// <param name="newActiveScreen">The new active screen.</param>
        protected virtual void ChangeActiveScreenCore(T newActiveScreen)
        {
            _activeScreen = newActiveScreen;

            if (!_changingThroughProperty)
                NotifyOfPropertyChange(() => ActiveScreen);
        }

        /// <summary>
        /// Creates the shutdown model.
        /// </summary>
        /// <returns></returns>
        public override ISubordinate CreateShutdownModel()
        {
            var custom = _activeScreen as ISupportCustomShutdown;

            if (custom != null)
            {
                var childModel = custom.CreateShutdownModel();

                if (childModel != null)
                    return new SubordinateContainer(this, childModel);
            }

            return null;
        }

        /// <summary>
        /// Determines whether this instance can shutdown based on the evaluated shutdown model.
        /// </summary>
        /// <param name="shutdownModel">The shutdown model.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can shutdown; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanShutdown(ISubordinate shutdownModel)
        {
            var container = (SubordinateContainer)shutdownModel;
            var custom = (ISupportCustomShutdown)_activeScreen;

            return custom.CanShutdown(container.Child);
        }
    }
}