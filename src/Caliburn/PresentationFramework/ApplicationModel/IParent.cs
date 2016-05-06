namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///   Interface used to define an object associated to a collection of children.
    /// </summary>
    public interface IParent
    {
        /// <summary>
        ///   Gets the children.
        /// </summary>
        /// <returns>
        ///   The collection of children.
        /// </returns>
        IEnumerable GetChildren();
    }

    /// <summary>
    /// Interface used to define a specialized parent.
    /// </summary>
    /// <typeparam name="T">The type of children.</typeparam>
#if NET_40
    public interface IParent<out T> : IParent
#else
    public interface IParent<T> : IParent
#endif
    {
        /// <summary>
        ///   Gets the children.
        /// </summary>
        /// <returns>
        ///   The collection of children.
        /// </returns>
        new IEnumerable<T> GetChildren();
    }
}