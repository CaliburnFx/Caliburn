namespace Caliburn.Core.InversionOfControl
{
    /// <summary>
    /// Implemented by services capable of injecting dependencies into existing objects.
    /// </summary>
    public interface IBuilder
    {
        /// <summary>
        /// Injects dependencies into the object.
        /// </summary>
        /// <param name="instance">The instance to build up.</param>
        void BuildUp(object instance);
    }
}