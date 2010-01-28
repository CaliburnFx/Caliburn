namespace Caliburn.Silverlight.NavigationShell.Framework
{
    using System.Windows.Media.Imaging;

    public interface ITaskBarItem : IEntryPoint
    {
        BitmapImage Icon { get; }
        string DisplayName { get; }
    }
}