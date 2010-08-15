using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.Caliburn.MVP.PropertyChanged
{
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using global::Caliburn.PresentationFramework;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.IO;

	[TestFixture]
	public class A_PropertyChangedBase_subclass : TestBase
	{
		 

		[Test]
		public void can_be_serialized_and_deserialized() {
		 	var original = new FakeNotifier();
			original.FakeProperty = "some string value";
			
			var serializer = new BinaryFormatter();
			var stream = new MemoryStream();
			serializer.Serialize(stream, original);

			stream.Position=0;
			var deserialized = (FakeNotifier)serializer.Deserialize(stream);

			Assert.AreNotSame(original, deserialized);
			Assert.AreEqual(original.FakeProperty, deserialized.FakeProperty); 
		}
		[Test]
		public void can_notify_after_deserialization()
		{
			var original = new FakeNotifier();
			
			var serializer = new BinaryFormatter();
			var stream = new MemoryStream();
			serializer.Serialize(stream, original);

			stream.Position = 0;
			var deserialized = (FakeNotifier)serializer.Deserialize(stream);

			deserialized.FakeProperty = "some string value";
		}
		[Test]
		public void can_accept_handlers_after_deserialization()
		{
			var original = new FakeNotifier();

			var serializer = new BinaryFormatter();
			var stream = new MemoryStream();
			serializer.Serialize(stream, original);

			stream.Position = 0;
			var deserialized = (FakeNotifier)serializer.Deserialize(stream);

			int handlerCalls = 0;
			deserialized.PropertyChanged += (o, e) => { handlerCalls++; };
			deserialized.FakeProperty = "some string value";

			Assert.AreEqual(1, handlerCalls);
		}


		[Serializable]
		public class FakeNotifier : PropertyChangedBase
		{
			string _FakeProperty;
			public string FakeProperty {
				get { return _FakeProperty; }
				set { _FakeProperty = value; NotifyOfPropertyChange(() => FakeProperty); }
			}
		}
	}
}
