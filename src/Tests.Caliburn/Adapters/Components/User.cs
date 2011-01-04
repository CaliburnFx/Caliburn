namespace Tests.Caliburn.Adapters.Components
{
    public class User
    {
        public User(Component comp)
        {
            Comp = comp;
        }

        public Component Comp { get; set; }
    }
}