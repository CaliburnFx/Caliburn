using System;
using Caliburn.Core.Metadata;

namespace Tests.Caliburn.Fakes
{
    public interface IFakeMetadata : IMetadata{}

    public class FakeMetadata : Attribute, IFakeMetadata {}

    public class FakeMetadata2 : Attribute, IFakeMetadata { }
}