namespace Caliburn.PresentationFramework.Screens
{
    using System;
    using System.Windows;
    using Core.Metadata;
    using Metadata;

    /// <summary>
    /// Implements common functionality used by all implementors of <see cref="IScreen"/>.
    /// </summary>
    public abstract class ScreenBase : MetadataContainer, IScreenEx
    {
        private IScreenCollection _parent;
        private bool _isActive;
        private bool _isInitialized;
        private string _displayName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenBase"/> class.
        /// </summary>
        protected ScreenBase()
        {
            var runtimeType = GetType();

            _displayName = runtimeType.Name;

            AddMetadataFrom(runtimeType);
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public virtual IScreenCollection Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                NotifyOfPropertyChange(() => Parent);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsInitialized
        {
            get { return _isInitialized; }
            protected set
            {
                _isInitialized = value;
                NotifyOfPropertyChange(() => IsInitialized);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public virtual bool IsActive
        {
            get { return _isActive; }
            protected set
            {
                _isActive = value;
                NotifyOfPropertyChange(() => IsActive);
            }
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public virtual string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                NotifyOfPropertyChange(() => DisplayName);
            }
        }

        /// <summary>
        /// Determines whether this instance can shutdown.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can shutdown; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanShutdown()
        {
            AttemptingShutdown(this, EventArgs.Empty);
            return CanShutdownCore();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Shuts down this instance.
        /// </summary>
        public abstract void Shutdown();

        /// <summary>
        /// Activates this instance.
        /// </summary>
        public abstract void Activate();

        /// <summary>
        /// Deactivates this instance.
        /// </summary>
        public abstract void Deactivate();

        /// <summary>
        /// Occurs when [initialized].
        /// </summary>
        public event EventHandler Initialized = delegate { };

        /// <summary>
        /// Occurs before attempting to shutdown.
        /// </summary>
        public virtual event EventHandler AttemptingShutdown = delegate { };

        /// <summary>
        /// Occurs when [was shutdown].
        /// </summary>
        public event EventHandler WasShutdown = delegate { };

        /// <summary>
        /// Occurs when [activated].
        /// </summary>
        public event EventHandler Activated = delegate { };

        /// <summary>
        /// Occurs when [deactivated].
        /// </summary>
        public event EventHandler Deactivated = delegate { };

        /// <summary>
        /// Called when [initialize].
        /// </summary>
        protected virtual void OnInitialize()
        {
            Initialized(this, EventArgs.Empty);
        }

        /// <summary>
        /// Determines whether this instance can shutdown.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can shutdown; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanShutdownCore()
        {
            return true;
        }

        /// <summary>
        /// Called when [shutdown].
        /// </summary>
        protected virtual void OnShutdown()
        {
            WasShutdown(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when [activate].
        /// </summary>
        protected virtual void OnActivate()
        {
            Activated(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when [deactivate].
        /// </summary>
        protected virtual void OnDeactivate()
        {
            Deactivated(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when the screen's view is loaded.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="context">The context.</param>
        public virtual void ViewLoaded(DependencyObject view, object context) {}

        /// <summary>
        /// Closes this instance by asking its Parent to initiate shutdown or by asking it's corresponding default view to close.
        /// </summary>
        public virtual void Close()
        {
            if (Parent != null)
                Parent.ShutdownScreen(this, delegate { });
            else
            {
                var view = this.GetView<object>(null);

                if (view == null)
                    throw new NotSupportedException(
                        "You cannot close an instance without a parent or a default view."
                        );

                var method = view.GetType().GetMethod("Close");

                if (method == null)
                    throw new NotSupportedException(
                        "The default view does not support the Close operation."
                        );

                method.Invoke(view, null);
            }
        }

#if !SILVERLIGHT

        /// <summary>
        /// Closes this instance by asking its Parent to initiate shutdown or by asking it's corresponding default view to close.
        /// This overload also provides an opportunity to pass a dialog result to it's corresponding default view.
        /// </summary>
        /// <param name="dialogResult">The dialog result.</param>
        public virtual void Close(bool? dialogResult)
        {
            var view = this.GetView<object>(null);

            if(view != null)
            {
                var property = view.GetType().GetProperty("DialogResult");
                if(property != null)
                    property.SetValue(view, dialogResult, null);
            }

            Close();
        }

#endif
    }
}