using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Tests.Caliburn.Fakes;

namespace Tests.Caliburn.Core.Metadata
{
    [TestFixture]
    public class The_metadata_provider : TestBase
    {
        [Test]
        public void is_a_custom_attribute()
        {
            var metadata = new FakeMetadata();

            Assert.That(metadata, Is.InstanceOfType(typeof(Attribute)));
        }
    }
}