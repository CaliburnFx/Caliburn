namespace Caliburn.PresentationFramework.Actions
{
    using Core.Invocation;
    using Filters;

    /// <summary>
    /// Hosts extension methods related to actions.
    /// </summary>
    public static class ActionExtensions
    {
        /// <summary>
        /// Creates the filter manager for the method based on the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static IFilterManager CreateFilterManager(this ActionCreationContext context, IMethod method)
        {
            return new FilterManager(context.TargetType, method.Info, context.ServiceLocator).Combine(context.TargetFilters);
        }
    }
}