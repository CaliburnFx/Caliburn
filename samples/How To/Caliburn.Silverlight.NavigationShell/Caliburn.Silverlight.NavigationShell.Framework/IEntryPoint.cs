namespace Caliburn.Silverlight.NavigationShell.Framework
{
    using System.Collections.Generic;
    using PresentationFramework.RoutedMessaging;

    public interface IEntryPoint
    {
        IEnumerable<IResult> Enter();
    }
}