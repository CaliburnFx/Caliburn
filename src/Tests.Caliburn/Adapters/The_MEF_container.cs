using System.ComponentModel.Composition.Hosting;
using Caliburn.MEF;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Tests.Caliburn.Adapters.Components;

namespace Tests.Caliburn.Adapters
{
    [TestFixture]
    public class The_MEF_container : ServiceLocatorTests
    {
        protected override IServiceLocator CreateServiceLocator()
        {
            var catalog = new TypeCatalog(typeof(SimpleLogger), typeof(AdvancedLogger));
            var container = new CompositionContainer(catalog);
            return new MEFAdapter(container);
        }
    }
}