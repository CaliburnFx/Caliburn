namespace Caliburn.ShellFramework.Menus
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using PresentationFramework.ApplicationModel;

    public interface IMenu : IEnumerable<IMenu>, INotifyPropertyChanged
    {
        string DisplayName { get; }
        IMenu Parent { get; set; }

        MenuScope CreateScope(ILifecycleNotifier scope);

        void Add(params IMenu[] menuItems);
        void Insert(int index, IMenu item);
        bool Remove(IMenu item);
        int IndexOf(IMenu item);
    }
}