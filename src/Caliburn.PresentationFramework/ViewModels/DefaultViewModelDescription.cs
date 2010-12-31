namespace Caliburn.PresentationFramework.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using Actions;
    using Conventions;
    using Core.Logging;
    using Filters;
    using Views;

    /// <summary>
    /// The default implementation of <see cref="IViewModelDescription"/>.
    /// </summary>
    public class DefaultViewModelDescription : IViewModelDescription
    {
        static readonly ILog Log = LogManager.GetLog(typeof(DefaultViewModelDescription));

        readonly IConventionManager conventionManager;
        readonly Type targetType;
        readonly Dictionary<string, IAction> actions = new Dictionary<string, IAction>();
        readonly Dictionary<Type, IViewApplicable[]> viewConventions = new Dictionary<Type, IViewApplicable[]>();
        readonly PropertyInfo[] properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewModelDescription"/> class.
        /// </summary>
        /// <param name="conventionManager">The convention manager.</param>
        /// <param name="targetType">Type of the target.</param>
        public DefaultViewModelDescription(IConventionManager conventionManager, Type targetType)
        {
            this.conventionManager = conventionManager;
            this.targetType = targetType;
            properties = targetType.GetProperties();
        }

        /// <summary>
        /// Gets the View Model's type.
        /// </summary>
        /// <value>The type of the View Model.</value>
        public Type TargetType
        {
            get { return targetType; }
        }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>The filters.</value>
        public IFilterManager Filters { get; set; }

        /// <summary>
        /// Gets the actions.
        /// </summary>
        /// <value>The actions.</value>
        public IEnumerable<IAction> Actions
        {
            get { return actions.Values; }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        public IEnumerable<PropertyInfo> Properties
        {
            get { return properties; }
        }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public IAction GetAction(ActionMessage message)
        {
            IAction found;
            actions.TryGetValue(message.MethodName, out found);
            return found;
        }

        /// <summary>
        /// Adds the action.
        /// </summary>
        /// <param name="action">The action.</param>
        public void AddAction(IAction action)
        {
            actions[action.Name] = action;
        }

        /// <summary>
        /// Sets the conventions for a particualr view type.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="applicableConventions">The applicable conventions.</param>
        public void SetConventionsFor(Type viewType, IEnumerable<IViewApplicable> applicableConventions)
        {
            viewConventions[viewType] = applicableConventions.ToArray();
        }

        /// <summary>
        /// Gets the conventions for the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The applicable conventions.</returns>
        public IEnumerable<IViewApplicable> GetConventionsFor(DependencyObject view)
        {
            IViewApplicable[] conventions;

            var viewType = view.GetType();
            var useConventionCache = ShouldUseConventionCache(view);

            if (useConventionCache)
            {
                if(!viewConventions.TryGetValue(viewType, out conventions))
                {
                    conventions = conventionManager.DetermineConventions(this, view).ToArray();
                    viewConventions[viewType] = conventions;
                    Log.Info("Cached conventions for {0}.", view);
                }
                else Log.Info("Using convention cache for {0}.", view);
            }
            else
            {
                conventions = conventionManager.DetermineConventions(this, view).ToArray();
                Log.Info("Ignoring convention cache for {0}.", view);
            }

            return conventions;
        }

        protected virtual bool ShouldUseConventionCache(DependencyObject view)
        {
#if !SILVERLIGHT
            return view is UserControl || view is Window;
#else
            return view is UserControl || view is ChildWindow;
#endif
        }
    }
}