namespace Caliburn.PresentationFramework.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Actions;
    using Core.Metadata;
    using Filters;

    /// <summary>
    /// The default implementation of <see cref="IViewModelDescription"/>.
    /// </summary>
    public class DefaultViewModelDescription : MetadataContainer, IViewModelDescription
    {
        private readonly Type _targetType;
        private readonly Dictionary<string, IAction> _actions = new Dictionary<string, IAction>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewModelDescription"/> class.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        public DefaultViewModelDescription(Type targetType)
        {
            _targetType = targetType;
            AddMetadataFrom(targetType);
        }

        /// <summary>
        /// Gets the View Model's type.
        /// </summary>
        /// <value>The type of the View Model.</value>
        public Type TargetType
        {
            get { return _targetType; }
        }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>The filters.</value>
        public IFilterManager Filters { get; set; }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public IAction GetAction(ActionMessage message)
        {
            IAction found;
            _actions.TryGetValue(message.MethodName, out found);
            return found;
        }

        /// <summary>
        /// Adds the action.
        /// </summary>
        /// <param name="action">The action.</param>
        public void AddAction(IAction action)
        {
            _actions[action.Name] = action;
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
    }
}