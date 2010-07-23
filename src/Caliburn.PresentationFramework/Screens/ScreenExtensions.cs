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
        /// Opens the specified screen.
        /// </summary>
        /// <param name="conductor">The conductor.</param>
        /// <param name="subjectSpecification">The subject.</param>
        /// <param name="callback">Is called with true if the screen is activated.</param>
        public static void ActivateSubject(this IConductor conductor, ISubjectSpecification subjectSpecification, Action<bool> callback)
        {
            var found = conductor.GetConductedItems()
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