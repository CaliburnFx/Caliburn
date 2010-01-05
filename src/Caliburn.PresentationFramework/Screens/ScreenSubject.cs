namespace Caliburn.PresentationFramework.Screens
{
    using System;
    using ViewModels;

    /// <summary>
    /// A simple implementation of <see cref="IScreenSubject"/>.
    /// </summary>
    /// <typeparam name="T">The screen subject's type.</typeparam>
    public class ScreenSubject<T> : IScreenSubject
    {
        private readonly T _subject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenSubject&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="subject">The subject.</param>
        public ScreenSubject(T subject)
        {
            _subject = subject;
        }

        /// <summary>
        /// Determines if the specified screen matches this subject.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <returns>
        /// 	<c>true</c> if the screen matches the subject; otherwise, <c>false</c>.
        /// </returns>
        public bool Matches(IScreen screen)
        {
            var specific = screen as IScreen<T>;
            if (specific == null) return false;

            return specific.Subject.Equals(_subject);
        }

        /// <summary>
        /// Creates the screen.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="constructionComplete">The construction completion callback.</param>
        public void CreateScreen(IViewModelFactory factory, Action<IScreen> constructionComplete)
        {
            constructionComplete(factory.CreateFor(_subject));
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Equals(ScreenSubject<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._subject, _subject);
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
            if (obj.GetType() != typeof(ScreenSubject<T>)) return false;
            return Equals((ScreenSubject<T>)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return _subject.GetHashCode();
        }
    }
}