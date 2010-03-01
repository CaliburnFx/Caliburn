namespace Tests.Caliburn.Core
{
    using global::Caliburn.PresentationFramework;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class A_bindable_collection
    {
        [Test]
        public void does_not_fire_a_collectionchanged_event_for_each_item_in_addrange()
        {
            var collection = new BindableCollection<string>();
            var eventsFired = 0;
            collection.CollectionChanged += (source, args) => { eventsFired++; };

            collection.AddRange(new[]
            {
                "abc", "def"
            });

            Assert.That(eventsFired, Is.EqualTo(1), "The CollectionChanged event should only have fired once.");
        }

        [Test]
        public void raises_a_collectionchanged_event_for_each_item_added()
        {
            var collection = new BindableCollection<string>();
            var eventsFired = 0;
            collection.CollectionChanged += (source, args) => { eventsFired++; };

            collection.Add("abc");
            collection.Add("def");

            Assert.That(eventsFired, Is.EqualTo(2), "The collection should have raised a CollectionChanged event twice.");
        }
    }
}