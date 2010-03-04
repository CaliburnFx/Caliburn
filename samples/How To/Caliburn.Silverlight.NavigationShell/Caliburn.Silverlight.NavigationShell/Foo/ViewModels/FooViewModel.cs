namespace Caliburn.Silverlight.NavigationShell.Foo.ViewModels
{
    using System.Collections.Generic;
    using PresentationFramework.RoutedMessaging;
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

        //The following overrides insure that all instances of this screen are treated as
        //equal by the screen activation mechanism without forcing a singleton registration
        //in the container.
        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == GetType();
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }
    }
}