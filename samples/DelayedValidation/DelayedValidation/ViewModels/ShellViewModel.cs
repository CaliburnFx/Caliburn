namespace DelayedValidation.ViewModels {
    using Caliburn.Core.InversionOfControl;
    using Caliburn.PresentationFramework.RoutedMessaging;
    using Caliburn.PresentationFramework.Screens;
    using Caliburn.ShellFramework.Results;
    using Framework;

    [Singleton(typeof(IShell))]
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive, IShell {
        public ShellViewModel() {
            DisplayName = "Delayed Validation Sample";
        }

        public IResult AddContact() {
            return Show.Child<ContactViewModel>()
                .Configured(x => x.Edit());
        }
    }
}