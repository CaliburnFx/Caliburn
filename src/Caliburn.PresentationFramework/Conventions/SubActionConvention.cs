namespace Caliburn.PresentationFramework.Conventions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Core.Logging;
    using ViewModels;
    using Views;

    /// <summary>
    /// An implementation of <see cref="IViewConvention{T}"/> that matches sub-actions.
    /// </summary>
    public class SubActionConvention : ViewConventionBase<PropertyInfo>
    {
        static readonly ILog Log = LogManager.GetLog(typeof(SubActionConvention));

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
        public override IViewApplicable TryCreateApplication(IConventionManager conventionManager, IViewModelDescription description, ElementDescription element, PropertyInfo target)
        {
            var exptectedPath = DeterminePropertyPath(element.Name);
            var index = exptectedPath.LastIndexOf(".");

            if (index == -1)
                return null;

            var propertyPath = exptectedPath.Substring(0, index);
            string correctedPath;
            var actualTarget = GetBoundProperty(target, propertyPath, out correctedPath);
            if (actualTarget == null)
                return null;

            var actionName = exptectedPath.Substring(index + 1);
            var subDescription = ViewModelDescriptionFactory.Create(actualTarget.PropertyType);
            var action = subDescription.Actions.FirstOrDefault(x => string.Compare(x.Name, actionName, StringComparison.CurrentCultureIgnoreCase) == 0);

            if (action == null)
                return null;

            var message = CreateActionMessage(action);

            Log.Info("Sub action convention matched for {0} on {1}.", element.Name, correctedPath);

            return new ApplicableAction(
                element.Name,
                correctedPath,
                message
                );
        }
    }
}