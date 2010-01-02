namespace Caliburn.PresentationFramework.ViewModels
{
    using System.Windows;

    public interface IViewApplicable
    {
        void ApplyTo(DependencyObject view);
    }
}