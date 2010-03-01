namespace AsyncActions
{
    using System.Windows;
    using Caliburn.Core;
    using Caliburn.Core.Configuration;
    using Caliburn.PresentationFramework;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //Note: This is the most basic configuration needed to get started using Actions.
            CaliburnFramework
                .Configure()
                .With.Core()
                .With.PresentationFramework()
                .Start();
        }
    }
}