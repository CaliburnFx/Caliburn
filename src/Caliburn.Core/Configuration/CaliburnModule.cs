namespace Caliburn.Core.Configuration
{
    /// <summary>
    /// A base class for modules that extend Caliburn itself.
    /// </summary>
    /// <typeparam name="TModule">The type of the module.</typeparam>
    public class CaliburnModule<TModule> : ModuleBase, IConfigurationBuilder
        where TModule : CaliburnModule<TModule>, new()
    {
        private static readonly TModule instance = new TModule();

        /// <summary>
        /// Gets the singleton instance of this module.
        /// </summary>
        /// <value>The instance.</value>
        public static TModule Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CaliburnModule&lt;TModule&gt;"/> class.
        /// </summary>
        protected CaliburnModule()
        {
            if (Instance != null)
                throw new CaliburnException("You can only create one instance of " + GetType().FullName + ". Please use the static Instance property.");
        }

        /// <summary>
        /// Allows extension of the configuration.
        /// </summary>
        /// <value>The extensibility hook.</value>
        public IModuleHook With
        {
            get { return CaliburnFramework.ModuleHook; }
        }

        /// <summary>
        /// Starts the framework.
        /// </summary>
        public void Start()
        {
            CaliburnFramework.Instance.Start();
        }
    }
}