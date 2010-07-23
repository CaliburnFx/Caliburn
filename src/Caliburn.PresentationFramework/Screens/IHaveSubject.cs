namespace Caliburn.PresentationFramework.Screens
{
    /// <summary>
    /// Indicates an item which has subject matter.
    /// </summary>
    public interface IHaveSubject
    {
        /// <summary>
        /// Gets the subject.
        /// </summary>
        /// <value>The subject.</value>
        object Subject { get; }

        /// <summary>
        /// Configures the screen with the subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns>Self</returns>
        IHaveSubject WithSubject(object subject);
    }

    /// <summary>
    /// Indicates an item which has subject matter.
    /// </summary>
    public interface IHaveSubject<T> : IHaveSubject
    {
        /// <summary>
        /// Gets the subject.
        /// </summary>
        /// <value>The subject.</value>
        new T Subject { get; }

        /// <summary>
        /// Configures the screen with the subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns>Self</returns>
        IHaveSubject<T> WithSubject(T subject);
    }
}