namespace Caliburn.PresentationFramework.Screens
{
    using Behaviors;

    /// <summary>
    /// A basic implementation of <see cref="IHaveSubject{T}"/>
    /// </summary>
    /// <typeparam name="T">The screen's type.</typeparam>
    public class Screen<T> : Screen, IHaveSubject<T>
    {
        T subject;

        /// <summary>
        /// Gets the subject.
        /// </summary>
        /// <value>The subject.</value>
        object IHaveSubject.Subject
        {
            get { return Subject; }
        }

        /// <summary>
        /// Gets the subject.
        /// </summary>
        /// <value>The subject.</value>
        [DoNotNotify]
        public virtual T Subject
        {
            get { return subject; }
        }

        /// <summary>
        /// Configures the screen with the subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns>Self</returns>
        IHaveSubject IHaveSubject.WithSubject(object subject)
        {
            return WithSubject((T)subject);
        }

        /// <summary>
        /// Configures the screen with the subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns>Self</returns>
        public virtual IHaveSubject<T> WithSubject(T subject)
        {
            this.subject = subject;
            NotifyOfPropertyChange(() => Subject);
            return this;
        }
    }
}