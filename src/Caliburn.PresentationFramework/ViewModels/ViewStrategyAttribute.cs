namespace Caliburn.PresentationFramework.ViewModels
{
    using System;
    using System.Windows;
    using Core.Metadata;

    /// <summary>
    /// An attribute base class that implements <see cref="IViewLocator"/>.
    /// </summary>
    public abstract class ViewStrategyAttribute : Attribute, IViewLocator, IMetadata
    {
        /// <summary>
        /// Determines whether this strategy applies in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>true if it matches the context; false otherwise</returns>
        public abstract bool Matches(object context);

        /// <summary>
        /// Gets the view for displaying the specified viewModel.
        /// </summary>
        /// <param name="viewModel">The model.</param>
        /// <param name="displayLocation">The control into which the view will be injected.</param>
        /// <param name="context">Some additional context used to select the proper view.</param>
        /// <returns>The view.</returns>
        public abstract DependencyObject Locate(object viewModel, DependencyObject displayLocation, object context);
    }
}