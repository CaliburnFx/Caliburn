namespace Caliburn.PresentationFramework.Actions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Core.Logging;
    using Filters;
    using RoutedMessaging;

    /// <summary>
    /// An overloaded <see cref="IAction"/>.
    /// </summary>
    public class OverloadedAction : IAction, IEnumerable<IAction>
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(OverloadedAction));

        private readonly string _name;
        private readonly List<IAction> _overloads = new List<IAction>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OverloadedAction"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public OverloadedAction(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the requirements.
        /// </summary>
        /// <value>The requirements.</value>
        public IList<RequiredParameter> Requirements
        {
            get { return new List<RequiredParameter>(); }
        }

        /// <summary>
        /// Occurs when action has completed.
        /// </summary>
        public event EventHandler Completed = delegate { };

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>The filters.</value>
        public IFilterManager Filters
        {
            get
            {
                var currentManager = _overloads[0].Filters;

                for(int i = 1; i < _overloads.Count; i++)
                {
                    currentManager = currentManager.Combine(
                        _overloads[i].Filters
                        );
                }

                return currentManager;
            }
        }

        /// <summary>
        /// Determines whether this action matches the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public bool Matches(ActionMessage message)
        {
            foreach(var overload in _overloads)
            {
                if(overload.Matches(message))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Adds the overload.
        /// </summary>
        /// <param name="action">The action.</param>
        public void AddOverload(IAction action)
        {
            action.Completed += delegate { OnCompleted(); };
            _overloads.Add(action);
        }

        /// <summary>
        /// Determines the overload or fail.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public IAction DetermineOverloadOrFail(ActionMessage message)
        {
            foreach(var overload in _overloads)
            {
                if(overload.Matches(message))
                    return overload;
            }

            if(message.Parameters.Count > 0)
            {
                var ex = new CaliburnException(
                    string.Format(
                        "Could not find overload for {0}({1}).",
                        message.MethodName,
                        message.Parameters
                            .Select(x => x.Value == null ? "null" : x.Value.GetType().Name)
                            .Aggregate((a, c) => a + ", " + c)
                        )
                    );

                Log.Error(ex);
                throw ex;
            }

            var ex2 = new CaliburnException(
                string.Format(
                    "Could not find overload for {0}().",
                    message.MethodName
                    )
                );

            Log.Error(ex2);
            throw ex2;
        }

        /// <summary>
        /// Determines how this instance affects trigger availability.
        /// </summary>
        /// <param name="actionMessage">The action message.</param>
        /// <param name="handlingNode">The node.</param>
        /// <returns>
        /// 	<c>true</c> if this instance enables triggers; otherwise, <c>false</c>.
        /// </returns>
        public bool ShouldTriggerBeAvailable(ActionMessage actionMessage, IInteractionNode handlingNode)
        {
            return DetermineOverloadOrFail(actionMessage).ShouldTriggerBeAvailable(actionMessage, handlingNode);
        }

        /// <summary>
        /// Executes the specified this action on the specified target.
        /// </summary>
        /// <param name="actionMessage">The action message.</param>
        /// <param name="handlingNode">The node.</param>
        /// <param name="context">The context.</param>
        public void Execute(ActionMessage actionMessage, IInteractionNode handlingNode, object context)
        {
            DetermineOverloadOrFail(actionMessage).Execute(actionMessage, handlingNode, context);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IAction> GetEnumerator()
        {
            return _overloads.GetEnumerator();
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
        /// Called when completed event needs to fire.
        /// </summary>
        protected virtual void OnCompleted()
        {
            Completed(this, EventArgs.Empty);
        }
    }
}