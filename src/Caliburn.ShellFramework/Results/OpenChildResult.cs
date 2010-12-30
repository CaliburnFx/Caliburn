namespace Caliburn.ShellFramework.Results
{
    using System;
    using Core.InversionOfControl;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;

    /// <summary>
    /// Opend a child view model in a <see cref="IConductor"/>.
    /// </summary>
    /// <typeparam name="TChild">The type of the child.</typeparam>
    public class OpenChildResult<TChild> : OpenResultBase<TChild>
    {
        Func<ResultExecutionContext, IConductor> locateParent =
            c => (IConductor)c.HandlingNode.MessageHandler.Unwrap();

        readonly Func<ResultExecutionContext, TChild> locateChild =
            c => c.ServiceLocator.GetInstance<IViewModelFactory>().Create<TChild>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenChildResult&lt;TChild&gt;"/> class.
        /// </summary>
        public OpenChildResult() { }

        /// <summary>
        /// Initializes a new instance with the child to open.
        /// </summary>
        /// <param name="child">The child.</param>
        public OpenChildResult(TChild child)
        {
            locateChild = c => child;
        }

        /// <summary>
        /// Declares the parent to open the child in.
        /// </summary>
        /// <typeparam name="TParent">The type of the parent.</typeparam>
        /// <returns>Itself.</returns>
        public OpenChildResult<TChild> In<TParent>()
            where TParent : IConductor
        {
            locateParent = c => c.ServiceLocator.GetInstance<IViewModelFactory>().Create<TParent>();
            return this;
        }

        /// <summary>
        /// Declares the parent to open the child in.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns>Itself</returns>
        public OpenChildResult<TChild> In(IConductor parent)
        {
            locateParent = c => parent;
            return this;
        }

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Execute(ResultExecutionContext context)
        {
            var parent = locateParent(context);
            var child = locateChild(context);

            if (onConfigure != null)
                onConfigure(child);

            EventHandler<ActivationProcessedEventArgs> processed = null;
            processed = (s, e) =>{
                parent.ActivationProcessed -= processed;

                if(e.Success)
                {
                    OnOpened(parent, child);

                    var deactivator = child as IDeactivate;
                    if (deactivator != null && onClose != null)
                    {
                        EventHandler<DeactivationEventArgs> handler = null;
                        handler = (s2, e2) =>{
                            if(!e2.WasClosed)
                                return;

                            deactivator.Deactivated -= handler;
                            onClose(child);
                        };

                        deactivator.Deactivated += handler;
                    }

                    OnCompleted(null, false);
                }
                else OnCompleted(null, true);
            };

            parent.ActivationProcessed += processed;
            parent.ActivateItem(child);
        }

        /// <summary>
        /// Called when [opened].
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="child">The child.</param>
        protected virtual void OnOpened(IConductor parent, TChild child) { }
    }
}