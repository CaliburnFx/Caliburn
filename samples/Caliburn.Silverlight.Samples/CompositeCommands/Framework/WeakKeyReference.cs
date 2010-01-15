namespace CompositeCommands.Framework
{
    using Caliburn.Core.MemoryManagement;

    /// <summary>
    /// Provides a weak reference to an object of the given type to be used in
    /// a WeakDictionary along with the given comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WeakKeyReference<T> : WeakReference<T> where T : class
    {
        /// <summary>
        /// The HashCode of the key.
        /// </summary>
        public readonly int HashCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakKeyReference&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="comparer">The comparer.</param>
        public WeakKeyReference(T key, WeakKeyComparer<T> comparer)
            : base(key)
        {
            // retain the object's hash code immediately so that even
            // if the target is GC'ed we will be able to find and
            // remove the dead weak reference.
            HashCode = comparer.GetHashCode(key);
        }
    }
}