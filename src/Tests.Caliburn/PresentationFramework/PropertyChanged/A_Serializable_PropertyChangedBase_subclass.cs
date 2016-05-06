using Shouldly;

namespace Tests.Caliburn.PresentationFramework.PropertyChanged
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using global::Caliburn.PresentationFramework;
    using Xunit;

    
    public class A_Serializable_PropertyChangedBase_subclass : TestBase
    {
        [Serializable]
        public class FakeSerializableNotifier : PropertyChangedBase
        {
            string fakeProperty;

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
            var original = new FakeSerializableNotifier();

            var serializer = new BinaryFormatter();
            var stream = new MemoryStream();
            serializer.Serialize(stream, original);

            stream.Position = 0;
            var deserialized = (FakeSerializableNotifier)serializer.Deserialize(stream);

            var handlerCalls = 0;
            deserialized.PropertyChanged += (o, e) => { handlerCalls++; };
            deserialized.FakeProperty = "some string value";

            handlerCalls.ShouldBe(1);
        }

        [Fact]
        public void can_be_serialized_and_deserialized()
        {
            var original = new FakeSerializableNotifier();
            original.FakeProperty = "some string value";

            var serializer = new BinaryFormatter();
            var stream = new MemoryStream();
            serializer.Serialize(stream, original);

            stream.Position = 0;
            var deserialized = (FakeSerializableNotifier)serializer.Deserialize(stream);

            original.ShouldNotBeSameAs(deserialized);
            original.FakeProperty.ShouldBe(deserialized.FakeProperty);
        }

        [Fact]
        public void can_notify_after_deserialization()
        {
            var original = new FakeSerializableNotifier();

            var serializer = new BinaryFormatter();
            var stream = new MemoryStream();
            serializer.Serialize(stream, original);

            stream.Position = 0;
            var deserialized = (FakeSerializableNotifier)serializer.Deserialize(stream);

            deserialized.FakeProperty = "some string value";
        }
    }
}