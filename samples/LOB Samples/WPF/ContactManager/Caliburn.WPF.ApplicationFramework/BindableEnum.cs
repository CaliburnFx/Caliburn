namespace Caliburn.WPF.ApplicationFramework
{
    public class BindableEnum
    {
        public int UnderlyingValue { get; set; }
        public string DisplayName { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }

        public override bool Equals(object obj)
        {
            var otherBindable = obj as BindableEnum;

            if(otherBindable != null) 
                return UnderlyingValue == otherBindable.UnderlyingValue;

            if(obj is int)
                return UnderlyingValue.Equals((int)obj);

            return false;
        }

        public override int GetHashCode()
        {
            return UnderlyingValue.GetHashCode();
        }
    }
}