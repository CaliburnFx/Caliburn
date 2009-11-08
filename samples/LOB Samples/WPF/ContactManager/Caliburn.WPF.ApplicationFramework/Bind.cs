namespace Caliburn.WPF.ApplicationFramework
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using Converters;
    using ModelFramework;
    using PresentationFramework.Metadata;

    public static class Bind
    {
        private static readonly CollapseWhenEmptyConverter _collapseWhenEmptyConverter =
            new CollapseWhenEmptyConverter();

        private static readonly ValidToBrushConverter _validToBrushConverter = 
            new ValidToBrushConverter();

        public static DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached(
                "Text",
                typeof(string),
                typeof(Bind),
                new PropertyMetadata(
                    (d, e) => SetBindings(
                                  d,
                                  e,
                                  TextBox.TextProperty,
                                  AlterBorderWhenIsValid()
                                  )
                    )
                );

        public static void SetText(DependencyObject d, string value)
        {
            d.SetValue(TextProperty, value);
        }

        public static string GetText(DependencyObject d, string value)
        {
            return (string)d.GetValue(TextProperty);
        }

        public static DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached(
                "ItemsSource",
                typeof(object),
                typeof(Bind),
                new PropertyMetadata(
                    (d, e) => SetBindings(
                                  d,
                                  e,
                                  ItemsControl.ItemsSourceProperty,
                                  AlterBorderWhenIsValid()
                                  )
                    )
                );

        public static void SetItemsSource(DependencyObject d, string value)
        {
            d.SetValue(ItemsSourceProperty, value);
        }

        public static object GetItemsSource(DependencyObject d, string value)
        {
            return d.GetValue(ItemsSourceProperty);
        }

        public static DependencyProperty SelectedItemProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItem",
                typeof(object),
                typeof(Bind),
                new PropertyMetadata(
                    (d, e) => SetBindings(
                                  d,
                                  e,
                                  Selector.SelectedItemProperty,
                                  AlterBorderWhenIsValid()
                                  )
                    )
                );

        public static void SetSelectedItem(DependencyObject d, string value)
        {
            d.SetValue(SelectedItemProperty, value);
        }

        public static object GetSelectedItem(DependencyObject d, string value)
        {
            return d.GetValue(SelectedItemProperty);
        }

        private static void SetBindings<T>(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e,
            DependencyProperty dependencyProperty,
            Action<T, IProperty> alter
            )
            where T : FrameworkElement
        {
            if(e.NewValue == e.OldValue || e.NewValue == null) return;

            var element = d as T;
            if(element == null) return;

            bool loaded = false;

            element.Loaded +=
                delegate{
                    if(loaded) return;
                    loaded = true;

                    var state = SetBindingForValue(element, dependencyProperty, e.NewValue.ToString());
                    if(alter != null) alter(element, state);
                    AddErrorToolTip(element, state);
                };
        }

        private static IProperty SetBindingForValue(FrameworkElement element, DependencyProperty dependencyProperty,
                                                    string modelProperty)
        {
            var model = (IModel)element.DataContext;
            var state = model[modelProperty];

            state.SetView(element, null, true);

            element.SetBinding(
                dependencyProperty,
                new Binding("Value")
                {
                    Source = state,
                    Mode = BindingMode.TwoWay
                });

            return state;
        }

        private static void AddErrorToolTip(DependencyObject dependencyObject, IProperty state)
        {
            var itemsControl = new ItemsControl();

            itemsControl.SetBinding(
                ItemsControl.ItemsSourceProperty,
                new Binding("ValidationResults")
                );

            itemsControl.DisplayMemberPath = "Message";

            var tooltip = new ToolTip
            {
                DataContext = state,
                Content = itemsControl,
            };

            tooltip.SetBinding(
                UIElement.VisibilityProperty,
                new Binding("ValidationResults.Count")
                {
                    Converter = _collapseWhenEmptyConverter
                });

            ToolTipService.SetToolTip(dependencyObject, tooltip);
        }

        private static Action<Control, IProperty> AlterBorderWhenIsValid()
        {
            return (control, state) => control.SetBinding(
                                           Control.BorderBrushProperty,
                                           new Binding("IsValid")
                                           {
                                               Source = state,
                                               Converter = _validToBrushConverter
                                           });
        }
    }
}