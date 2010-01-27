namespace Caliburn.PresentationFramework.ViewModels
{
    using System;
    using System.Windows;
    using ApplicationModel;
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
        private bool _applyConventionsByDefault = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewModelBinder"/> class.
        /// </summary>
        /// <param name="viewModelDescriptionFactory"></param>
        public DefaultViewModelBinder(IViewModelDescriptionFactory viewModelDescriptionFactory)
        {
            _viewModelDescriptionFactory = viewModelDescriptionFactory;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to apply conventions by default.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if conventions should be applied by default; otherwise, <c>false</c>.
        /// </value>
        public bool ApplyConventionsByDefault
        {
            get { return _applyConventionsByDefault; }
            set { _applyConventionsByDefault = value; }
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

#if !SILVERLIGHT || SILVERLIGHT_30 || SILVERLIGHT_40
            var significantView = DefaultWindowManager.GetSignificantView(view);
#else
            var significantView = view;
#endif

            if (ShouldApplyConventions(viewModel, significantView, context))
                ApplyConventions(viewModel, significantView);
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

        /// <summary>
        /// Indicates whether or not the conventions should be applied.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="view">The view.</param>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if conventions should be applied; otherwise, <c>false</c></returns>
        protected virtual bool ShouldApplyConventions(object viewModel, DependencyObject view, object context)
        {
            var overriden = View.GetApplyConventions(view);
            return overriden.GetValueOrDefault(_applyConventionsByDefault);
        }

        /// <summary>
        /// Applies the conventions.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="view">The view.</param>
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