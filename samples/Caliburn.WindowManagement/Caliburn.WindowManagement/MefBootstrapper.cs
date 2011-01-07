namespace Caliburn.WindowManagement {
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using Core.InversionOfControl;
    using MEF;
    using PresentationFramework.ApplicationModel;

    public class MefBootstrapper : Bootstrapper<IShell> {
        protected override IServiceLocator CreateContainer() {
            var container = new CompositionContainer(
                new AggregateCatalog(SelectAssemblies().Select(x => new AssemblyCatalog(x)))
                );

            return new MEFAdapter(container);
        }
    }
}