namespace Caliburn.PresentationFramework.Filters
{
    /// <summary>
    /// Manages filters for an object.
    /// </summary>
    public interface IFilterManager
    {
        /// <summary>
        /// Gets the trigger affecting filters.
        /// </summary>
        /// <value>The trigger effects.</value>
        IPreProcessor[] TriggerEffects { get; }

        /// <summary>
        /// 
        /// Gets the filters that execute before something else.
        /// </summary>
        /// <value>The pre execute.</value>
        IPreProcessor[] PreProcessors { get; }

        /// <summary>
        /// Gets the filters that execute after something else.
        /// </summary>
        /// <value>The post execute.</value>
        IPostProcessor[] PostProcessors { get; }

        /// <summary>
        /// Gets the instance aware filters.
        /// </summary>
        /// <value>The instance aware filters.</value>
        IHandlerAware[] HandlerAware { get; }

        /// <summary>
        /// Gets a filter that performs a resuce.
        /// </summary>
        /// <value>The rescue.</value>
        IRescue[] Rescues { get; }

        /// <summary>
        /// Adds the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        void Add(IFilter filter);

        /// <summary>
        /// Combines the filters from the specified manager with the current instance.
        /// </summary>
        /// <param name="filterManager">The filter manager.</param>
        /// <returns>A new filter manager representing the filter combinations.</returns>
        IFilterManager Combine(IFilterManager filterManager);
    }
}