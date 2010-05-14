namespace Caliburn.Reactive
{
    using Core.Configuration;

    /// <summary>
    /// Extensions methods related to the Caliburn's Reactive Framework module.
    /// </summary>
    public static class ReactiveExtensions
    {
        /// <summary>
        /// Adds the Reactive Framework module's configuration to the system.
        /// </summary>
        /// <param name="hook">The hook.</param>
        public static ReactiveFrameworkConfiguration ReactiveFramework(this IModuleHook hook)
        {
            return hook.Module(CaliburnModule<ReactiveFrameworkConfiguration>.Instance);
        }
    }
}