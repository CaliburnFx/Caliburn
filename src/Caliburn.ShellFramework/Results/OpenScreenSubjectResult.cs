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
        private readonly ISubjectSpecification subjectSpecification;
        private Func<ResultExecutionContext, IConductor> locateParent;

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
            if (locateParent == null)
                locateParent = c => (IConductor)c.HandlingNode.MessageHandler.Unwrap();

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
                    if(deactivator != null)
                    {
                        deactivator.Deactivated += (s, e) =>{
                            if(!e.WasClosed)
                                return;

                            if(onClose != null)
                                onClose(child);

                            OnCompleted(null, false);
                        };
                    }
                    else OnCompleted(null, false);
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