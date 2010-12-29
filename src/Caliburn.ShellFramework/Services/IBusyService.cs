namespace Caliburn.ShellFramework.Services
{
    /// <summary>
    /// Implemented by services that track the busy status of a view model.
    /// </summary>
    public interface IBusyService
    {
        /// <summary>
        /// Marks a ViewModel as busy.
        /// </summary>
        /// <param name="sourceViewModel">The ViewModel to mark as busy.</param>
        /// <param name="busyViewModel">The busy content ViewModel.</param>
        void MarkAsBusy(object sourceViewModel, object busyViewModel);

        /// <summary>
        /// Marks a ViewModel as not busy.
        /// </summary>
        /// <param name="sourceViewModel">The ViewModel to mark as not busy.</param>
        void MarkAsNotBusy(object sourceViewModel);
    }
}