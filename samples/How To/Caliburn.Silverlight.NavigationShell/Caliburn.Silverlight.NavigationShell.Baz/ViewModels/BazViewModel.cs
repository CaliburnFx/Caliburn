namespace Caliburn.Silverlight.NavigationShell.Baz.ViewModels
{
    using System.Collections.Generic;
    using System.Windows.Controls;
    using PresentationFramework;
    using PresentationFramework.Screens;
    using ShellFramework.History;
    using ShellFramework.Results;

    [HistoryKey("Baz", typeof(BazViewModel))]
    public class BazViewModel : Screen
    {
        public IEnumerable<IResult> Save()
        {
            var dialog = new SaveFileDialog();
            yield return Show.Dialog(dialog);

            yield return Show.MessageBox("You saved " + dialog.SafeFileName + ".", "File");
        }

        public IEnumerable<IResult> Open()
        {
            var dialog = new OpenFileDialog();
            yield return Show.Dialog(dialog);

            yield return Show.MessageBox("You opened " + dialog.File.Name + ".", "File");
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