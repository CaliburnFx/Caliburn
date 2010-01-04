namespace Caliburn.PresentationFramework.Screens
{
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
}