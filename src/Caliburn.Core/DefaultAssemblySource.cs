namespace Caliburn.Core
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reflection;

    /// <summary>
    /// The default implementation of <see cref="IAssemblySource"/>
    /// </summary>
    public class DefaultAssemblySource : Collection<Assembly>, IAssemblySource
    {
        /// <summary>
        /// Occurs when an assembly is added.
        /// </summary>
        public event Action<Assembly> AssemblyAdded = delegate { };

        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is less than zero.
        /// -or-
        /// <paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.
        /// </exception>
        protected override void InsertItem(int index, Assembly item)
        {
            base.InsertItem(index, item);
            AssemblyAdded(item);
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index. The value can be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is less than zero.
        /// -or-
        /// <paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.
        /// </exception>
        protected override void SetItem(int index, Assembly item)
        {
            base.SetItem(index, item);
            AssemblyAdded(item);
        }
    }
}