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
    }
}