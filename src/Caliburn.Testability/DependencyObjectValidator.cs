namespace Caliburn.Testability
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// Validates a <see cref="DependencyObjectElement"/>.
    /// </summary>
    public class DependencyObjectValidator
    {
        readonly ElementEnumeratorSettings settings;
        readonly DependencyObjectElement element;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyObjectValidator"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="element">The element.</param>
        public DependencyObjectValidator(ElementEnumeratorSettings settings, DependencyObjectElement element)
        {
            this.settings = settings;
            this.element = element;
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public ElementEnumeratorSettings Settings
        {
            get { return settings; }
        }

        /// <summary>
        /// Validates the bindings.
        /// </summary>
        /// <returns>The result of the validation process.</returns>
        public virtual ValidationResult ValidateBindings()
        {
            var elementResult = new ValidationResult();

            CheckAmbiguities(elementResult);

            if(element.Type == null)
            {
                elementResult.Add(
                    Error.BadDataContext(
                        element,
                        element.Type,
                        FrameworkElement.DataContextProperty
                        )
                    );
            }
            else
            {
                foreach(var info in GetBindings())
                {
                    if(info.Property.Name == "DataContext")
                    {
                        if(element.DataContextCheckType != null)
                        {
                            var validatedProperty = element.DataContextCheckType.ValidateAgainst(element,
                                                                                                  info.Property,
                                                                                                  info.Binding);
                            elementResult.Add(validatedProperty);
                        }
                    }
                    else
                    {
                        var validatedProperty = element.Type.ValidateAgainst(element, info.Property, info.Binding);
                        elementResult.Add(validatedProperty);
                    }
                }
            }

            return elementResult;
        }

        /// <summary>
        /// Gets the bindings.
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<BindingInfo> GetBindings()
        {
            var enumerator = element.Element.GetLocalValueEnumerator();

            while(enumerator.MoveNext())
            {
                var entry = enumerator.Current;

                if(!BindingOperations.IsDataBound(element.Element, entry.Property)) continue;

                var expression = entry.Value as BindingExpressionBase;

                foreach(var actualBinding in expression.GetActualBindings())
                {
                    yield return new BindingInfo(actualBinding, entry.Property);
                }
            }
        }

        void CheckAmbiguities(ValidationResult result)
        {
            if(element.Element is ContentControl)
            {
                var contentControl = (ContentControl)element.Element;

                if(contentControl.ContentTemplateSelector != null &&
                   contentControl.ContentTemplate != null)
                {
                    result.Add(
                        Error.TemplateSelectorAmbiguity(element, element.Type, "ContentTemplate")
                        );
                }

                if(element.Element is HeaderedContentControl)
                {
                    var headeredContentControl = (HeaderedContentControl)element.Element;

                    if(headeredContentControl.HeaderTemplateSelector != null &&
                       headeredContentControl.HeaderTemplate != null)
                    {
                        result.Add(
                            Error.TemplateSelectorAmbiguity(element, element.Type, "HeaderTemplate")
                            );
                    }
                }
            }
            else if(element.Element is ItemsControl)
            {
                var itemsControl = (ItemsControl)element.Element;

                if(itemsControl.ItemTemplateSelector != null &&
                   itemsControl.ItemTemplate != null)
                {
                    result.Add(
                        Error.TemplateSelectorAmbiguity(element, element.Type, "ItemTemplate")
                        );
                }

                if(itemsControl.ItemContainerStyle != null &&
                   itemsControl.ItemContainerStyleSelector != null)
                {
                    result.Add(
                        Error.StyleSelectorAmbiguity(element, element.Type, "ItemContainerStyle")
                        );
                }

                if(itemsControl.GroupStyle.Count > 0 &&
                   itemsControl.GroupStyleSelector != null)
                {
                    result.Add(
                        Error.StyleSelectorAmbiguity(element, element.Type, "GroupStyle")
                        );
                }

                if(element.Element is HeaderedItemsControl)
                {
                    var headeredItemsControl = (HeaderedItemsControl)element.Element;

                    if(headeredItemsControl.HeaderTemplateSelector != null &&
                       headeredItemsControl.HeaderTemplate != null)
                    {
                        result.Add(
                            Error.TemplateSelectorAmbiguity(element, element.Type, "HeaderTemplate")
                            );
                    }
                }
            }
        }
    }
}