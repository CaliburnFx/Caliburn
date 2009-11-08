namespace Caliburn.PresentationFramework.ApplicationModel
{
    /// <summary>
    /// Instances that implement this interface support custom shutdown logic through a developer defined model.
    /// </summary>
    public interface ISupportCustomShutdown
    {
        /// <summary>
        /// Creates the shutdown model.
        /// </summary>
        /// <returns></returns>
        ISubordinate CreateShutdownModel();

        /// <summary>
        /// Determines whether this instance can shutdown based on the evaluated shutdown model.
        /// </summary>
        /// <param name="shutdownModel">The shutdown model.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can shutdown; otherwise, <c>false</c>.
        /// </returns>
        bool CanShutdown(ISubordinate shutdownModel);
    }
}