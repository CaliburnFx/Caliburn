namespace Caliburn.PresentationFramework.Screens
{
    /// <summary>
    /// Denotes an object that can be closed.
    /// </summary>
    public interface IClose
    {
        /// <summary>
        /// Tries to close this instance.
        /// </summary>
        /// <returns><c>True</c> if the close operation was successfull, otherwise <c>false</c>.</returns>
        void TryClose();
    }
}