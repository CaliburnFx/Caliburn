namespace Caliburn.ShellFramework.Menus
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using PresentationFramework.Screens;

    public interface IMenu : IList<IMenu>, IChild<IMenu>, IHaveDisplayName, INotifyPropertyChanged
    {
    }
}