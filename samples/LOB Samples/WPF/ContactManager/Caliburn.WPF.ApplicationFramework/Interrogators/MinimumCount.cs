namespace Caliburn.WPF.ApplicationFramework.Interrogators
{
    using ModelFramework;

    public class MinimumCount<T> : IPropertyValidator<ICollectionNode<T>>
    {
        public MinimumCount(int minimum)
            : this("{0} must have at least {1} items.", minimum) {}

        public MinimumCount(string message, int count)
        {
            Message = message;
            Count = count;
        }

        public int Count { get; set; }
        public string Message { get; set; }

        public bool Interrogate(IProperty<ICollectionNode<T>> instance)
        {
            var value = instance.Value;

            if(value == null || value.Count < Count)
            {
                instance.ValidationResults.Add(
                    new ValidationResult(
                        string.Format(Count < 2 ? Message.Replace("s.", ".") : Message, instance.Definition.Name, Count),
                        instance
                        )
                    );
                return false;
            }

            return true;
        }
    }
}