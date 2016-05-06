namespace Caliburn.ShellFramework.Results
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;
    using PresentationFramework.RoutedMessaging;

    /// <summary>
    /// An <see cref="IResult"/> that controls animation in the view.
    /// </summary>
    public class AnimationResult : IResult
    {
        /// <summary>
        /// The action to perform on an animation.
        /// </summary>
        public enum AnimationAction
        {
            /// <summary>
            /// Begin the animation.
            /// </summary>
            Begin,
            /// <summary>
            /// Pause the animation.
            /// </summary>
            Pause,
            /// <summary>
            /// Resume the animation.
            /// </summary>
            Resume,
            /// <summary>
            /// Stop the animation.
            /// </summary>
            Stop
        }

        bool wait;
        readonly string key;
        readonly AnimationAction action;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationResult"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="action">The action.</param>
        public AnimationResult(string key, AnimationAction action)
        {
            this.key = key;
            this.action = action;
        }

        /// <summary>
        /// Gets the animation's resource key.
        /// </summary>
        /// <value>The key.</value>
        public string Key
        {
            get { return key; }
        }

        /// <summary>
        /// Gets the action to perform on the animation.
        /// </summary>
        /// <value>The action.</value>
        public AnimationAction Action
        {
            get { return action; }
        }

        /// <summary>
        /// Causes the completed event to fire after the animation completes.
        /// </summary>
        /// <returns></returns>
        public AnimationResult Wait()
        {
            wait = true;
            return this;
        }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
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

            var storyboard = (Storyboard)element.Resources[key];

            if(wait)
            {
                EventHandler handler = null;
                handler = delegate{
                    storyboard.Completed -= handler;
                    Completed(this, new ResultCompletionEventArgs());
                };
                storyboard.Completed += handler;
            }

            switch(action)
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

            if(!wait)
                Completed(this, new ResultCompletionEventArgs());
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}