namespace Caliburn.Silverlight.NavigationShell.Framework
{
    using System.Collections.Generic;
    using PresentationFramework;

    public interface IEntryPoint
    {
        IEnumerable<IResult> Enter();
    }
}