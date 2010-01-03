namespace Caliburn.PresentationFramework.ViewModels
{
    using System;
    using System.Windows;
    using Core;
    using Core.Metadata;
    using Metadata;
    using Action=Actions.Action;

    /// <summary>
    /// The default implementation of <see cref="IViewModelBinder"/>.
    /// </summary>
    public class DefaultViewModelBinder : IViewModelBinder
    {
        private readonly IViewModelDescriptionFactory _viewModelDescriptionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewModelBinder"/> class.
        /// </summary>
        /// <param name="viewModelDescriptionFactory"></param>
        public DefaultViewModelBinder(IViewModelDescriptionFactory viewModelDescriptionFactory)
        {
            _viewModelDescriptionFactory = viewModelDescriptionFactory;
        }

        /// <summary>
        /// Binds the specified viewModel to the view.
        /// </summary>
        /// <param name="viewModel">The model.</param>
        /// <param name="view">The view.</param>
        /// <param name="context">The context.</param>
        public void Bind(object viewModel, DependencyObject view, object context)
        {
            BindCore(viewModel, view, context);
            ApplyConventions(viewModel, view);
        }

        /// <summary>
        /// Attaches the view model and the view.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="view">The view.</param>
        /// <param name="context">The context.</param>
        protected virtual void BindCore(object viewModel, DependencyObject view, object context)
        {
            Action.SetTarget(view, viewModel);

            var metadataContainer = viewModel as IMetadataContainer;
            if (metadataContainer != null) 
                metadataContainer.SetView(view, context, false);

            var viewAware = viewModel as IViewAware;
            if (viewAware != null)
                view.OnLoad(delegate { viewAware.ViewLoaded(view, context); });
        }

        protected virtual void ApplyConventions(object viewModel, DependencyObject view)
        {
            var modelType = GetModelType(viewModel);
            var description = _viewModelDescriptionFactory.Create(modelType);

            description.GetConventionsFor(view)
                .Apply(x => x.ApplyTo(view));
        }

        /// <summary>
        /// Gets the type of the view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        protected virtual Type GetModelType(object viewModel)
        {
            return viewModel.GetType();
        }
    }
}