namespace Caliburn.PresentationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using Core;
    using Core.Invocation;

#if SILVERLIGHT
    using Core;
#endif

    /// <summary>
    /// A base collection class that supports automatic UI thread marshalling.
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class BindableCollection<T> : ObservableCollection<T>, IObservableCollection<T>
    {
        private bool _raiseCollectionChanged = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableCollection&lt;T&gt;"/> class.
        /// </summary>
        public BindableCollection() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="collection"/> parameter cannot be null.
        /// </exception>
        public BindableCollection(IEnumerable<T> collection)
#if !SILVERLIGHT
            : base(collection)
#endif
        {
#if SILVERLIGHT
            collection.Apply(Add);
#endif
        }

        /// <summary>
        /// Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.PropertyChanged"/> event with the provided arguments.
        /// </summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            Execute.OnUIThread(() => RaisePropertyChangedEventImmediately(e));
        }

        /// <summary>
        /// Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged"/> event with the provided arguments.
        /// </summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_raiseCollectionChanged)
                return;

            Execute.OnUIThread(() => RaiseCollectionChangedEventImmediately(e));
        }

        /// <summary>
        /// Raises the collection changed event immediately.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        public void RaiseCollectionChangedEventImmediately(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
        }

        /// <summary>
        /// Raises the property changed event immediately.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        public void RaisePropertyChangedEventImmediately(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        public void AddRange(IEnumerable<T> items)
        {
            _raiseCollectionChanged = false;
            items.Apply(Add);
            _raiseCollectionChanged = true;

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}