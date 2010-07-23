namespace Caliburn.PresentationFramework.Screens
{
    using System;
    using ViewModels;

    /// <summary>
    /// A simple implementation of <see cref="ISubjectSpecification"/>.
    /// </summary>
    /// <typeparam name="T">The screen subject's type.</typeparam>
    public class SubjectSpecification<T> : ISubjectSpecification
    {
        private readonly T subject;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubjectSpecification{T}"/> class.
        /// </summary>
        /// <param name="subject">The subject.</param>
        public SubjectSpecification(T subject)
        {
            this.subject = subject;
        }

        /// <summary>
        /// Determines if the specified screen matches this subject.
        /// </summary>
        /// <param name="itemWithSubject">The screen.</param>
        /// <returns>
        /// 	<c>true</c> if the screen matches the subject; otherwise, <c>false</c>.
        /// </returns>
        public bool Matches(IHaveSubject itemWithSubject)
        {
            var specific = itemWithSubject as IHaveSubject<T>;
            if (specific == null) return false;

            return specific.Subject.Equals(subject);
        }

        /// <summary>
        /// Creates the screen.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="constructionComplete">The construction completion callback.</param>
        public void CreateSubjectHost(IViewModelFactory factory, Action<IHaveSubject> constructionComplete)
        {
            constructionComplete(factory.CreateFor(subject));
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>The result of the equality check.</returns>
        public bool Equals(SubjectSpecification<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.subject, subject);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(SubjectSpecification<T>)) return false;
            return Equals((SubjectSpecification<T>)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return subject.GetHashCode();
        }
    }
}