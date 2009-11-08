namespace Caliburn.PresentationFramework.ApplicationModel
{
    /// <summary>
    /// A base implementation of <see cref="IPresenter"/>.
    /// </summary>
    public class Presenter : PresenterBase
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
}