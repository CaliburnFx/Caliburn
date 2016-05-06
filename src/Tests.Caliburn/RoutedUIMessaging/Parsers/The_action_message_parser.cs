using Shouldly;

namespace Tests.Caliburn.RoutedUIMessaging.Parsers
{
    using global::Caliburn.Core.InversionOfControl;
    using global::Caliburn.PresentationFramework.Actions;
    using global::Caliburn.PresentationFramework.Conventions;
    using global::Caliburn.PresentationFramework.RoutedMessaging;
    using Xunit;
    using Rhino.Mocks;

    
    public class The_action_message_parser : TestBase
    {
        ActionMessageParser parser;

        protected override void given_the_context_of()
        {
            parser = new ActionMessageParser(
                MockRepository.GenerateStub<IConventionManager>(),
                MockRepository.GenerateStub<IMessageBinder>()
                );

            var container = Stub<IServiceLocator>();
            IoC.Initialize(container);
        }

        [Fact]
        public void can_parse_message_with_no_parameters()
        {
            var result = parser.Parse(null, "Foo");
            result.Parameters.Count.ShouldBe(0);
        }

        [Fact]
        public void can_parse_message_with_no_parameters_but_parenthesis()
        {
            var result = parser.Parse(null, "Foo()");
            result.Parameters.Count.ShouldBe(0);
        }

        [Fact]
        public void can_parse_quoted_string_literals()
        {
            var result = parser.Parse(null, "Foo('a','abc')");
            result.Parameters.Count.ShouldBe(2);
            result.Parameters[0].Value.ShouldBe("a");
            result.Parameters[1].Value.ShouldBe("abc");
        }

        [Fact]
        public void can_parse_quoted_string_literals_containing_commas()
        {
            var result = parser.Parse(null, "Foo(',','a,',',a','a,b')");
            result.Parameters.Count.ShouldBe(4);
            result.Parameters[0].Value.ShouldBe(",");
            result.Parameters[1].Value.ShouldBe("a,");
            result.Parameters[2].Value.ShouldBe(",a");
            result.Parameters[3].Value.ShouldBe("a,b");
        }

        [Fact]
        public void can_parse_quoted_string_literals_containing_parenthesis()
        {
            var result = parser.Parse(null, "Foo('a(bc)d')");
            result.Parameters.Count.ShouldBe(1);
            result.Parameters[0].Value.ShouldBe("a(bc)d");
        }

        [Fact]
        public void can_parse_quoted_string_literals_containing_single_quotes()
        {
            var result = parser.Parse(null, @"Foo('The answer is: '42'')");
            result.Parameters.Count.ShouldBe(1);
            result.Parameters[0].Value.ShouldBe("The answer is: '42'");
        }

        [Fact]
        public void can_parse_unquoted_number_literals()
        {
            var result = parser.Parse(null, "Foo(1,123,12.34)");
            result.Parameters.Count.ShouldBe(3);
            result.Parameters[0].Value.ShouldBe("1");
            result.Parameters[1].Value.ShouldBe("123");
            result.Parameters[2].Value.ShouldBe("12.34");
        }

        [Fact]
        public void ignores_whitespace_between_parameters()
        {
            var result = parser.Parse(null, "Foo('abc',   'def',   'ghi')");
            result.Parameters.Count.ShouldBe(3);
            result.Parameters[0].Value.ShouldBe("abc");
            result.Parameters[1].Value.ShouldBe("def");
            result.Parameters[2].Value.ShouldBe("ghi");
        }
    }
}