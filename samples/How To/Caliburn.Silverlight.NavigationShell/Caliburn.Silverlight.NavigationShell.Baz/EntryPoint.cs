namespace Caliburn.Silverlight.NavigationShell.Baz
{
    using System.Collections.Generic;
    using Framework;
    using PresentationFramework;
    using ShellFramework.Results;
    using ViewModels;

    public class EntryPoint : IEntryPoint
    {
        public IEnumerable<IResult> Enter()
        {
            yield return Show.Child<BazViewModel>()
                .In<IShell>();
        }
    }
}