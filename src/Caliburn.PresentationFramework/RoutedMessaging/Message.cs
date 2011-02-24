namespace Caliburn.PresentationFramework.RoutedMessaging
{
    using System.Windows;
    using Parsers;

    /// <summary>
    /// Hosts attached properties related to routed UI messaging.
    /// </summary>
    public static class Message
    {
        static IRoutedMessageController controller;
        static IParser parser;

        /// <summary>
        /// A property definition representing a collection of triggers.
        /// </summary>
        public static readonly DependencyProperty TriggersProperty =
            DependencyProperty.RegisterAttached(
                "Triggers",
                typeof(RoutedMessageTriggerCollection),
                typeof(Message),
                new PropertyMetadata(OnTriggersChanged)
                );

        /// <summary>
        /// A property definition representing attached triggers and messages.
        /// </summary>
        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached(
                "Attach",
                typeof(string),
                typeof(Message),
                new PropertyMetadata(OnAttachChanged)
                );

        /// <summary>
        /// A property representing the availability effect of a given message.
        /// </summary>
        public static readonly DependencyProperty AvailabilityEffectProperty =
            DependencyProperty.RegisterAttached(
                "AvailabilityEffect",
                typeof(IAvailabilityEffect),
                typeof(Message),
                new PropertyMetadata(AvailabilityEffect.Disable)
                );

        /// <summary>
        /// Initializes the message property host.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="parser">The parser.</param>
        public static void Initialize(IRoutedMessageController controller, IParser parser)
        {
            Message.controller = controller;
            Message.parser = parser;
        }

        /// <summary>
        /// Sets the triggers.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="triggers">The triggers.</param>
        public static void SetTriggers(DependencyObject d, RoutedMessageTriggerCollection triggers)
        {
            d.SetValue(TriggersProperty, triggers);
        }

        /// <summary>
        /// Gets the triggers.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static RoutedMessageTriggerCollection GetTriggers(DependencyObject d)
        {
            return d.GetValue(TriggersProperty) as RoutedMessageTriggerCollection;
        }

        private static void OnTriggersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(controller == null) return;

            if(e.NewValue != e.OldValue)
            {
                var triggers = e.NewValue as RoutedMessageTriggerCollection;

                if(triggers != null)
                {
                    foreach(var messageTrigger in triggers)
                    {
                        controller.AttachTrigger(d, messageTrigger);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the attached triggers and messages.
        /// </summary>
        /// <param name="d">The element to attach to.</param>
        /// <param name="attachText">The parsable attachment text.</param>
        public static void SetAttach(DependencyObject d, string attachText)
        {
            d.SetValue(AttachProperty, attachText);
        }

        /// <summary>
        /// Gets the attached triggers and messages.
        /// </summary>
        /// <param name="d">The element that was attached to.</param>
        /// <returns>The parsable attachment text.</returns>
        public static string GetAttach(DependencyObject d)
        {
            return d.GetValue(AttachProperty) as string;
        }

        private static void OnAttachChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(controller == null) return;
            if(e.NewValue == e.OldValue) return;

            var attachText = e.NewValue as string;
            if(string.IsNullOrEmpty(attachText)) return;

            var triggers = new RoutedMessageTriggerCollection();

            foreach(var trigger in parser.Parse(d, attachText))
            {
                triggers.Add((BaseMessageTrigger)trigger);
            }

            SetTriggers(d, triggers);
        }

        /// <summary>
        /// Sets the availability effect.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="effect">The effect.</param>
        public static void SetAvailabilityEffect(DependencyObject d, IAvailabilityEffect effect)
        {
            d.SetValue(AvailabilityEffectProperty, effect);
        }

        /// <summary>
        /// Gets the availability effect.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static IAvailabilityEffect GetAvailabilityEffect(DependencyObject d)
        {
            return d.GetValue(AvailabilityEffectProperty) as IAvailabilityEffect;
        }
    }
}