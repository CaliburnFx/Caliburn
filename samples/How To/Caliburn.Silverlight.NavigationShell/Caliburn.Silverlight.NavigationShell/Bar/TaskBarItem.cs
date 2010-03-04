namespace Caliburn.Silverlight.NavigationShell.Bar
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows.Media.Imaging;
    using Framework;
    using PresentationFramework.RoutedMessaging;
    using ShellFramework.Resources;
    using ShellFramework.Results;

    public class TaskBarItem : ITaskBarItem
    {
        private readonly IResourceManager _resourceManager;

        public TaskBarItem(IResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        public BitmapImage Icon
        {
            get { return _resourceManager.GetBitmap("Bar/Resources/BarIcon.png", Assembly.GetExecutingAssembly().GetAssemblyName()); }
        }

        public string DisplayName
        {
            get { return "Bar"; }
        }

        public IEnumerable<IResult> Enter()
        {
            yield return Show.ChildFor(new ViewModels.Message {Text = "This is a subject."})
                .In<IShell>();
        }
    }
}