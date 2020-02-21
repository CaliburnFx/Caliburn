using Shouldly;

namespace Tests.Caliburn.PresentationFramework
{
    using System;
    using System.Collections.Generic;
    using global::Caliburn.Core;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.PresentationFramework.Views;
    using Fakes.Model;
    using Xunit;


    public class The_DefaultViewLocator : TestBase
    {
        TestFriendlyDefaultViewLocator defaultViewLocator;

        protected override void given_the_context_of()
        {
            base.given_the_context_of();
            defaultViewLocator = new TestFriendlyDefaultViewLocator(
                Mock<IAssemblySource>(),
                Mock<IServiceLocator>()
                );
        }

        void AssertMakeInterface(string part, string expected)
        {
            var interfaceName = defaultViewLocator.MakeInterface(part);
            interfaceName.ShouldBe(expected);
        }

        void AssertMakeInterface<T>(string expected)
        {
            AssertMakeInterface(typeof(T).FullName, expected);
        }

        [Fact]
        public void should_make_an_interface_name_from_a_name_part()
        {
            AssertMakeInterface("A.Simple.Full.Name", "A.Simple.Full.IName");
            AssertMakeInterface<Address>("Tests.Caliburn.Fakes.Model.IAddress");

            //http://caliburn.codeplex.com/workitem/6275

            var stringAQN = typeof(string).AssemblyQualifiedName;
            var intAQN = typeof(int).AssemblyQualifiedName;

            AssertMakeInterface<List<string>>(
                string.Format("System.Collections.Generic.IList`1[[{0}]]", stringAQN)
                );

            AssertMakeInterface<KeyValuePair<int, string>>(
                string.Format("System.Collections.Generic.IKeyValuePair`2[[{0}],[{1}]]", intAQN, stringAQN)
                );
        }
    }

    public class TestFriendlyDefaultViewLocator : DefaultViewLocator
    {
        public TestFriendlyDefaultViewLocator(IAssemblySource assemblySource, IServiceLocator serviceLocator)
            : base(assemblySource, serviceLocator) {}

        public new string MakeInterface(string part)
        {
            return base.MakeInterface(part);
        }

        public new IEnumerable<string> GetTypeNamesToCheck(Type modelType, string context)
        {
            return base.GetTypeNamesToCheck(modelType, context);
        }
    }
}
