namespace Tests.Caliburn.Core
{
    using global::Caliburn.PresentationFramework;
    using Xunit;
    using Shouldly;

    
    public class A_bindable_collection
    {
        [Fact]
        public void does_not_fire_a_collectionchanged_event_for_each_item_in_addrange()
        {
            var collection = new BindableCollection<string>();
            var eventsFired = 0;
            collection.CollectionChanged += (source, args) => { eventsFired++; };

            collection.AddRange(new[]
            {
                "abc", "def"
            });

            eventsFired.ShouldBe(1, "The CollectionChanged event should only have fired once.");
        }

        [Fact]
        public void raises_a_collectionchanged_event_for_each_item_added()
        {
            var collection = new BindableCollection<string>();
            var eventsFired = 0;
            collection.CollectionChanged += (source, args) => { eventsFired++; };

            collection.Add("abc");
            collection.Add("def");

            eventsFired.ShouldBe(2,"The collection should have raised a CollectionChanged event twice.");
        }
    }
}