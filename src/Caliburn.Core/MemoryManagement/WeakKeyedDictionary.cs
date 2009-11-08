namespace Caliburn.Core.MemoryManagement
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A generic dictionary, which allows its keys 
    /// to be garbage collected if there are no other references
    /// to them than from the dictionary itself.
    /// </summary>
    ///
    /// <remarks>
    /// If the key of a particular entry in the dictionary
    /// has been collected, then both the key and value become effectively
    /// unreachable. However, left-over WeakReference objects for the key
    /// and value will physically remain in the dictionary until
    /// RemoveCollectedEntries is called. This will lead to a discrepancy
    /// between the Count property and the number of iterations required
    /// to visit all of the elements of the dictionary using its
    /// enumerator or those of the Keys and Values collections. Similarly,
    /// CopyTo will copy fewer than Count elements in this situation.
    /// </remarks>
    public sealed class WeakKeyedDictionary<TKey, TValue> : BaseDictionary<TKey, TValue>
        where TKey : class
    {
        private readonly WeakKeyComparer<TKey> comparer;
        private readonly Dictionary<object, TValue> dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakKeyedDictionary{TKey,TValue}"/> class.
        /// </summary>
        public WeakKeyedDictionary()
            : this(0, null) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakKeyedDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public WeakKeyedDictionary(int capacity)
            : this(capacity, null) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakKeyedDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public WeakKeyedDictionary(IEqualityComparer<TKey> comparer)
            : this(0, comparer) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakKeyedDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <param name="comparer">The comparer.</param>
        public WeakKeyedDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            this.comparer = new WeakKeyComparer<TKey>(comparer);
            dictionary = new Dictionary<object, TValue>(capacity, this.comparer);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</returns>
        ///<remarks>
        /// WARNING: The count returned here may include entries for which
        /// either the key or value objects have already been garbage
        /// collected. Call RemoveCollectedEntries to weed out collected
        /// entries and update the count accordingly.
        /// </remarks>
        public override int Count
        {
            get { return dictionary.Count; }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public override void Add(TKey key, TValue value)
        {
            if(key == null) throw new ArgumentNullException("key");
            var weakKey = new WeakKeyReference<TKey>(key, comparer);
            dictionary.Add(weakKey, value);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.</exception>
        public override bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public override bool Remove(TKey key)
        {
            return dictionary.Remove(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.</exception>
        public override bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected override void SetValue(TKey key, TValue value)
        {
            WeakReference<TKey> weakKey = new WeakKeyReference<TKey>(key, comparer);
            dictionary[weakKey] = value;
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        public override void Clear()
        {
            dictionary.Clear();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach(var kvp in dictionary)
            {
                var weakKey = (WeakReference<TKey>)(kvp.Key);
                var key = weakKey.Target;
                var value = kvp.Value;
                if(weakKey.IsAlive)
                    yield return new KeyValuePair<TKey, TValue>(key, value);
            }
        }

        /// <summary>
        /// Removes the left-over weak references for entries in the dictionary
        /// whose key has already been reclaimed by the garbage
        /// collector. This will reduce the dictionary's Count by the number
        /// of dead key-value pairs that were eliminated.
        /// </summary>
        public void RemoveCollectedEntries()
        {
            List<object> toRemove = null;
            foreach(var pair in dictionary)
            {
                var weakKey = (WeakReference<TKey>)(pair.Key);

                if(!weakKey.IsAlive)
                {
                    if(toRemove == null)
                        toRemove = new List<object>();
                    toRemove.Add(weakKey);
                }
            }

            if(toRemove != null)
            {
                foreach(var key in toRemove)
                    dictionary.Remove(key);
            }
        }
    }
}