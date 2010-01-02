namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.Windows;
    using Core;
    using Core.Metadata;
    using Metadata;
    using ViewModels;
    using Action=Actions.Action;

    /// <summary>
    /// The default implementation of <see cref="IBinder"/>.
    /// </summary>
    public class DefaultBinder : IBinder
    {
        private readonly IViewModelDescriptionFactory _viewModelDescriptionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBinder"/> class.
        /// </summary>
        /// <param name="viewModelDescriptionFactory"></param>
        public DefaultBinder(IViewModelDescriptionFactory viewModelDescriptionFactory)
        {
            _viewModelDescriptionFactory = viewModelDescriptionFactory;
        }

        /// <summary>
        /// Binds the specified model to the view.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="view">The view.</param>
        /// <param name="context">The context.</param>
        public void Bind(object model, DependencyObject view, object context)
        {
            BindCore(model, view, context);
            ApplyConventions(model, view);
        }

        /// <summary>
        /// Attaches the model and the view.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="view">The view.</param>
        /// <param name="context">The context.</param>
        protected virtual void BindCore(object model, object view, object context)
        {
            Action.SetTarget(view as DependencyObject, model);

            var metadataContainer = model as IMetadataContainer;
            if (metadataContainer != null) metadataContainer.SetView(view, context, false);

            var viewAware = model as IViewAware;
            if (viewAware != null)
            {
                var element = view as FrameworkElement;
                if (element != null)
                {
                    element.Loaded += delegate
                    {
                        viewAware.ViewLoaded(element, context);
                    };
                }
#if !SILVERLIGHT
                else
                {
                    var contentElement = view as FrameworkContentElement;
                    if (contentElement != null)
                    {
                        contentElement.Loaded += delegate
                        {
                            viewAware.ViewLoaded(contentElement, context);
                        };
                    }
                }
#endif
            }
        }

        protected virtual void ApplyConventions(object model, DependencyObject view)
        {
            var modelType = GetModelType(model);
            var description = _viewModelDescriptionFactory.Create(modelType);

            description.GetConventionsFor(view)
                .Apply(x => x.ApplyTo(view));
        }

        /// <summary>
        /// Gets the type of the model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        protected virtual Type GetModelType(object model)
        {
            return model.GetType();
        }
    }
}