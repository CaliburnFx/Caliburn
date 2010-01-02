namespace Caliburn.PresentationFramework.Conventions
{
    using System.Windows;

    public interface IViewApplicable
    {
        void ApplyTo(DependencyObject view);
    }
}