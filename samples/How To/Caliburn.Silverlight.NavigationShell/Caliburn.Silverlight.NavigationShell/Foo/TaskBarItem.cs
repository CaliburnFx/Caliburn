namespace Caliburn.Silverlight.NavigationShell.Foo
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows.Media.Imaging;
    using Framework;
    using PresentationFramework.RoutedMessaging;
    using ShellFramework.Resources;
    using ShellFramework.Results;
    using ViewModels;

    public class TaskBarItem : ITaskBarItem
    {
        private readonly IResourceManager _resourceManager;

        public TaskBarItem(IResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        public BitmapImage Icon
        {
            get { return _resourceManager.GetBitmap("Foo/Resources/FooIcon.png", Assembly.GetExecutingAssembly().GetAssemblyName()); }
        }

        public string DisplayName
        {
            get { return "Foo"; }
        }

        public IEnumerable<IResult> Enter()
        {
            yield return Show.Child<FooViewModel>()
                .In<IShell>();
        }
    }
}