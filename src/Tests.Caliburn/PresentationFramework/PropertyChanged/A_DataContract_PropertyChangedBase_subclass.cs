using Shouldly;

namespace Tests.Caliburn.PresentationFramework.PropertyChanged
{
    using System.IO;
    using System.Runtime.Serialization;
    using global::Caliburn.PresentationFramework;
    using Xunit;

    
    public class A_DataContract_PropertyChangedBase_subclass : TestBase
    {
        [DataContract]
        public class FakeDataContractNotifier : PropertyChangedBase
        {
            string fakeProperty;

            [DataMember]
            public string FakeProperty
            {
                get { return fakeProperty; }
                set
                {
                    fakeProperty = value;
                    NotifyOfPropertyChange(() => FakeProperty);
                }
            }
        }

        [Fact]
        public void can_accept_handlers_after_deserialization()
        {
            var original = new FakeDataContractNotifier();

            var serializer = new DataContractSerializer(typeof(FakeDataContractNotifier));
            var stream = new MemoryStream();
            serializer.WriteObject(stream, original);

            stream.Position = 0;
            var deserialized = (FakeDataContractNotifier)serializer.ReadObject(stream);

            var handlerCalls = 0;
            deserialized.PropertyChanged += (o, e) => { handlerCalls++; };
            deserialized.FakeProperty = "some string value";

            handlerCalls.ShouldBe(1);
        }

        [Fact]
        public void can_be_serialized_and_deserialized()
        {
            var original = new FakeDataContractNotifier();
            original.FakeProperty = "some string value";

            var serializer = new DataContractSerializer(typeof(FakeDataContractNotifier));
            var stream = new MemoryStream();
            serializer.WriteObject(stream, original);

            stream.Position = 0;
            var deserialized = (FakeDataContractNotifier)serializer.ReadObject(stream);

            original.ShouldNotBeSameAs(deserialized);
            original.FakeProperty.ShouldBe(deserialized.FakeProperty);
        }

        [Fact]
        public void can_notify_after_deserialization()
        {
            var original = new FakeDataContractNotifier();

            var serializer = new DataContractSerializer(typeof(FakeDataContractNotifier));
            var stream = new MemoryStream();
            serializer.WriteObject(stream, original);

            stream.Position = 0;
            var deserialized = (FakeDataContractNotifier)serializer.ReadObject(stream);

            deserialized.FakeProperty = "some string value";
        }
    }
}