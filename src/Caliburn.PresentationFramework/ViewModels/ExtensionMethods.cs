namespace Caliburn.PresentationFramework.ViewModels
{
    using System;
    using System.Windows;
    using Core.Metadata;
    using Metadata;

    /// <summary>
    /// Hosts extension methods related to view model functionality.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// The overridable implementation of Locate.
        /// </summary>
        public static Func<IViewLocator, object, DependencyObject, object, DependencyObject> LocateImplementation = DefaultLocateImplementation;

        /// <summary>
        /// Gets the view for displaying the specified model.
        /// </summary>
        /// <param name="locator">The view locator.</param>
        /// <param name="model">The model.</param>
        /// <param name="displayLocation">The control into which the view will be injected.</param>
        /// <param name="context">Some additional context used to select the proper view.</param>
        /// <returns></returns>
        public static DependencyObject Locate(this IViewLocator locator, object model, DependencyObject displayLocation, object context)
        {
            return LocateImplementation(locator, model, displayLocation, context);
        }

        private static DependencyObject DefaultLocateImplementation(IViewLocator locator, object model, DependencyObject displayLocation, object context)
        {
            if (model == null)
                return null;

#if !SILVERLIGHT
            var metadataContainer = model as IMetadataContainer;
            if (metadataContainer != null)
            {
                var view = metadataContainer.GetView<DependencyObject>(context);
                if (view != null)
                {
                    var windowCheck = view as Window;
                    if (windowCheck == null || !windowCheck.IsLoaded)
                    {
                        return view;
                    }
                }
            }
#endif

            return locator.Locate(model.GetModelType(), displayLocation, context);
        }
    }
}