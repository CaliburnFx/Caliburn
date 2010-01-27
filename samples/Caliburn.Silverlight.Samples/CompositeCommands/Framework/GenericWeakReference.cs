namespace Caliburn.Core.MemoryManagement
{
    using System;

    /// <summary>
    /// Adds strong typing to WeakReference.Target using generics.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WeakReference<T>
        where T : class
    {
        private readonly WeakReference _inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReference&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public WeakReference(T target)
        {
            _inner = new WeakReference(target);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
        public bool IsAlive
        {
            get { return _inner.IsAlive; }
        }

        /// <summary>
        /// Gets or sets the object (the target) referenced by the current <see cref="T:System.WeakReference"/> object.
        /// </summary>
        /// <value></value>
        /// <returns>null if the object referenced by the current <see cref="T:System.WeakReference"/> object has been garbage collected; otherwise, a reference to the object referenced by the current <see cref="T:System.WeakReference"/> object.</returns>
        /// <exception cref="T:System.InvalidOperationException">The reference to the target object is invalid. This exception can be thrown while setting this property if the value is a null reference or if the object has been finalized during the set operation.</exception>
        public T Target
        {
            get { return (T)_inner.Target; }
        }
    }
}