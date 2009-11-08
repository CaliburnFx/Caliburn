namespace Caliburn.ModelFramework
{
    using System.Collections.Generic;
    using System.Collections.Specialized;

    /// <summary>
    /// A collection that supports <see cref="IModelNode"/> semantics.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICollectionNode<T> : IList<T>, IModelNode, INotifyCollectionChanged {}
}