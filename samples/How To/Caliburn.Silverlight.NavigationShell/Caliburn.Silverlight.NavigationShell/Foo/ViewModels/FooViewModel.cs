namespace Caliburn.Silverlight.NavigationShell.Foo.ViewModels
{
    using System.Collections.Generic;
    using PresentationFramework;
    using PresentationFramework.Screens;
    using ShellFramework.History;
    using ShellFramework.Results;

    [HistoryKey("Foo", typeof(FooViewModel))]
    public class FooViewModel : Screen
    {
        public IEnumerable<IResult> ClickMe()
        {
            yield return Show.MessageBox(
                "This is a message from me to you! It's happening asynchronously.  Take a look at the FooViewModel."
                );

            yield return Show.MessageBox(
                "This is another asynchronous message.  Pretty neat how you can write these synchronously."
                );

            yield return Show.MessageBox(
                "Just imagine how much easier it would be to handle web services and loaders with this technique."
                );
        }
    }
}