namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of <see cref="ISubordinateComposite"/> that has multiple children.
    /// </summary>
    public class SubordinateGroup : BindableCollection<ISubordinate>, ISubordinateComposite
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubordinateGroup"/> class.
        /// </summary>
        /// <param name="master">The master.</param>
        public SubordinateGroup(IPresenter master)
        {
            Master = master;
        }

        /// <summary>
        /// Gets the <see cref="IPresenter"/> that owns this instance.
        /// </summary>
        /// <value>The master.</value>
        public virtual IPresenter Master { get; protected set; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<ISubordinate> GetChildren()
        {
            return this;
        }
    }
}