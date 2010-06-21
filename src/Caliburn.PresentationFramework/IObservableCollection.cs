namespace Caliburn.PresentationFramework
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    /// <summary>
    /// Represents a collection that is observable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObservableCollection<T> : IList<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        void AddRange(IEnumerable<T> items);

        /// <summary>
        /// Removes the range.
        /// </summary>
        /// <param name="items">The items.</param>
        void RemoveRange(IEnumerable<T> items);
    }
}