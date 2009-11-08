namespace Configuration
{
    using System.Windows;
    using System.Windows.Controls;
    using Caliburn.Castle;
    using Caliburn.Core;
    using Caliburn.PresentationFramework;
    using Castle.Windsor;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //Note: Using an external container via adapter.
            //Note: The usage pattern is the same for all supported containers.
            var container = new WindsorContainer();
            var adapter = new WindsorAdapter(container);

            CaliburnFramework
                .ConfigureCore(adapter) //Note: Set the container.
                .WithPresentationFramework()
                .Start();

            //Note: Retrieve one of Caliburn's services.
            var controller = container.Resolve<IRoutedMessageController>();

            //Note: Customize the default behavior of button elements.
            controller.SetupDefaults(
                new GenericInteractionDefaults<Button>(
                    "MouseEnter",
                    (b, v) => b.DataContext = v,
                    b => b.DataContext
                    )
                );

            //Note: Use the above method to add defaults for additional controls as well.
        }
    }
}