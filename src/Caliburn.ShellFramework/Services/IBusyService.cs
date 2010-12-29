namespace Caliburn.ShellFramework.Services
{
    /// <summary>
    /// Implemented by services that track the busy status of a view model.
    /// </summary>
    public interface IBusyService
    {
        /// <summary>
        /// Marks a view model as busy.
        /// </summary>
        /// <param name="sourceViewModel">The source view model.</param>
        /// <param name="busyViewModel">The busy view model.</param>
        void MarkAsBusy(object sourceViewModel, object busyViewModel);

        /// <summary>
        /// Marks a view model as not busy.
        /// </summary>
        /// <param name="sourceViewModel">The source view model.</param>
        void MarkAsNotBusy(object sourceViewModel);
    }
}