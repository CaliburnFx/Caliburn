namespace Caliburn.PresentationFramework.Screens
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ApplicationModel;
    using Behaviors;

    /// <summary>
    /// An implementation of <see cref="IScreenConductor"/>.
    /// </summary>
    /// <typeparam name="T">A type of <see cref="IScreen"/>.</typeparam>
    public partial class ScreenConductor<T>
        where T : class, IScreen
    {
        /// <summary>
        /// A class container for implementations of <see cref="IScreenConductor"/> that use a screen collection.
        /// </summary>
        public abstract partial class WithCollection
        {
            /// <summary>
            /// An implementation of <see cref="IScreenCollection"/> that activates/deactivates all child screens together.
            /// </summary>
            public class AllScreensActive : ScreenCollectionBase<T>
            {
                private readonly IObservableCollection<T> _screens = new BindableCollection<T>();

                /// <summary>
                /// Gets the screens that are currently managed.
                /// </summary>
                /// <value>The screens.</value>
                [DoNotNotify]
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

                        foreach (var screen in _screens)
                        {
                            screen.Initialize();
                        }

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

                        foreach (var screen in _screens)
                        {
                            screen.Activate();
                        }

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

                        foreach (var screen in _screens)
                        {
                            screen.Deactivate();
                        }

                        IsActive = false;
                    }
                }

                /// <summary>
                /// Opens the specified screen.
                /// </summary>
                /// <param name="screen">The presenter.</param>
                /// <param name="completed">Called when the open action is finished.</param>
                public override void OpenScreen(T screen, Action<bool> completed)
                {
                    if (screen == null)
                    {
                        completed(false);
                        return;
                    }

                    screen = EnsurePresenter(screen);
                    screen.Activate();

                    completed(true);
                }

                private T EnsurePresenter(T screen)
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
                /// Shuts down the specified screen.
                /// </summary>
                /// <param name="screen">The screen.</param>
                /// <param name="completed">Called when the open action is finished.</param>
                public override void ShutdownScreen(T screen, Action<bool> completed)
                {
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

                    foreach (var presenter in _screens)
                    {
                        var match = (from child in subordinateGroup
                                     where child.Master == presenter
                                     select child).FirstOrDefault();

                        if (match == null)
                        {
                            if (presenter.CanShutdown())
                                screensToRemove.Add(presenter);
                            else result = false;
                        }
                        else
                        {
                            var custom = (ISupportCustomShutdown)presenter;
                            var canShutdown = custom.CanShutdown(match);

                            if (canShutdown)
                                screensToRemove.Add(presenter);
                            else result = false;
                        }
                    }

                    FinalizeShutdown(result, screensToRemove);

                    return result;
                }

                /// <summary>
                /// Finalizes the shutdown of some or all child screens.
                /// </summary>
                /// <param name="canShutdown">if set to <c>true</c> all screens in the Screen Collection can shutdown.</param>
                /// <param name="allowedToShutdown">Only the screens which are allowed to shutdown.</param>
                protected virtual void FinalizeShutdown(bool canShutdown, IEnumerable<T> allowedToShutdown)
                {
                    foreach (var presenter in allowedToShutdown)
                    {
                        _screens.Remove(presenter);
                        presenter.Deactivate();
                        presenter.Shutdown();
                    }
                }
            }
        }
    }
}