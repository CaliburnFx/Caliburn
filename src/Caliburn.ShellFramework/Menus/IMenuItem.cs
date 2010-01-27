namespace Caliburn.ShellFramework.Menus
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using PresentationFramework.ApplicationModel;

    public interface IMenuItem : IEnumerable<IMenuItem>, INotifyPropertyChanged
    {
        IMenuItem Parent { get; set; }

        MenuScope CreateScope(ILifecycleNotifier scope);

        void Add(params IMenuItem[] menuItems);
        void Insert(int index, IMenuItem item);
        bool Remove(IMenuItem item);
        int IndexOf(IMenuItem item);
    }
}