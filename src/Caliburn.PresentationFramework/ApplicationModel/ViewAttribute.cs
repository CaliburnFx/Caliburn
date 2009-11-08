namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.Linq;
    using System.Windows;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// An implementation of <see cref="IViewStrategy"/> that provides a basic lookup strategy for an attributed model.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, AllowMultiple = true)]
    public class ViewAttribute : ViewStrategyAttribute
    {
        private readonly Type _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAttribute"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public ViewAttribute(Type key)
        {
            _key = key;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public Type Key
        {
            get { return _key; }
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public object Context { get; set; }

        /// <summary>
        /// Determines whether this strategy applies in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// true if it matches the context; false otherwise
        /// </returns>
        public override bool Matches(object context)
        {
            if(Context == null)
                return context == null;

            return Context.Equals(context);
        }

        /// <summary>
        /// Gets the view for displaying the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="displayLocation">The control into which the view will be injected.</param>
        /// <param name="context">Some additional context used to select the proper view.</param>
        /// <returns>The view.</returns>
        public override object GetView(object model, DependencyObject displayLocation, object context)
        {
            var instances = ServiceLocator.Current.GetAllInstances(_key);

            var view = instances.Count() > 0
                           ? instances.First()
                           : Activator.CreateInstance(_key);

            return view;
        }
    }
}