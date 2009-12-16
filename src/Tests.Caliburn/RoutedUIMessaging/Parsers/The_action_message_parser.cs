namespace Tests.Caliburn.RoutedUIMessaging.Parsers
{
    using global::Caliburn.PresentationFramework;
    using global::Caliburn.PresentationFramework.Actions;
    using Microsoft.Practices.ServiceLocation;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class The_action_message_parser : TestBase
    {
        private ActionMessageParser _parser;

        protected override void given_the_context_of()
        {
            _parser = new ActionMessageParser(MockRepository.GenerateStub<IMessageBinder>());

            var container = Stub<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => container);
        }

        [Test]
        public void can_parse_message_with_no_parameters()
        {
            var result = _parser.Parse(null, "Foo");
            Assert.That(result.Parameters.Count, Is.EqualTo(0));
        }

        [Test]
        public void can_parse_message_with_no_parameters_but_parenthesis()
        {
            var result = _parser.Parse(null, "Foo()");
            Assert.That(result.Parameters.Count, Is.EqualTo(0));
        }

        [Test]
        public void can_parse_unquoted_number_literals()
        {
            var result = _parser.Parse(null, "Foo(1,123,12.34)");
            Assert.That(result.Parameters.Count, Is.EqualTo(3));
            Assert.That(result.Parameters[0].Value, Is.EqualTo("1"));
            Assert.That(result.Parameters[1].Value, Is.EqualTo("123"));
            Assert.That(result.Parameters[2].Value, Is.EqualTo("12.34"));
        }

        [Test]
        public void can_parse_quoted_string_literals()
        {
            var result = _parser.Parse(null, "Foo('a','abc')");
            Assert.That(result.Parameters.Count, Is.EqualTo(2));
            Assert.That(result.Parameters[0].Value, Is.EqualTo("a"));
            Assert.That(result.Parameters[1].Value, Is.EqualTo("abc"));
        }

        [Test]
        public void can_parse_quoted_string_literals_containing_parenthesis()
        {
            var result = _parser.Parse(null, "Foo('a(bc)d')");
            Assert.That(result.Parameters.Count, Is.EqualTo(1));
            Assert.That(result.Parameters[0].Value, Is.EqualTo("a(bc)d"));
        }

        [Test]
        public void can_parse_quoted_string_literals_containing_commas()
        {
            var result = _parser.Parse(null, "Foo(',','a,',',a','a,b')");
            Assert.That(result.Parameters.Count, Is.EqualTo(4));
            Assert.That(result.Parameters[0].Value, Is.EqualTo(","));
            Assert.That(result.Parameters[1].Value, Is.EqualTo("a,"));
            Assert.That(result.Parameters[2].Value, Is.EqualTo(",a"));
            Assert.That(result.Parameters[3].Value, Is.EqualTo("a,b"));
        }

        [Test]
        public void can_parse_quoted_string_literals_containing_single_quotes()
        {
            var result = _parser.Parse(null, @"Foo('The answer is: '42'')");
            Assert.That(result.Parameters.Count, Is.EqualTo(1));
            Assert.That(result.Parameters[0].Value, Is.EqualTo("The answer is: '42'"));
        }

        [Test]
        public void ignores_whitespace_between_parameters()
        {
            var result = _parser.Parse(null, "Foo('abc',   'def',   'ghi')");
            Assert.That(result.Parameters.Count, Is.EqualTo(3));
            Assert.That(result.Parameters[0].Value, Is.EqualTo("abc"));
            Assert.That(result.Parameters[1].Value, Is.EqualTo("def"));
            Assert.That(result.Parameters[2].Value, Is.EqualTo("ghi"));
        }
    }
}