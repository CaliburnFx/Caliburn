namespace GameLibrary.ViewModels {
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Caliburn.PresentationFramework.RoutedMessaging;
    using Caliburn.PresentationFramework.Screens;
    using Caliburn.ShellFramework.Results;
    using Framework;

    [Export(typeof(IShell))]
    public class ShellViewModel : Conductor<IScreen>, IShell {
        readonly SearchViewModel firstScreen;

        [ImportingConstructor]
        public ShellViewModel(SearchViewModel firstScreen) {
            this.firstScreen = firstScreen;
        }

        public IEnumerable<IResult> Back() {
            yield return Show.Child<SearchViewModel>();
        }

        protected override void OnInitialize() {
            ActivateItem(firstScreen);
            base.OnInitialize();
        }
    }
}