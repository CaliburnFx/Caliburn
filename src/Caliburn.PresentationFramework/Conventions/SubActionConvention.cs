namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using ViewModels;

    /// <summary>
    /// An implementation of <see cref="IViewConvention{T}"/> that matches sub-actions.
    /// </summary>
    public class SubActionConvention : ViewConventionBase<PropertyInfo>
    {
        /// <summary>
        /// Tries to creates the application of the convention.
        /// </summary>
        /// <param name="conventionManager">The convention manager.</param>
        /// <param name="description">The description.</param>
        /// <param name="element">The element.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        /// The convention application, or null if not applicable
        /// </returns>
        public override IViewApplicable TryCreateApplication(IConventionManager conventionManager, IViewModelDescription description, IElementDescription element, PropertyInfo target)
        {
            var path = DeterminePropertyPath(element.Name);
            var index = path.LastIndexOf(".");

            if (index == -1)
                return null;

            var subPath = path.Substring(0, index);

            var actualTarget = GetBoundProperty(target, subPath);
            if (actualTarget == null)
                return null;

            var actionName = path.Substring(index + 1);

            var subDescription = ViewModelDescriptionFactory.Create(actualTarget.PropertyType);
            var action = subDescription.Actions
                .FirstOrDefault(x => string.Compare(x.Name, actionName, StringComparison.CurrentCultureIgnoreCase) == 0);

            if (action == null)
                return null;

            var message = CreateActionMessage(action);

            return new ApplicableAction(
                element.Name,
                subPath,
                message
                );
        }
    }
}