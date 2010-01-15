namespace Caliburn.PresentationFramework.Behaviors
{
    /// <summary>
    /// Inidicates how dependency tracking should occur.
    /// </summary>
    public enum DependencyMode
    {
        /// <summary>
        /// Will record dependencies on every get.
        /// </summary>
        AlwaysRecord = 0,
        /// <summary>
        /// Will record dependencies on the first get.
        /// </summary>
        RecordOnce = 1,
        /// <summary>
        /// Will not record dependencies.
        /// </summary>
        DoNotRecord = 2
    }
}