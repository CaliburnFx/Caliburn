namespace Caliburn.PresentationFramework.Screens
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ApplicationModel;

    /// <summary>
    /// An implementation of <see cref="IScreenConductor"/>.
    /// </summary>
    public partial class ScreenConductor<T>
            where T : class, IScreen
    {
        /// <summary>
        /// A class container for implementations of <see cref="IScreenConductor"/> that use a screen collection.
        /// </summary>
        public abstract partial class WithCollection
        {
            private WithCollection() {}

            /// <summary>
            /// An implementation of <see cref="IScreenConductor"/> that allows only one screen to be active at a time.
            /// </summary>
            public class OneScreenActive : ScreenConductorBase<T>
            {
                private readonly IObservableCollection<T> _screens = new BindableCollection<T>();
                private T _activeScreen;
                private bool _changingThroughProperty;

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

                        OpenScreen(
                            value,
                            success =>{
                                _changingThroughProperty = false;
                                NotifyOfPropertyChange(() => ActiveScreen);
                            });
                    }
                }

                /// <summary>
                /// Gets the presenters that are currently managed.
                /// </summary>
                /// <value>The presenters.</value>
                public override IObservableCollection<T> Screens
                {
                    get { return _screens; }
                }

                /// <summary>
                /// Determines whether this instance can shutdown.
                /// </summary>
                /// <returns>
                /// 	<c>true</c> if this instance can shutdown; otherwise, <c>false</c>.
                /// </returns>
                public override bool CanShutdown()
                {
                    foreach (var screen in _screens)
                    {
                        if (!screen.CanShutdown()) return false;
                    }

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
                /// Shutdowns this instance.
                /// </summary>
                public override void Shutdown()
                {
                    foreach (var screen in _screens)
                    {
                        screen.Shutdown();
                    }

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

                    if (_activeScreen != null)
                        _activeScreen.Deactivate();

                    screen = EnsureScreen(screen);
                    screen.Activate();

                    ChangeActiveScreenCore(screen);

                    completed(true);
                }

                private T EnsureScreen(T screen)
                {
                    int index = _screens.IndexOf(screen);

                    if (index == -1)
                        _screens.Add(screen);
                    else screen = _screens[index];

                    var node = screen as IHierarchicalScreen;
                    if (node != null) node.Parent = this;

                    screen.Initialize();

                    return screen;
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

                            int index = _screens.IndexOf(_activeScreen);

                            _screens.Remove(_activeScreen);
                            _activeScreen.Deactivate();
                            _activeScreen.Shutdown();

                            var node = _activeScreen as IHierarchicalScreen;
                            if (node != null) node.Parent = null;

                            var next = DetermineNextScreenToActivate(index);

                            if (next != null)
                            {
                                next = EnsureScreen(next);
                                next.Activate();
                            }

                            ChangeActiveScreenCore(next);

                            completed(true);
                        });
                }

                /// <summary>
                /// Gets the next screen to activate after another screen is shutdown.
                /// </summary>
                /// <param name="lastIndex">The index of the previously removed screen.</param>
                /// <returns>The screen to activate or null if none.</returns>
                protected virtual T DetermineNextScreenToActivate(int lastIndex)
                {
                    var toRemoveAt = lastIndex - 1;

                    if (toRemoveAt == -1 && _screens.Count > 0)
                        return _screens[0];
                    if (toRemoveAt > -1 && toRemoveAt < _screens.Count)
                        return _screens[toRemoveAt];
                    return null;
                }

                /// <summary>
                /// Shuts down the specified screen.
                /// </summary>
                /// <param name="screen">The screen.</param>
                /// <param name="completed">Called when the shutdown action is finished.</param>
                public override void ShutdownScreen(T screen, Action<bool> completed)
                {
                    if (_activeScreen == screen)
                    {
                        ShutdownActiveScreen(completed);
                        return;
                    }

                    if (screen == null)
                    {
                        completed(true);
                        return;
                    }

                    CanShutdownScreen(
                        screen,
                        isSuccess =>
                        {
                            if (!isSuccess)
                            {
                                completed(false);
                                return;
                            }

                            _screens.Remove(screen);

                            screen.Deactivate();
                            screen.Shutdown();

                            var node = screen as IHierarchicalScreen;
                            if (node != null) node.Parent = null;

                            completed(true);
                        });
                }

                /// <summary>
                /// Changes the active screen.
                /// </summary>
                /// <param name="newActiveScreen">The new current presenter.</param>
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
                    var model = new SubordinateGroup(this);

                    foreach (var presenter in _screens)
                    {
                        if (presenter.CanShutdown()) continue;

                        var custom = presenter as ISupportCustomShutdown;

                        if (custom != null)
                        {
                            var childModel = custom.CreateShutdownModel();

                            if (childModel != null)
                                model.Add(childModel);
                        }
                    }

                    return model;
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
                    var subordinateGroup = (SubordinateGroup)shutdownModel;
                    var screensToRemove = new List<T>();
                    bool result = true;

                    foreach (var screen in _screens)
                    {
                        var match = (from child in subordinateGroup
                                     where child.Master == screen
                                     select child).FirstOrDefault();

                        if (match == null)
                        {
                            if (screen.CanShutdown())
                                screensToRemove.Add(screen);
                            else result = false;
                        }
                        else
                        {
                            var custom = (ISupportCustomShutdown)screen;
                            var canShutdown = custom.CanShutdown(match);

                            if (canShutdown)
                                screensToRemove.Add(screen);
                            else result = false;
                        }
                    }

                    FinalizeShutdown(result, screensToRemove);

                    if (_activeScreen == null || !_screens.Contains(_activeScreen))
                    {
                        if (_screens.Count > 0)
                        {
                            _screens[0].Activate();
                            ChangeActiveScreenCore(_screens[0]);
                        }
                    }

                    return result;
                }

                /// <summary>
                /// Finalizes the shutdown of some or all child screens.
                /// </summary>
                /// <param name="canShutdown">if set to <c>true</c> all screens in the Screen collection can shutdown.</param>
                /// <param name="allowedToShutdown">Only the screens which are allowed to shutdown.</param>
                protected virtual void FinalizeShutdown(bool canShutdown, IEnumerable<T> allowedToShutdown)
                {
                    foreach (var screen in allowedToShutdown)
                    {
                        _screens.Remove(screen);
                        screen.Deactivate();
                        screen.Shutdown();
                    }
                }
            }
        }
    }
}