namespace Caliburn.PresentationFramework.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Core;
    using Core.InversionOfControl;

    /// <summary>
    /// An implementation of <see cref="IFilterManager"/>.
    /// </summary>
    public class FilterManager : IFilterManager
    {
        readonly Type targetType;
        readonly MemberInfo memberInfo;
        readonly IServiceLocator serviceLocator;

        IPreProcessor[] preExecute;
        IPreProcessor[] triggerEffects;
        IPostProcessor[] postExecute;
        IHandlerAware[] handlerAware;
        IRescue[] rescues;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterManager"/> class.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="member">The member.</param>
        /// <param name="serviceLocator">The serviceLocator.</param>
        public FilterManager(Type targetType, MemberInfo member, IServiceLocator serviceLocator)
        {
            this.targetType = targetType;
            memberInfo = member;
            this.serviceLocator = serviceLocator;

            var filters = member.GetAttributes<IFilter>(true)
                .OrderByDescending(x => x.Priority);

            filters.OfType<IInitializable>()
                .Apply(x => x.Initialize(targetType, member, serviceLocator));

            handlerAware = filters.OfType<IHandlerAware>().ToArray();

            preExecute = filters.OfType<IPreProcessor>().ToArray();

            triggerEffects =
                (from filter in preExecute
                 where filter.AffectsTriggers
                 select filter).ToArray();

            postExecute = filters.OfType<IPostProcessor>().ToArray();

            rescues = filters.OfType<IRescue>().ToArray();
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
        /// <param name="member">The member.</param>
        private FilterManager(Type targetType, MemberInfo member, IServiceLocator serviceLocator,
            IEnumerable<IPreProcessor> preExecute, IEnumerable<IPreProcessor> triggerEffects,
            IEnumerable<IPostProcessor> postExecute, IEnumerable<IHandlerAware> instanceAwareFilters, IEnumerable<IRescue> rescues)
        {
            this.targetType = targetType;
            memberInfo = member;
            this.serviceLocator = serviceLocator;

            this.preExecute = preExecute.ToArray();
            this.triggerEffects = triggerEffects.ToArray();
            this.postExecute = postExecute.ToArray();
            handlerAware = instanceAwareFilters.ToArray();
            this.rescues = rescues.ToArray();
        }

        /// <summary>
        /// Gets the filters that execute before something else.
        /// </summary>
        /// <value>The pre execute.</value>
        public IEnumerable<IPreProcessor> PreProcessors
        {
            get { return preExecute; }
        }

        /// <summary>
        /// Gets the trigger affecting filters.
        /// </summary>
        /// <value>The trigger effects.</value>
        public IEnumerable<IPreProcessor> TriggerEffects
        {
            get { return triggerEffects; }
        }

        /// <summary>
        /// Gets the filters that execute after something else.
        /// </summary>
        /// <value>The post execute.</value>
        public IEnumerable<IPostProcessor> PostProcessors
        {
            get { return postExecute; }
        }

        /// <summary>
        /// Gets the instance aware filters.
        /// </summary>
        /// <value>The instance aware filters.</value>
        public IEnumerable<IHandlerAware> HandlerAware
        {
            get { return handlerAware; }
        }

        /// <summary>
        /// Gets a filter that performs a resuce.
        /// </summary>
        /// <value>The rescue.</value>
        public IEnumerable<IRescue> Rescues
        {
            get { return rescues; }
        }

        /// <summary>
        /// Adds the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public void Add(IFilter filter)
        {
            var initializable = filter as IInitializable;
            if (initializable != null)
                initializable.Initialize(targetType, memberInfo, serviceLocator);

            TryAdd(ref preExecute, filter);
            TryAdd(ref postExecute, filter);
            TryAdd(ref handlerAware, filter);
            TryAdd(ref rescues, filter);

            var pre = filter as IPreProcessor;
            if(pre == null) return;

            if(pre.AffectsTriggers)
                TryAdd(ref triggerEffects, filter);
        }

        static void TryAdd<T>(ref T[] array, IFilter filter)
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
                targetType,
                memberInfo,
                serviceLocator,
                preExecute.Union(filterManager.PreProcessors),
                triggerEffects.Union(filterManager.TriggerEffects),
                postExecute.Union(filterManager.PostProcessors),
                handlerAware.Union(filterManager.HandlerAware),
                rescues.Union(filterManager.Rescues)
                );

            return newManager;
        }
    }
}