namespace Caliburn.WPF.ApplicationFramework.Controls
{
    using System.Windows;

    public interface ITransition
    {
        bool RequiresNewContentTopmost { get; }
        void BeginTransition(TransitionPresenter transitionElement, UIElement oldContent, UIElement newContent);
    }
}