namespace Caliburn.PresentationFramework.Commands
{
    /// <summary>
    /// Indicates locations where a command can be found.
    /// </summary>
    public enum CommandSource
    {
        /// <summary>
        /// Looks for the command in the resources collection.
        /// </summary>
        Resource,
        /// <summary>
        /// Looks for the command in the DI container.
        /// </summary>
        Container,
        /// <summary>
        /// Databinds to a command using a property path.
        /// </summary>
        Bound 
    }
}