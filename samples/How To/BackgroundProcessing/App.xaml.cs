namespace BackgroundProcessing
{
    using System.Reflection;
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
            CaliburnFramework
                .Configure()
                .With.Core()
                .With.Assemblies(Assembly.GetExecutingAssembly())
                .With.PresentationFramework()
                .Start();
        }
    }
}