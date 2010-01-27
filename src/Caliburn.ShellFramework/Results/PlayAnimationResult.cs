namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;
    using PresentationFramework;

    public class PlayAnimationResult : IResult
    {
        private bool _wait;
        private readonly string _animationKey;

        public PlayAnimationResult(string animationKey)
        {
            _animationKey = animationKey;
        }

        public PlayAnimationResult Wait()
        {
            _wait = true;
            return this;
        }

        public void Execute(ResultExecutionContext context)
        {
            FrameworkElement element = null;

            if(context.HandlingNode != null)
                element = context.HandlingNode.UIElement as FrameworkElement;

            if(element == null && context.Message != null)
                element = context.Message.Source.UIElement as FrameworkElement;

            if(element == null)
            {
                Completed(this, new ResultCompletionEventArgs());
                return;
            }

            var storyboard = (Storyboard)element.Resources[_animationKey];

            if(_wait)
                storyboard.Completed += delegate { Completed(this, new ResultCompletionEventArgs()); };

            storyboard.Begin();

            if(!_wait)
                Completed(this, new ResultCompletionEventArgs());
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}