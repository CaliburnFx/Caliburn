namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of <see cref="INavigator"/>.
    /// </summary>
    public class Navigator : PresenterManager, INavigator
    {
        private readonly Stack<Action<Action<bool>>> _next = new Stack<Action<Action<bool>>>();
        private readonly Stack<Action<Action<bool>>> _previous = new Stack<Action<Action<bool>>>();

        private Action<Action<bool>> _current;
        private bool _isNavigating;

        /// <summary>
        /// Gets a value indicating whether the history can be cleared.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the history can be cleared; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CanClearHistory
        {
            get { return CanGoBack || CanGoForward; }
        }

        /// <summary>
        /// Gets the total number of actions tracked by the navigator 
        /// </summary>
        public virtual int Count
        {
            get { return _next.Count + CurrentPosition; }
        }

        /// <summary>
        /// Gets the position of the current action
        /// </summary>
        /// <remarks>
        /// This is intended for display within a UI.
        /// </remarks>
        public virtual int CurrentPosition
        {
            get { return _previous.Count + ((_current == null) ? 0 : 1); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can navigate back.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can go back; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CanGoBack
        {
            get { return _previous.Count > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can navigate forward.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can go forward; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CanGoForward
        {
            get { return _next.Count > 0; }
        }

        /// <summary>
        /// Navigates forward.
        /// </summary>
        public virtual void Forward(Action<bool> completed)
        {
            if (!CanGoForward)
            {
                completed(false);
                return;
            }

            _isNavigating = true;

            if (_current != null)
                _previous.Push(_current);

            _current = _next.Pop();

            _current(
                isSuccess =>{
                    if(!isSuccess)
                    {
                        _next.Push(_current);
                        _current = _previous.Count > 0 ? _previous.Pop() : null;
                    }

                    RaiseChangeNotifications();

                    _isNavigating = false;

                    completed(isSuccess);
                });
        }

        /// <summary>
        /// Navigates back.
        /// </summary>
        public virtual void Back(Action<bool> completed)
        {
            if (!CanGoBack)
            {
                completed(false);
                return;
            }

            _isNavigating = true;

            if (_current != null)
                _next.Push(_current);

            _current = _previous.Pop();

            _current(
                isSuccess =>
                    {
                        if (!isSuccess)
                        {
                            _previous.Push(_current);
                            _current = _next.Count > 0 ? _next.Pop() : null;
                        }

                        RaiseChangeNotifications();

                        _isNavigating = false;

                        completed(isSuccess);
                    });
        }

        /// <summary>
        /// Navigates using the specified action.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="completed">Called when navigation is completed.</param>
        public virtual void Navigate(Action<Action<bool>> function, Action<bool> completed)
        {
            _next.Clear();
            _next.Push(function);

            Forward(completed);
        }

        /// <summary>
        /// Clears the navigation history.
        /// </summary>
        public virtual void ClearHistory()
        {
            _previous.Clear();
            _next.Clear();

            RaiseChangeNotifications();
        }

        /// <summary>
        /// Navigates forward.
        /// </summary>
        public virtual void Forward()
        {
            Forward(result => { });
        }

        /// <summary>
        /// Navigates back.
        /// </summary>
        public virtual void Back()
        {
            Back(result => { });
        }

        /// <summary>
        /// Notifies subscribers of the property change.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public override void NotifyOfPropertyChange(string propertyName)
        {
            if (propertyName == "CurrentPresenter")
            {
                if (!_isNavigating)
                {
                    var presenter = CurrentPresenter;

                    if (_current != null)
                        _previous.Push(_current);

                    if (presenter != null)
                        _current = completed => Open(presenter, completed);
                    else _current = null;

                    _next.Clear();

                    RaiseChangeNotifications();
                }
            }

            base.NotifyOfPropertyChange(propertyName);
        }

        /// <summary>
        /// Raises all related change notifications for the Navigator.
        /// </summary>
        protected virtual void RaiseChangeNotifications()
        {
            NotifyOfPropertyChange("CanGoBack");
            NotifyOfPropertyChange("CanGoForward");
            NotifyOfPropertyChange("CanClearHistory");
            NotifyOfPropertyChange("Count");
            NotifyOfPropertyChange("CurrentPosition");
        }
    }
}