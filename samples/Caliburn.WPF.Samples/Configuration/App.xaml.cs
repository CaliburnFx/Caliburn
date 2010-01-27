namespace Configuration
{
    using System.Windows;
    using System.Windows.Controls;
    using Caliburn.Windsor;
    using Caliburn.Core.Configuration;
    using Caliburn.PresentationFramework.Configuration;
    using Caliburn.PresentationFramework.Conventions;
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
                .Configure(adapter)
                .With.Core()
                .With.PresentationFramework()
                .Start();

            //Note: Retrieve one of Caliburn's services.
            var controller = adapter.GetInstance<IConventionManager>();

            //Note: Customize the default behavior of button elements.
            controller.AddElementConvention(
                new DefaultElementConvention<Button>(
                    "MouseEnter",
                    Button.ContentProperty,
                    (b, v) => b.DataContext = v,
                    b => b.DataContext
                    )
                );

            //Note: Use the above method to add defaults for additional controls as well.
        }
    }
}