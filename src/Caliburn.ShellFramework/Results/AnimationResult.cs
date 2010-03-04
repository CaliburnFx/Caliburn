namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;
    using PresentationFramework.RoutedMessaging;

    public class AnimationResult : IResult
    {
        public enum AnimationAction
        {
            Begin,
            Pause,
            Resume,
            Stop
        }

        private bool _wait;
        private readonly string _key;
        private readonly AnimationAction _action;

        public AnimationResult(string key, AnimationAction action)
        {
            _key = key;
            _action = action;
        }

        public string Key
        {
            get { return _key; }
        }

        public AnimationAction Action
        {
            get { return _action; }
        }

        public AnimationResult Wait()
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

            var storyboard = (Storyboard)element.Resources[_key];

            if(_wait)
                storyboard.Completed += delegate { Completed(this, new ResultCompletionEventArgs()); };

            switch(_action)
            {
                case AnimationAction.Begin:
                    storyboard.Begin();
                    break;
                case AnimationAction.Pause:
                    storyboard.Pause();
                    break;
                case AnimationAction.Resume:
                    storyboard.Resume();
                    break;
                case AnimationAction.Stop:
                    storyboard.Stop();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if(!_wait)
                Completed(this, new ResultCompletionEventArgs());
        }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}