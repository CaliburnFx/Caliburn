namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Core.Invocation;
    using Core.Metadata;
    using Filters;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// An implementation of <see cref="IActionHost"/>.
    /// </summary>
    public class ActionHost : MetadataContainer, IActionHost
    {
        private readonly Dictionary<string, IAction> _actions = new Dictionary<string, IAction>();
        private readonly Type _targetType;
        private readonly IServiceLocator _serviceLocator;
        private readonly IFilterManager _filters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionHost"/> class.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="serviceLocator">The container</param>
        public ActionHost(Type targetType, IActionFactory factory, IServiceLocator serviceLocator)
        {
            _targetType = targetType;
            _serviceLocator = serviceLocator;

            AddMetadataFrom(targetType);

            _filters = new FilterManager(_targetType, this, serviceLocator);

            foreach(var action in factory.CreateFor(this))
            {
                _actions[action.Name] = action;
            }
        }

        /// <summary>
        /// Gets the type of the target.
        /// </summary>
        /// <value>The type of the target.</value>
        public Type TargetType
        {
            get { return _targetType; }
        }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>The filters.</value>
        public IFilterManager Filters
        {
            get { return _filters; }
        }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <param name="message">The action message.</param>
        /// <returns></returns>
        public IAction GetAction(ActionMessage message)
        {
            IAction found;
            _actions.TryGetValue(message.MethodName, out found);
            return found;
        }

        /// <summary>
        /// Gets the filter manager for a given method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public IFilterManager GetFilterManager(IMethod method)
        {
            return new FilterManager(TargetType, method, _serviceLocator).Combine(Filters);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IAction> GetEnumerator()
        {
            return _actions.Values.GetEnumerator();
        }
    }
}