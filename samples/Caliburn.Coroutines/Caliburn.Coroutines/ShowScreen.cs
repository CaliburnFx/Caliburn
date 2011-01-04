namespace Caliburn.Coroutines {
    using System;
    using System.ComponentModel.Composition;
    using Core.InversionOfControl;
    using PresentationFramework.RoutedMessaging;

    public class ShowScreen : IResult {
        readonly string name;
        readonly Type screenType;

        public ShowScreen(string name) {
            this.name = name;
        }

        public ShowScreen(Type screenType) {
            this.screenType = screenType;
        }

        [Import]
        public IShell Shell { get; set; }

        public void Execute(ResultExecutionContext context) {
            var screen = !string.IsNullOrEmpty(name)
                ? context.ServiceLocator.GetInstance<object>(name)
                : context.ServiceLocator.GetInstance(screenType, null);

            Shell.ActivateItem(screen);
            Completed(this, new ResultCompletionEventArgs());
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

        public static ShowScreen Of<T>() {
            return new ShowScreen(typeof(T));
        }
    }
}