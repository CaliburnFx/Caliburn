namespace Caliburn.WPF.ApplicationFramework.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    [ContentProperty("Content")]
    public class TransitionPresenter : Grid
    {
        public event EventHandler TransitionCompleted = delegate { };

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                "Content",
                typeof(object),
                typeof(TransitionPresenter),
                new PropertyMetadata(ContentChanged)
                );

        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(
                "ContentTemplate",
                typeof(DataTemplate),
                typeof(TransitionPresenter),
                null
                );

        public static readonly DependencyProperty TransitionProperty =
            DependencyProperty.Register(
                "Transition",
                typeof(ITransition),
                typeof(TransitionPresenter),
                new PropertyMetadata(new CrossFade())
                );

        public object Content
        {
            get { return GetValue(ContentProperty) as UIElement; }
            set { SetValue(ContentProperty, value); }
        }

        public DataTemplate ContentTemplate
        {
            get { return GetValue(ContentTemplateProperty) as DataTemplate; }
            set { SetValue(ContentTemplateProperty, value); }
        }

        public ITransition Transition
        {
            get { return GetValue(TransitionProperty) as ITransition; }
            set { SetValue(TransitionProperty, value); }
        }

        private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var presenter = (TransitionPresenter)d;
            presenter.DoTransition(e.NewValue, new CrossFade());
        }

        public void DoTransition(object newContent, ITransition transition)
        {
            if(newContent == null)
            {
                Children.Clear();
                return;
            }

            UIElement view;

            if(ContentTemplate != null)
            {
                var template = ContentTemplate.LoadContent();
                var fe = template as FrameworkElement;

                if(fe != null)
                    fe.DataContext = newContent;

                view = template as UIElement;
            }
            else view = newContent as UIElement;

            if(view == null) return;
            if(Children.Contains(view)) return;

            if(Children.Count > 0)
            {
                if(transition.RequiresNewContentTopmost)
                {
                    Children.Add(view);
                    transition.BeginTransition(this, Children[0], view);
                }
                else
                {
                    Children.Insert(0, view);
                    transition.BeginTransition(this, Children[1], view);
                }
            }
            else
            {
                Children.Add(view);
                TransitionEnded(transition, null, view);
            }
        }

        public void TransitionEnded(ITransition transition, UIElement oldContent, UIElement newContent)
        {
            if(oldContent != null) Children.Remove(oldContent);
            TransitionCompleted(this, EventArgs.Empty);
        }
    }
}