namespace Caliburn.PresentationFramework
{
    using System;
    using Configuration;
    using Core.Behaviors;
    using Core.Configuration;

    /// <summary>
    /// Extension methods related to the PresentationFramework classes.
    /// </summary>
    public static class PresentationFrameworkExtensions
    {
        /// <summary>
        /// The overridable implemenation of GetModelType.
        /// </summary>
        public static Func<object, Type> GetModelTypeImplementation = DefaultGetModelTypeImplemenation;

        /// <summary>
        /// Gets the type of the model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Type GetModelType(this object model)
        {
            return GetModelTypeImplementation(model);
        }

        private static Type DefaultGetModelTypeImplemenation(this object model)
        {
            var proxy = model as IProxy;
            return proxy != null ? proxy.OriginalType : model.GetType();
        }

        /// <summary>
        /// Adds the presentation framework module's configuration to the system.
        /// </summary>
        /// <param name="hook">The hook.</param>
        public static PresentationFrameworkConfiguration PresentationFramework(this IModuleHook hook)
        {
            return hook.Module(CaliburnModule<PresentationFrameworkConfiguration>.Instance);
        }

        /// <summary>
        /// Safely converts an object to a string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted string or null, if the value was null.</returns>
        internal static string SafeToString(this object value)
        {
            return value == null ? null : value.ToString();
        }
    }
}