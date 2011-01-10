namespace Caliburn.PresentationFramework.Screens
{
    using System;
    using System.Linq;
    using ViewModels;

    /// <summary>
    /// Hosts extension methods for <see cref="IScreen"/> classes.
    /// </summary>
    public static class ScreenExtensions
    {
        static IViewModelFactory viewModelFactory;

        /// <summary>
        /// Initializes the extensions with the specified view model factory.
        /// </summary>
        /// <param name="viewModelFactory">The view model factory.</param>
        public static void Initialize(IViewModelFactory viewModelFactory)
        {
            ScreenExtensions.viewModelFactory = viewModelFactory;
        }

        /// <summary>
        /// Activates the item if it implements <see cref="IActivate"/>, otherwise does nothing.
        /// </summary>
        /// <param name="potentialActivatable">The potential activatable.</param>
        public static void TryActivate(object potentialActivatable)
        {
            var activator = potentialActivatable as IActivate;
            if (activator != null)
                activator.Activate();
        }

        /// <summary>
        /// Deactivates the item if it implements <see cref="IDeactivate"/>, otherwise does nothing.
        /// </summary>
        /// <param name="potentialDeactivatable">The potential deactivatable.</param>
        /// <param name="close">Indicates whether or not to close the item after deactivating it.</param>
        public static void TryDeactivate(object potentialDeactivatable, bool close)
        {
            var deactivator = potentialDeactivatable as IDeactivate;
            if (deactivator != null)
                deactivator.Deactivate(close);
        }

        /// <summary>
        /// Closes the specified item.
        /// </summary>
        /// <param name="conductor">The conductor.</param>
        /// <param name="item">The item to close.</param>
        public static void CloseItem(this IConductor conductor, object item)
        {
            conductor.DeactivateItem(item, true);
        }

        /// <summary>
        /// Closes the specified item.
        /// </summary>
        /// <param name="conductor">The conductor.</param>
        /// <param name="item">The item to close.</param>
        public static void CloseItem<T>(this ConductorBase<T> conductor, T item)
        {
            conductor.DeactivateItem(item, true);
        }

        /// <summary>
        /// Opens the specified screen.
        /// </summary>
        /// <param name="conductor">The conductor.</param>
        /// <param name="subjectSpecification">The subject.</param>
        /// <param name="callback">Is called with true if the screen is activated.</param>
        public static void ActivateSubject(this IConductor conductor, ISubjectSpecification subjectSpecification, Action<bool> callback)
        {
            var found = conductor.GetChildren()
                .OfType<IHaveSubject>()
                .FirstOrDefault(subjectSpecification.Matches);

            EventHandler<ActivationProcessedEventArgs> processed = null;
            processed = (s, e) =>{
                conductor.ActivationProcessed -= processed;
                callback(e.Success);
            };

            conductor.ActivationProcessed += processed;

            if(found != null)
                conductor.ActivateItem(found);
            else subjectSpecification.CreateSubjectHost(viewModelFactory, conductor.ActivateItem);
        }
    }
}