using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Caliburn.PresentationFramework
{
    /// <summary>
    /// Represents a collection that is observable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObservableCollection<T> : IList<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        
    }
}