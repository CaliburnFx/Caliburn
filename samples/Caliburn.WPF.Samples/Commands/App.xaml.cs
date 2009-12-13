namespace Commands
{
    using System.Reflection;
    using System.Windows;
    using Caliburn.Core;
    using Caliburn.Core.Configuration;
    using Caliburn.Core.IoC;
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.Configuration;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //Note: Explicitly creating the container.
            var container = new SimpleContainer();

            CaliburnFramework
                .Configure(container) //Note: Setting the container.
                .With.Core()
                .With.Assemblies(Assembly.GetExecutingAssembly())
                .With.PresentationFramework()
                .Start();
        }
    }
}