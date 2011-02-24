#if SILVERLIGHT

namespace Caliburn.PresentationFramework.RoutedMessaging.Triggers
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using RoutedMessaging;

    /// <summary>
    /// An implentation of <see cref="IMessageTrigger"/> that enables triggers based on an input gesture.
    /// </summary>
    public class GestureMessageTrigger : BaseMessageTrigger
    {
        internal static TimeSpan DoubleClickInterval = TimeSpan.FromMilliseconds(200);
        DateTime lastClick = DateTime.MinValue;

        /// <summary>
        /// A dependency property representing the key's value.
        /// </summary>
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register(
                "Key",
                typeof(Key),
                typeof(GestureMessageTrigger),
                new PropertyMetadata(Key.None)
                );

        /// <summary>
        /// A dependency property representing the modifier's value.
        /// </summary>
        public static readonly DependencyProperty ModifiersProperty =
            DependencyProperty.Register(
                "Modifiers",
                typeof(ModifierKeys),
                typeof(GestureMessageTrigger),
                new PropertyMetadata(ModifierKeys.None)
                );

        /// <summary>
        /// A dependency property representing the mouse action's value.
        /// </summary>
        public static readonly DependencyProperty MouseActionProperty =
            DependencyProperty.Register(
                "MouseAction",
                typeof(MouseAction),
                typeof(GestureMessageTrigger),
                new PropertyMetadata(MouseAction.None)
                );

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public Key Key
        {
            get { return (Key)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the modifiers.
        /// </summary>
        /// <value>The modifiers.</value>
        public ModifierKeys Modifiers
        {
            get { return (ModifierKeys)GetValue(ModifiersProperty); }
            set { SetValue(ModifiersProperty, value); }
        }

        /// <summary>
        /// Gets or sets the mouse action.
        /// </summary>
        /// <value>The mouse action.</value>
        public MouseAction MouseAction
        {
            get { return (MouseAction)GetValue(MouseActionProperty); }
            set { SetValue(MouseActionProperty, value); }
        }

        /// <summary>
        /// Wires the trigger into the interactin hierarchy.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Attach(IInteractionNode node)
        {
            var element = (UIElement)node.UIElement;

            if (Key != Key.None)
                element.KeyUp += element_KeyUp;
            else if (MouseAction == MouseAction.LeftClick)
                element.MouseLeftButtonUp += element_LeftClick;
            else if (MouseAction == MouseAction.LeftDoubleClick)
                element.MouseLeftButtonDown += element_LeftDoubleClick;
#if SILVERLIGHT_40
            else if (MouseAction == MouseAction.RightClick)
                element.MouseRightButtonUp += element_RightClick;
            else if (MouseAction == MouseAction.RightDoubleClick)
                element.MouseRightButtonDown += element_RightDoubleClick;
#endif

            base.Attach(node);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "GestureMessageTrigger(" + Modifiers + ", " + Key + ", " + MouseAction  + ")";
        }

        void element_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key && Keyboard.Modifiers == Modifiers)
            {
                Node.ProcessMessage(Message, e);
            }
        }

        void element_LeftClick(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == Modifiers)
            {
                Node.ProcessMessage(Message, e);
            }
        }

        void element_LeftDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var difference = DateTime.Now - lastClick;

            if(difference <= DoubleClickInterval)
            {
                lastClick = DateTime.MinValue;

                if (Keyboard.Modifiers == Modifiers)
                {
                    Node.ProcessMessage(Message, e);
                }
            }
            else lastClick = DateTime.Now;
        }

#if SILVERLIGHT_40

        void element_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == Modifiers)
            {
                Node.ProcessMessage(Message, e);
            }
        }

        void element_RightDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var difference = DateTime.Now - lastClick;

            if(difference <= DoubleClickInterval)
            {
                lastClick = DateTime.MinValue;

                if (Keyboard.Modifiers == Modifiers)
                {
                    Node.ProcessMessage(Message, e);
                }
            }
            else lastClick = DateTime.Now;

            e.Handled = true;
        }

#endif
    }
}

#endif