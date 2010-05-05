namespace DelayedValidation
{
    using Caliburn.PresentationFramework.ApplicationModel;
    using Framework;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : CaliburnApplication
    {
        public App()
        {
            InitializeComponent();
        }

        protected override object CreateRootModel()
        {
            return Container.GetInstance<IShell>();
        }
    }
}