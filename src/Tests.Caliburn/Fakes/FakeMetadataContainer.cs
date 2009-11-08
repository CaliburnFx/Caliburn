using Caliburn.Core.Metadata;

namespace Tests.Caliburn.Fakes
{
    [FakeMetadata]
    public class FakeMetadataContainer : MetadataContainer
    {
        public void AddMetadataBasedOnMemberInfo()
        {
            AddMetadataFrom(typeof(FakeMetadataContainer));
        }
    }
}