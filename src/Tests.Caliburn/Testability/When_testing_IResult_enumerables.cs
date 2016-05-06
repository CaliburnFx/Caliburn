using Shouldly;

namespace Tests.Caliburn.Testability
{
    using System.Collections.Generic;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using global::Caliburn.PresentationFramework.Screens;
    using global::Caliburn.ShellFramework.Results;
    using global::Caliburn.Testability;
    using Xunit;

    
    public class When_testing_IResult_enumerables : TestBase
    {
        [Fact]
        public void can_enumerate_results_without_executing()
        {
            var results = new TestResultEnumerator(DoAnimation());
            var animation = results.Next<AnimationResult>();

            animation.Key.ShouldBe("4");
        }

        [Fact]
        public void can_enumerate_multiple_results_without_executing()
        {
            var results = new TestResultEnumerator(DoSeveralThings());

            var animation = results.Next<AnimationResult>();
            animation.Key.ShouldBe("4");

            var showChild = results.Next<OpenChildResult<FakeScreen>>();
            showChild.ShouldNotBeNull();
        }

        IEnumerable<IResult> DoAnimation()
        {
            int i = 2 * 2;
            yield return Animation.Begin(i.ToString());
        }

        IEnumerable<IResult> DoSeveralThings()
        {
            foreach(var result in DoAnimation())
            {
                yield return result;
            }

            yield return Show.Child<FakeScreen>();
        }

        private class FakeScreen : Screen
        {
            
        }
    }
}