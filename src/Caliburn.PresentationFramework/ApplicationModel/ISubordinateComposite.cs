namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System.Collections.Generic;

    /// <summary>
    /// An <see cref="ISubordinate"/> with one or more children.
    /// </summary>
    public interface ISubordinateComposite : ISubordinate
    {
        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISubordinate> GetChildren();
    }
}