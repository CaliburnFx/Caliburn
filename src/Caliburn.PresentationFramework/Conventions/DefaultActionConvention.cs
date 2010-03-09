namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using Actions;
    using Core.Logging;
    using ViewModels;
    using Views;

    /// <summary>
    /// The default implementation of <see cref="IViewConvention{T}"/> for actions.
    /// </summary>
    public class DefaultActionConvention : ViewConventionBase<IAction>
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(DefaultActionConvention));

        /// <summary>
        /// Creates the application of the convention.
        /// </summary>
        /// <param name="conventionManager">The convention manager.</param>
        /// <param name="description">The description.</param>
        /// <param name="element">The element.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public override IViewApplicable TryCreateApplication(IConventionManager conventionManager, IViewModelDescription description, IElementDescription element, IAction action)
        {
            if (string.Compare(element.Name, action.Name, StringComparison.CurrentCultureIgnoreCase) != 0)
                return null;

            var message = CreateActionMessage(action);

            Log.Info("Action convention matched for {0}.", element.Name);
            return new ApplicableAction(element.Name, null, message);
        }
    }
}