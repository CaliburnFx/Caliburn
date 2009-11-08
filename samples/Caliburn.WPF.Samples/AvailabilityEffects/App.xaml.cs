namespace AvailabilityEffects
{
    using System.Windows;
    using Caliburn.Core;
    using Caliburn.PresentationFramework;

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
                .ConfigureCore(container) //Note: Setting the container.
                .WithPresentationFramework()
                .Start();

            //Note: Register the custom IAvailabilityEffect by key.
            container.RegisterSingleton<OpacityEffect>("Opacity");
        }
    }
}