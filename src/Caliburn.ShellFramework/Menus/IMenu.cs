namespace Caliburn.ShellFramework.Menus
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using PresentationFramework.Screens;

    /// <summary>
    /// Represents the model of a menu.
    /// </summary>
    public interface IMenu : IList<IMenu>, IChild<IMenu>, IHaveDisplayName, INotifyPropertyChanged
    {
    }
}