namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using Core;

    /// <summary>
    /// A base implementation of <see cref="IPresenterManager"/>.
    /// </summary>
    public class PresenterManager : PresenterBase, IPresenterManager, ISupportCustomShutdown
    {
        private IPresenter _currentPresenter;
        private bool _changingThroughProperty;

        /// <summary>
        /// Gets the presenters that are currently managed.
        /// </summary>
        /// <value>The presenters.</value>
        public virtual IObservableCollection<IPresenter> Presenters
        {
            get { return new BindableCollection<IPresenter> { _currentPresenter }; }
        }

        /// <summary>
        /// Determines whether this instance can shutdown.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can shutdown; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanShutdown()
        {
            if (_currentPresenter != null)
                return _currentPresenter.CanShutdown();

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

                if (_currentPresenter != null)
                    _currentPresenter.Initialize();

                IsInitialized = true;
            }
        }

        /// <summary>
        /// Shuts down this instance.
        /// </summary>
        public override void Shutdown()
        {
            if (_currentPresenter != null)
                _currentPresenter.Shutdown();

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

                if (_currentPresenter != null)
                    _currentPresenter.Activate();

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

                if (_currentPresenter != null)
                    _currentPresenter.Deactivate();

                IsActive = false;
            }
        }

        /// <summary>
        /// Gets or sets the current presenter.
        /// </summary>
        /// <value>The current presenter.</value>
        public virtual IPresenter CurrentPresenter
        {
            get { return _currentPresenter; }
            set
            {
                _changingThroughProperty = true;

                ShutdownCurrent(
                    isShutdownSuccess =>
                    {
                        if (isShutdownSuccess)
                        {
                            Open(
                                value,
                                isOpenSuccess =>
                                {
                                    _changingThroughProperty = false;
                                    NotifyOfPropertyChange("CurrentPresenter");
                                });
                        }
                        else
                        {
                            _changingThroughProperty = false;
                            NotifyOfPropertyChange("CurrentPresenter");
                        }
                    });
            }
        }

        /// <summary>
        /// Opens the specified presenter.
        /// </summary>
        /// <param name="presenter">The presenter.</param>
        /// <param name="completed">Called when the open action is finished.</param>
        public virtual void Open(IPresenter presenter, Action<bool> completed)
        {
            if (presenter == null)
            {
                completed(false);
                return;
            }

            Action successfulCompletion =
                () =>
                {
                    var node = presenter as IPresenterNode;
                    if (node != null) node.Parent = this;

                    presenter.Initialize();
                    presenter.Activate();

                    ChangeCurrentPresenterCore(presenter);

                    completed(true);
                };

            if (_currentPresenter != null)
            {
                CanShutdownPresenter(
                    _currentPresenter,
                    isSuccess =>
                    {
                        if (!isSuccess)
                        {
                            completed(false);
                            return;
                        }

                        _currentPresenter.Deactivate();
                        _currentPresenter.Shutdown();

                        var node = _currentPresenter as IPresenterNode;
                        if (node != null) node.Parent = null;

                        successfulCompletion();
                    });
            }
            else successfulCompletion();
        }

        /// <summary>
        /// Shuts down the specified presenter.
        /// </summary>
        /// <param name="presenter">The presenter.</param>
        /// <param name="completed">Called when the open action is finished.</param>
        public virtual void Shutdown(IPresenter presenter, Action<bool> completed)
        {
            if (presenter != _currentPresenter)
                throw new CaliburnException("You cannot shutdown a presenter that is not hosted by this manager.");

            ShutdownCurrent(completed);
        }

        /// <summary>
        /// Shuts down the current presenter.
        /// </summary>
        /// <param name="completed">Called when the shutdown action is finished.</param>
        public virtual void ShutdownCurrent(Action<bool> completed)
        {
            if (_currentPresenter == null)
            {
                completed(true);
                return;
            }

            CanShutdownPresenter(
                _currentPresenter,
                isSuccess =>
                {
                    if (!isSuccess)
                    {
                        completed(false);
                        return;
                    }

                    _currentPresenter.Deactivate();
                    _currentPresenter.Shutdown();

                    var node = _currentPresenter as IPresenterNode;
                    if (node != null) node.Parent = null;

                    ChangeCurrentPresenterCore(null);

                    completed(true);
                });
        }

        /// <summary>
        /// Changes the current presenter.
        /// </summary>
        /// <param name="newCurrent">The new current presenter.</param>
        protected virtual void ChangeCurrentPresenterCore(IPresenter newCurrent)
        {
            _currentPresenter = newCurrent;

            if (!_changingThroughProperty)
                NotifyOfPropertyChange("CurrentPresenter");
        }

        /// <summary>
        /// Creates the shutdown model.
        /// </summary>
        /// <returns></returns>
        ISubordinate ISupportCustomShutdown.CreateShutdownModel()
        {
            var custom = _currentPresenter as ISupportCustomShutdown;

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
        bool ISupportCustomShutdown.CanShutdown(ISubordinate shutdownModel)
        {
            var container = (SubordinateContainer)shutdownModel;
            var custom = (ISupportCustomShutdown)_currentPresenter;

            return custom.CanShutdown(container.Child);
        }
    }
}