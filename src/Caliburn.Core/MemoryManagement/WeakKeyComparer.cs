namespace Caliburn.Core.MemoryManagement
{
    using System.Collections.Generic;

    /// <summary>
    /// Compares objects of the given type or WeakKeyReferences to them
    /// for equality based on the given comparer. Note that we can only
    /// implement IEqualityComparer{T} for T = object as there is no
    /// other common base between T and WeakKeyReference{T}. We need a
    /// single comparer to handle both types because we don't want to
    /// allocate a new weak reference for every lookup.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class WeakKeyComparer<T> : IEqualityComparer<object>
        where T : class
    {
        private readonly IEqualityComparer<T> comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakKeyComparer&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public WeakKeyComparer(IEqualityComparer<T> comparer)
        {
            if(comparer == null)
                comparer = EqualityComparer<T>.Default;

            this.comparer = comparer;
        }

        #region IEqualityComparer<object> Members

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
        public int GetHashCode(object obj)
        {
            var weakKey = obj as WeakKeyReference<T>;
            return weakKey != null ? weakKey.HashCode : comparer.GetHashCode((T)obj);
        }

        // Note: There are actually 9 cases to handle here.
        //
        //  Let Wa = Alive Weak Reference
        //  Let Wd = Dead Weak Reference
        //  Let S  = Strong Reference
        //  
        //  x  | y  | Equals(x,y)
        // -------------------------------------------------
        //  Wa | Wa | comparer.Equals(x.Target, y.Target)
        //  Wa | Wd | false
        //  Wa | S  | comparer.Equals(x.Target, y)
        //  Wd | Wa | false
        //  Wd | Wd | x == y
        //  Wd | S  | false
        //  S  | Wa | comparer.Equals(x, y.Target)
        //  S  | Wd | false
        //  S  | S  | comparer.Equals(x, y)
        // -------------------------------------------------

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T"/> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T"/> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public new bool Equals(object x, object y)
        {
            bool xIsDead, yIsDead;
            var first = GetTarget(x, out xIsDead);
            var second = GetTarget(y, out yIsDead);

            if(xIsDead)
                return yIsDead ? x == y : false;

            return !yIsDead && comparer.Equals(first, second);
        }

        #endregion

        private static T GetTarget(object obj, out bool isDead)
        {
            var wref = obj as WeakKeyReference<T>;
            T target;
            if(wref != null)
            {
                target = wref.Target;
                isDead = !wref.IsAlive;
            }
            else
            {
                target = (T)obj;
                isDead = false;
            }
            return target;
        }
    }
}