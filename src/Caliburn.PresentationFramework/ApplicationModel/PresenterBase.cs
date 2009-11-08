namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Core.Metadata;
    using Metadata;

    /// <summary>
    /// Implements common functionality used by all implementors of <see cref="IPresenter"/>.
    /// </summary>
    public abstract class PresenterBase : MetadataContainer, IExtendedPresenter
    {
        private IPresenterHost _parent;
        private bool _isActive;
        private bool _isInitialized;
        private string _displayName;

        /// <summary>
        /// Initializes a new instance of the <see cref="PresenterBase"/> class.
        /// </summary>
        protected PresenterBase()
        {
            var runtimeType = GetType();

            _displayName = runtimeType.Name;

            AddMetadataFrom(runtimeType);
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public virtual IPresenterHost Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                NotifyOfPropertyChange("Parent");
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
                NotifyOfPropertyChange("IsInitialized");
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
                NotifyOfPropertyChange("IsActive");
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
                NotifyOfPropertyChange("DisplayName");
            }
        }

        /// <summary>
        /// Executes the specified <see cref="IResult"/> on the <see cref="IInteractionNode"/> 
        /// associated with the view tied to the context.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <param name="result">The result.</param>
        public virtual void Execute(object viewContext, IResult result)
        {
            ExecuteCore(this, viewContext, new[] {result});
        }

        /// <summary>
        /// Executes the specified <see cref="IResult"/> on the <see cref="IInteractionNode"/>
        /// associated with the default view.
        /// </summary>
        /// <param name="result">The result.</param>
        public virtual void Execute(IResult result)
        {
            ExecuteCore(this, null, new[] {result});
        }

        /// <summary>
        /// Executes the specified instances of <see cref="IResult"/> on the <see cref="IInteractionNode"/> 
        /// associated with the default view.
        /// </summary>
        /// <param name="results">The results to execute.</param>
        public virtual void Execute(IEnumerable<IResult> results)
        {
            ExecuteCore(this, null, results);
        }

        /// <summary>
        /// Executes the specified instances of <see cref="IResult"/> on the <see cref="IInteractionNode"/> 
        /// associated with the view tied to the context.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <param name="results">The results to execute.</param>
        public virtual void Execute(object viewContext, IEnumerable<IResult> results)
        {
            ExecuteCore(this, viewContext, results);
        }

        /// <summary>
        /// Executes the specified instances of <see cref="IResult"/> on the <see cref="IInteractionNode"/> 
        /// associated with the view tied to the context and <see cref="IMetadataContainer"/>.
        /// This delegate is provided in order to facilitate Unit Testing.  Set it to hijack
        /// the execution of results.
        /// </summary>
        public Action<IMetadataContainer, object, IEnumerable<IResult>> ExecuteCore =
            (presenter, viewContext, results) =>{
                var view = presenter.GetView<DependencyObject>(viewContext);

                IInteractionNode node = null;
                if(view != null)
                    node = view.GetValue(RoutedMessageController.NodeProperty) as IInteractionNode;

                new SequentialResult(results).Execute(null, node);
            };

        /// <summary>
        /// Determines whether this instance can shutdown.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can shutdown; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanShutdown()
        {
            return true;
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
        /// Determines if the specified presenter can be shut down.
        /// </summary>
        /// <param name="presenter">The presenter.</param>
        /// <param name="completed">Called when the shutdown action is finished.</param>
        protected virtual void CanShutdownPresenter(IPresenter presenter, Action<bool> completed)
        {
            var canShutdownPresenter = presenter.CanShutdown();

            if(!canShutdownPresenter)
            {
                var custom = presenter as ISupportCustomShutdown;

                if(custom != null)
                {
                    var model = custom.CreateShutdownModel();

                    if(model != null)
                    {
                        ExecuteShutdownModel(
                            model,
                            () => completed(custom.CanShutdown(model))
                            );

                        return;
                    }
                }
            }

            completed(canShutdownPresenter);
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

        /// <summary>
        /// Called when the presenter's view is loaded.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="context">The context.</param>
        public virtual void ViewLoaded(object view, object context) {}

        /// <summary>
        /// Closes this instance by asking its Parent to initiate shutdown or by asking it's corresponding default view to close.
        /// </summary>
        public virtual void Close()
        {
            if (Parent != null)
                Parent.Shutdown(this, delegate { });
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