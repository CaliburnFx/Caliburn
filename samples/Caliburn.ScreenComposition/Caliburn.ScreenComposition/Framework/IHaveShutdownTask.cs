namespace Caliburn.ScreenComposition.Framework {
    using PresentationFramework.RoutedMessaging;

    public interface IHaveShutdownTask {
        IResult GetShutdownTask();
    }
}