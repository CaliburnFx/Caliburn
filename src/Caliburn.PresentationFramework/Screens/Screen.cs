namespace Caliburn.PresentationFramework.Screens
{
    using Behaviors;

    /// <summary>
    /// A base implementation of <see cref="IScreen"/> and <see cref="IScreenEx"/>.
    /// </summary>
    public class Screen : ScreenBase
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
            if(!IsInitialized)
            {
                OnInitialize();
                IsInitialized = true;
            }
        }

        /// <summary>
        /// Shutdowns this instance.
        /// </summary>
        public override void Shutdown()
        {
            OnShutdown();
        }

        /// <summary>
        /// Activates this instance.
        /// </summary>
        public override void Activate()
        {
            if(!IsActive)
            {
                OnActivate();
                IsActive = true;
            }
        }

        /// <summary>
        /// Deactivates this instance.
        /// </summary>
        public override void Deactivate()
        {
            if(IsActive)
            {
                OnDeactivate();
                IsActive = false;
            }
        }
    }

    /// <summary>
    /// A basic implementation of <see cref="IScreen{T}"/>
    /// </summary>
    /// <typeparam name="T">The screen's type.</typeparam>
    public class Screen<T> : Screen, IScreen<T>
    {
        private T _subject;

        /// <summary>
        /// Gets the subject.
        /// </summary>
        /// <value>The subject.</value>
        [DoNotNotify]
        public virtual T Subject
        {
            get { return _subject; }
        }

        /// <summary>
        /// Configures the screen with the subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns>Self</returns>
        public virtual IScreen<T> WithSubject(T subject)
        {
            _subject = subject;
            NotifyOfPropertyChange(() => Subject);
            return this;
        }
    }
}