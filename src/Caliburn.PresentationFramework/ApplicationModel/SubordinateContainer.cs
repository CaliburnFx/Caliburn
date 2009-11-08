namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of <see cref="ISubordinateComposite"/> that supports exactly one child.
    /// </summary>
    public class SubordinateContainer : ISubordinateComposite
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubordinateContainer"/> class.
        /// </summary>
        /// <param name="master">The master.</param>
        /// <param name="child">The child.</param>
        public SubordinateContainer(IPresenter master, ISubordinate child)
        {
            Master = master;
            Child = child;
        }

        /// <summary>
        /// Gets the <see cref="IPresenter"/> that owns this instance.
        /// </summary>
        /// <value>The master.</value>
        public virtual IPresenter Master { get; protected set; }

        /// <summary>
        /// Gets or sets the child.
        /// </summary>
        /// <value>The child.</value>
        public virtual ISubordinate Child { get; protected set; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<ISubordinate> GetChildren()
        {
            yield return Child;
        }
    }
}