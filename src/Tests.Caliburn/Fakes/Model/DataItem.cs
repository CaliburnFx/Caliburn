namespace Tests.Caliburn.Fakes.Model
{
    using System.Collections.ObjectModel;

    public class DataItem
    {
        public DataItem(int id, string description, string @group)
        {
            Id = id;
            Description = description;
            Group = group;
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
    }

    public class DataItemsCollection : ObservableCollection<DataItem>
    {
        public DataItemsCollection()
        {
            Add(new DataItem(1, "Item 1", "Group 1"));
            Add(new DataItem(2, "Item 2", "Group 1"));
            Add(new DataItem(3, "Item 3", "Group 2"));
            Add(new DataItem(4, "Item 4", "Group 2"));
            Add(new DataItem(5, "Item 5", "Group 3"));
        }
    }

    public class MyDataSource
    {
        private readonly DataItemsCollection _items = new DataItemsCollection();

        public DataItemsCollection Items
        {
            get { return _items; }
        }
    }
}