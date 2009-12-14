namespace Caliburn.PresentationFramework.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Core.Metadata;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// An implementation of <see cref="IFilterManager"/>.
    /// </summary>
    public class FilterManager : IFilterManager
    {
        private readonly Type _targetType;
        private readonly IMetadataContainer _metadataContainer;
        private readonly IServiceLocator _serviceLocator;

        private IPreProcessor[] _preExecute;
        private IPreProcessor[] _triggerEffects;
        private IPostProcessor[] _postExecute;
        private IHandlerAware[] _handlerAware;
        private IRescue[] _rescues;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterManager"/> class.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="metadataContainer">The metadata container.</param>
        /// <param name="serviceLocator">The serviceLocator.</param>
        public FilterManager(Type targetType, IMetadataContainer metadataContainer, IServiceLocator serviceLocator)
        {
            _targetType = targetType;
            _metadataContainer = metadataContainer;
            _serviceLocator = serviceLocator;

            var filters = metadataContainer.FindMetadata<IFilter>()
                .OrderByDescending(x => x.Priority);

            filters.OfType<IInitializable>()
                .Apply(x => x.Initialize(targetType, metadataContainer, serviceLocator));

            _handlerAware = filters.OfType<IHandlerAware>().ToArray();

            _preExecute = filters.OfType<IPreProcessor>().ToArray();

            _triggerEffects =
                (from filter in _preExecute
                 where filter.AffectsTriggers
                 select filter).ToArray();

            _postExecute = filters.OfType<IPostProcessor>().ToArray();

            _rescues = filters.OfType<IRescue>().ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterManager"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="preExecute">The pre execute.</param>
        /// <param name="triggerEffects">The trigger effects.</param>
        /// <param name="postExecute">The post execute.</param>
        /// <param name="instanceAwareFilters">The instance aware.</param>
        /// <param name="rescues">The rescues</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="metadataContainer">The metadatacontainer.</param>
        private FilterManager(Type targetType, IMetadataContainer metadataContainer, IServiceLocator serviceLocator,
            IEnumerable<IPreProcessor> preExecute, IEnumerable<IPreProcessor> triggerEffects,
            IEnumerable<IPostProcessor> postExecute, IEnumerable<IHandlerAware> instanceAwareFilters, IEnumerable<IRescue> rescues)
        {
            _targetType = targetType;
            _metadataContainer = metadataContainer;
            _serviceLocator = serviceLocator;

            _preExecute = preExecute.ToArray();
            _triggerEffects = triggerEffects.ToArray();
            _postExecute = postExecute.ToArray();
            _handlerAware = instanceAwareFilters.ToArray();
            _rescues = rescues.ToArray();
        }

        /// <summary>
        /// Gets the filters that execute before something else.
        /// </summary>
        /// <value>The pre execute.</value>
        public IPreProcessor[] PreProcessors
        {
            get { return _preExecute; }
        }

        /// <summary>
        /// Gets the trigger affecting filters.
        /// </summary>
        /// <value>The trigger effects.</value>
        public IPreProcessor[] TriggerEffects
        {
            get { return _triggerEffects; }
        }

        /// <summary>
        /// Gets the filters that execute after something else.
        /// </summary>
        /// <value>The post execute.</value>
        public IPostProcessor[] PostProcessors
        {
            get { return _postExecute; }
        }

        /// <summary>
        /// Gets the instance aware filters.
        /// </summary>
        /// <value>The instance aware filters.</value>
        public IHandlerAware[] HandlerAware
        {
            get { return _handlerAware; }
        }

        /// <summary>
        /// Gets a filter that performs a resuce.
        /// </summary>
        /// <value>The rescue.</value>
        public IRescue[] Rescues
        {
            get { return _rescues; }
        }

        /// <summary>
        /// Adds the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public void Add(IFilter filter)
        {
            var initializable = filter as IInitializable;
            if (initializable != null)
                initializable.Initialize(_targetType, _metadataContainer, _serviceLocator);

            TryAdd(ref _preExecute, filter);
            TryAdd(ref _postExecute, filter);
            TryAdd(ref _handlerAware, filter);
            TryAdd(ref _rescues, filter);

            var preExecute = filter as IPreProcessor;
            if(preExecute == null) return;

            if(preExecute.AffectsTriggers)
                TryAdd(ref _triggerEffects, filter);
        }

        private static void TryAdd<T>(ref T[] array, IFilter filter)
        {
            if(!(filter is T)) return;

            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = (T)filter;
        }

        /// <summary>
        /// Combines the filters from the specified manager with the current instance.
        /// </summary>
        /// <param name="filterManager">The filter manager.</param>
        /// <returns>A new filter manager representing the filter combinations.</returns>
        public IFilterManager Combine(IFilterManager filterManager)
        {
            if(filterManager == null) return this;

            var newManager = new FilterManager(
                _targetType,
                _metadataContainer,
                _serviceLocator,
                _preExecute.Union(filterManager.PreProcessors),
                _triggerEffects.Union(filterManager.TriggerEffects),
                _postExecute.Union(filterManager.PostProcessors),
                _handlerAware.Union(filterManager.HandlerAware),
                _rescues.Union(filterManager.Rescues)
                );

            return newManager;
        }
    }
}