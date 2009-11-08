namespace Caliburn.WPF.ApplicationFramework.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;

    public class CrossFade : ITransition
    {
        private Duration _fadeLength = new Duration(TimeSpan.FromSeconds(.5));

        public bool RequiresNewContentTopmost
        {
            get { return false; }
        }

        public Duration FadeLength
        {
            get { return _fadeLength; }
            set { _fadeLength = value; }
        }

        public void BeginTransition(TransitionPresenter transitionElement, UIElement oldContent, UIElement newContent)
        {
            var sb = new Storyboard();
            var animation = CreateFadeOutAnimation(oldContent);

            sb.Children.Add(animation);
            sb.Duration = FadeLength;

            sb.Completed +=
                (s, e) =>{
                    sb.Stop();
                    transitionElement.TransitionEnded(this, oldContent, newContent);
                };

            sb.Begin();
        }

        private DoubleAnimation CreateFadeOutAnimation(DependencyObject target)
        {
            var animation = new DoubleAnimation();

            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.Opacity)"));

            animation.To = 0.0;
            animation.Duration = FadeLength;

            return animation;
        }
    }
}