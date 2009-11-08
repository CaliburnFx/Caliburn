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
        private readonly ElementEnumeratorSettings _settings;
        private readonly DependencyObjectElement _element;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyObjectValidator"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="element">The element.</param>
        public DependencyObjectValidator(ElementEnumeratorSettings settings, DependencyObjectElement element)
        {
            _settings = settings;
            _element = element;
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public ElementEnumeratorSettings Settings
        {
            get { return _settings; }
        }

        /// <summary>
        /// Validates the bindings.
        /// </summary>
        /// <returns>The result of the validation process.</returns>
        public virtual ValidationResult ValidateBindings()
        {
            var elementResult = new ValidationResult();

            CheckAmbiguities(elementResult);

            if(_element.Type == null)
            {
                elementResult.Add(
                    Error.BadDataContext(
                        _element,
                        _element.Type,
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
                        if(_element.DataContextCheckType != null)
                        {
                            var validatedProperty = _element.DataContextCheckType.ValidateAgainst(_element,
                                                                                                  info.Property,
                                                                                                  info.Binding);
                            elementResult.Add(validatedProperty);
                        }
                    }
                    else
                    {
                        var validatedProperty = _element.Type.ValidateAgainst(_element, info.Property, info.Binding);
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
            var enumerator = _element.Element.GetLocalValueEnumerator();

            while(enumerator.MoveNext())
            {
                var entry = enumerator.Current;

                if(!BindingOperations.IsDataBound(_element.Element, entry.Property)) continue;

                var expression = entry.Value as BindingExpressionBase;

                foreach(var actualBinding in expression.GetActualBindings())
                {
                    yield return new BindingInfo(actualBinding, entry.Property);
                }
            }
        }

        private void CheckAmbiguities(ValidationResult result)
        {
            if(_element.Element is ContentControl)
            {
                var contentControl = (ContentControl)_element.Element;

                if(contentControl.ContentTemplateSelector != null &&
                   contentControl.ContentTemplate != null)
                {
                    result.Add(
                        Error.TemplateSelectorAmbiguity(_element, _element.Type, "ContentTemplate")
                        );
                }

                if(_element.Element is HeaderedContentControl)
                {
                    var headeredContentControl = (HeaderedContentControl)_element.Element;

                    if(headeredContentControl.HeaderTemplateSelector != null &&
                       headeredContentControl.HeaderTemplate != null)
                    {
                        result.Add(
                            Error.TemplateSelectorAmbiguity(_element, _element.Type, "HeaderTemplate")
                            );
                    }
                }
            }
            else if(_element.Element is ItemsControl)
            {
                var itemsControl = (ItemsControl)_element.Element;

                if(itemsControl.ItemTemplateSelector != null &&
                   itemsControl.ItemTemplate != null)
                {
                    result.Add(
                        Error.TemplateSelectorAmbiguity(_element, _element.Type, "ItemTemplate")
                        );
                }

                if(itemsControl.ItemContainerStyle != null &&
                   itemsControl.ItemContainerStyleSelector != null)
                {
                    result.Add(
                        Error.StyleSelectorAmbiguity(_element, _element.Type, "ItemContainerStyle")
                        );
                }

                if(itemsControl.GroupStyle.Count > 0 &&
                   itemsControl.GroupStyleSelector != null)
                {
                    result.Add(
                        Error.StyleSelectorAmbiguity(_element, _element.Type, "GroupStyle")
                        );
                }

                if(_element.Element is HeaderedItemsControl)
                {
                    var headeredItemsControl = (HeaderedItemsControl)_element.Element;

                    if(headeredItemsControl.HeaderTemplateSelector != null &&
                       headeredItemsControl.HeaderTemplate != null)
                    {
                        result.Add(
                            Error.TemplateSelectorAmbiguity(_element, _element.Type, "HeaderTemplate")
                            );
                    }
                }
            }
        }
    }
}