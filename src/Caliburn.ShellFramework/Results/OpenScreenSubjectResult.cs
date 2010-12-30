namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Linq;
    using Core.InversionOfControl;
    using PresentationFramework.RoutedMessaging;
    using PresentationFramework.Screens;
    using PresentationFramework.ViewModels;

    /// <summary>
    /// An <see cref="IResult"/> for showing screens based on an <see cref="ISubjectSpecification"/>.
    /// </summary>
    public class OpenScreenSubjectResult : OpenResultBase<object>
    {
        readonly ISubjectSpecification subjectSpecification;

        Func<ResultExecutionContext, IConductor> locateParent =
            c => (IConductor)c.HandlingNode.MessageHandler.Unwrap();

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenScreenSubjectResult"/> class.
        /// </summary>
        /// <param name="subjectSpecification">The subject specification.</param>
        public OpenScreenSubjectResult(ISubjectSpecification subjectSpecification)
        {
            this.subjectSpecification = subjectSpecification;
        }

        /// <summary>
        /// Declares the parent to open the child in.
        /// </summary>
        /// <typeparam name="TParent">The type of the parent.</typeparam>
        /// <returns></returns>
        public OpenScreenSubjectResult In<TParent>()
            where TParent : IConductor
        {
            locateParent = c => c.ServiceLocator.GetInstance<IViewModelFactory>().Create<TParent>();
            return this;
        }

        /// <summary>
        /// Declares the parent to open the child in.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        public OpenScreenSubjectResult In(IConductor parent)
        {
            locateParent = c => parent;
            return this;
        }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Execute(ResultExecutionContext context)
        {
            var parent = locateParent(context);

            parent.ActivateSubject(subjectSpecification, success =>{
                if(success)
                {
                    var child = parent
                        .GetConductedItems()
                        .OfType<IHaveSubject>()
                        .FirstOrDefault(subjectSpecification.Matches);

                    if(onConfigure != null)
                        onConfigure(child);

                    OnOpened(parent, child);

                    var deactivator = child as IDeactivate;
                    if (deactivator != null && onClose != null)
                    {
                        EventHandler<DeactivationEventArgs> handler = null;
                        handler = (s2, e2) =>
                        {
                            if (e2.WasClosed)
                            {
                                deactivator.Deactivated -= handler;
                                onClose(child);
                            }
                        };

                        deactivator.Deactivated += handler;
                    }

                    OnCompleted(null, false);
                }
                else OnCompleted(null, true);
            });
        }

        /// <summary>
        /// Called when the child is opened.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="child">The child.</param>
        protected virtual void OnOpened(IConductor parent, object child) { }
    }
}