namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using Actions;
    using ViewModels;

    /// <summary>
    /// The default implementation of <see cref="IViewConvention{T}"/> for actions.
    /// </summary>
    public class DefaultActionConvention : ViewConventionBase<IAction>
    {
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

            return new ApplicableAction(element.Name, null, message);
        }
    }
}