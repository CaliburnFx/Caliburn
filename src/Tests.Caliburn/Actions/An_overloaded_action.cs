using Shouldly;

namespace Tests.Caliburn.Actions
{
    using System.Linq;
    using global::Caliburn.Core;
    using global::Caliburn.Core.Invocation;
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.PresentationFramework.Actions;
    using global::Caliburn.PresentationFramework.Filters;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using Xunit;


    public class An_overloaded_action : TestBase
    {
        OverloadedAction action;

        protected override void given_the_context_of()
        {
            var methodFactory = new DefaultMethodFactory();

            action = new OverloadedAction("Test");

            var infos = typeof(MethodHost)
                .GetMethods()
                .Where(x => x.Name == "Test");

            foreach(var info in infos)
            {
                action.AddOverload(
                    new SynchronousAction(
                        Mock<IServiceLocator>(),
                        methodFactory.CreateFrom(info),
                        Mock<IMessageBinder>(),
                        Mock<IFilterManager>(),
                        false
                        )
                    );
            }
        }

        public class MethodHost
        {
            public void Test(int number) {}
            public void Test(int number, string text) {}
            public void Test(int number, string text, double value) {}
        }

        [Fact]
        public void can_determine_overload()
        {
            var message = new ActionMessage();

            message.Parameters.Add(new Parameter(5));
            var found = action.DetermineOverloadOrFail(message);

            found.ShouldNotBeNull();

            message.Parameters.Add(new Parameter("hello"));
            found = action.DetermineOverloadOrFail(message);

            found.ShouldNotBeNull();

            message.Parameters.Add(new Parameter(5d));
            found = action.DetermineOverloadOrFail(message);

            found.ShouldNotBeNull();
        }

        [Fact]
        public void fails_if_no_match_is_found()
        {
            Assert.Throws<CaliburnException>(() =>{
                var message = new ActionMessage();
                action.DetermineOverloadOrFail(message);
            });
        }
    }
}
