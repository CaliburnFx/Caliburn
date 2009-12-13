namespace Caliburn.Core.Configuration
{
    public class CaliburnModule<TModule> : ModuleBase
        where TModule : CaliburnModule<TModule>, new()
    {
        private static readonly TModule _instance = new TModule();

        public static TModule Instance
        {
            get { return _instance; }
        }

        protected CaliburnModule()
        {
            if (Instance != null)
                throw new CaliburnException("You can only create one instance of " + GetType().FullName + ". Please use the static Instance property.");
        }

        public IModuleHook With
        {
            get { return CaliburnFramework.ModuleHook; }
        }

        public void Start()
        {
            CaliburnFramework.Instance.Start();
        }
    }
}