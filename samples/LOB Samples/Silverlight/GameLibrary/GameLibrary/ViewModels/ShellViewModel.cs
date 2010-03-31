namespace GameLibrary.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Caliburn.PresentationFramework.RoutedMessaging;
    using Caliburn.PresentationFramework.Screens;
    using Caliburn.ShellFramework.Results;
    using Framework;

    [Export(typeof(IShell))]
    public class ShellViewModel : ScreenConductor<IScreen>, IShell
    {
        private readonly SearchViewModel _firstScreen;

        [ImportingConstructor]
        public ShellViewModel(SearchViewModel firstScreen)
        {
            _firstScreen = firstScreen;
        }

        public IEnumerable<IResult> Back()
        {
            yield return Show.Child<SearchViewModel>().In<IShell>();
        }

        protected override void OnInitialize()
        {
            this.OpenScreen(_firstScreen);
        }
    }
}