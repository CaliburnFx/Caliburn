using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Tests.Caliburn.Fakes;

namespace Tests.Caliburn.Core.Metadata
{
    [TestFixture]
    public class A_metadata_container : TestBase
    {
        private FakeMetadataContainer _container;

        protected override void given_the_context_of()
        {
            _container = new FakeMetadataContainer();
        }

        [Test]
        public void can_add_and_get_metadata()
        {
            var metadata = new FakeMetadata();

            _container.AddMetadata(metadata);

            var found = _container.GetMetadata<FakeMetadata>();

            Assert.That(found, Is.EqualTo(metadata));
        }

        [Test]
        public void returns_null_if_metadata_not_found()
        {
            var metadata = _container.GetMetadata<FakeMetadata>();

            Assert.That(metadata, Is.Null);
        }

        [Test]
        public void can_add_metadata_based_on_member_info()
        {
            _container.AddMetadataBasedOnMemberInfo();

            var found = _container.GetMetadata<FakeMetadata>();

            Assert.That(found, Is.Not.Null);
        }

        [Test]
        public void can_get_matching_metadata()
        {
            _container.AddMetadata(new FakeMetadata());
            _container.AddMetadata(new FakeMetadata2());

            var found = _container.GetMatchingMetadata<IFakeMetadata>().ToList();

            Assert.That(found.Count, Is.EqualTo(2));
        }
    }
}