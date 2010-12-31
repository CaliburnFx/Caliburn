namespace Caliburn.Testability
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// Represents a data bound element.
    /// </summary>
    public class DependencyObjectElement : IBoundElement
    {
        readonly DependencyObject element;
        readonly BoundType type;
        readonly BoundType dataContextCheckType;
        bool checkLogicalChildren = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyObjectElement"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="boundType">Type of the bound.</param>
        /// <param name="baseName">Name of the base.</param>
        internal DependencyObjectElement(DependencyObject element, BoundType boundType, string baseName)
        {
            this.element = element;
            type = EnsureBoundType(boundType);

            if(type != boundType && type != null)
                dataContextCheckType = boundType;

            BaseName = baseName ?? string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyObjectElement"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="boundType">Type of the bound.</param>
        internal DependencyObjectElement(DependencyObject element, BoundType boundType)
            : this(element, boundType, string.Empty) {}

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name
        {
            get
            {
                var frameworkElement = Element as FrameworkElement;

                if(frameworkElement != null)
                {
                    var name = frameworkElement.GetValue(FrameworkElement.NameProperty) as string;

                    if(!string.IsNullOrEmpty(name))
                        return BaseName + Element.GetType().Name + ": " + name;
                }

                return BaseName + Element.GetType().Name;
            }
        }

        /// <summary>
        /// Gets or sets the base name used to generate the name.
        /// </summary>
        /// <value>The base name.</value>
        public string BaseName { get; protected set; }

        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value>The element.</value>
        public DependencyObject Element
        {
            get { return element; }
        }

        /// <summary>
        /// Gets the type the item is bound to.
        /// </summary>
        /// <value>The type.</value>
        public BoundType Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the type of the data context check.
        /// </summary>
        /// <value>The type of the data context check.</value>
        public BoundType DataContextCheckType
        {
            get { return dataContextCheckType; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to check child elements.
        /// </summary>
        /// <value><c>true</c> to check children; otherwise, <c>false</c>.</value>
        public bool CheckLogicalChildren
        {
            get { return checkLogicalChildren; }
            set { checkLogicalChildren = value; }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public IEnumerable<IElement> GetChildren(ElementEnumeratorSettings settings)
        {
            if(Type == null) yield break;

            var checkedProperties = new List<string>();

            foreach(var item in CheckContentControl(checkedProperties, settings))
            {
                yield return item;
            }

            foreach(var item in CheckItemsControl(checkedProperties, settings))
            {
                yield return item;
            }

            var logicalChildren = LogicalTreeHelper.GetChildren(Element).Cast<object>().ToList();
            var enumerator = element.GetLocalValueEnumerator();

            while(enumerator.MoveNext())
            {
                var entry = enumerator.Current;

                if(BindingOperations.IsDataBound(element, entry.Property)) continue;
                if(checkedProperties.Contains(entry.Property.Name)) continue;
                if(logicalChildren.Contains(entry.Value)) continue;

                var dataBound = settings.GetBoundProperty(entry.Value, Type, Name);

                if(dataBound != null)
                    yield return dataBound;
            }

            if(!CheckLogicalChildren) yield break;

            foreach(var child in logicalChildren)
            {
                if(child is DependencyObject)
                {
                    yield return Bound.DependencyObject(
                        child as DependencyObject,
                        Type, Name + ".",
                        (settings.TraverseUserControls || !(child is UserControl))
                        );
                }
            }
        }

        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public virtual void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Ensures that the type is accurately represented.
        /// </summary>
        /// <param name="boundType">Type of the bound.</param>
        /// <returns></returns>
        protected BoundType EnsureBoundType(BoundType boundType)
        {
            var frameworkElement = element as FrameworkElement;

            if(frameworkElement != null)
            {
                var binding = BindingOperations.GetBinding(
                    element,
                    FrameworkElement.DataContextProperty
                    );

                if(binding.ShouldValidate())
                {
                    var propertyPath = binding.Path.Path;
                    return boundType.GetAssociatedType(propertyPath);
                }
            }

            return boundType;
        }

        IEnumerable<IElement> CheckContentControl(ICollection<string> checkedProperties, ElementEnumeratorSettings settings)
        {
            if(!settings.IncludeTemplates) yield break;

            var contentControl = Element as ContentControl;

            if(contentControl != null)
            {
                checkedProperties.Add(ContentControl.ContentTemplateProperty.Name);

                var template = contentControl.ContentTemplate;

                if(template != null)
                {
                    var dataType = template.DataType as Type;

                    if(dataType == null)
                    {
                        var binding = BindingOperations.GetBinding(
                            contentControl,
                            ContentControl.ContentProperty
                            );

                        if(binding != null)
                            yield return Bound.DataTemplate(template, Type.GetAssociatedType(binding.Path.Path), Name);
                    }
                    else yield return Bound.DataTemplate(template, new BoundType(dataType), Name);
                }

                foreach(var item in CheckHeaderedContentControl(checkedProperties))
                {
                    yield return item;
                }
            }
        }

        IEnumerable<IElement> CheckHeaderedContentControl(ICollection<string> checkedProperties)
        {
            var headeredContentControl = Element as HeaderedContentControl;

            if(headeredContentControl != null)
            {
                checkedProperties.Add(HeaderedContentControl.HeaderTemplateProperty.Name);

                var template = headeredContentControl.HeaderTemplate;

                if(template != null)
                {
                    var dataType = template.DataType as Type;

                    if(dataType == null)
                    {
                        var binding = BindingOperations.GetBinding(
                            headeredContentControl,
                            HeaderedContentControl.HeaderProperty
                            );

                        if(binding != null)
                            yield return Bound.DataTemplate(template, Type.GetAssociatedType(binding.Path.Path), Name);
                    }
                    else yield return Bound.DataTemplate(template, new BoundType(dataType), Name);
                }
            }
        }

        IEnumerable<IElement> CheckItemsControl(ICollection<string> checkedProperties, ElementEnumeratorSettings settings)
        {
            var itemsControl = Element as ItemsControl;

            if(itemsControl != null)
            {
                checkedProperties.Add(ItemsControl.ItemTemplateProperty.Name);

                var template = itemsControl.ItemTemplate;

                if(template != null && settings.IncludeTemplates)
                {
                    var dataType = template.DataType as Type;

                    if(dataType == null)
                    {
                        var binding = BindingOperations.GetBinding(
                            itemsControl,
                            ItemsControl.ItemsSourceProperty
                            );

                        if(binding != null)
                        {
                            var enumerableType = Type.GetPropertyType(binding.Path.Path);

							if (enumerableType != null)
                            {   
                                var generics = enumerableType.GetGenericArguments();

                                if(generics != null && generics.Length == 1)
                                    yield return Bound.DataTemplate(template, new BoundType(generics[0]), Name);
                            }
                        }
                    }
                    else yield return Bound.DataTemplate(template, new BoundType(dataType), Name);
                }

                if(settings.IncludeStyles)
                {
                    if(itemsControl.ItemContainerStyle != null)
                    {
                        checkedProperties.Add(ItemsControl.ItemContainerStyleProperty.Name);
                        yield return Bound.Style(itemsControl.ItemContainerStyle, Type, Name);
                    }

                    foreach(var groupStyle in itemsControl.GroupStyle)
                    {
                        checkedProperties.Add("GroupStyle");
                        yield return Bound.GroupStyle(groupStyle, Type, Name);
                    }
                }

                foreach(var item in CheckHeaderedItemsControl(checkedProperties, settings))
                {
                    yield return item;
                }
            }
        }

        IEnumerable<IElement> CheckHeaderedItemsControl(ICollection<string> checkedProperties, ElementEnumeratorSettings settings)
        {
            if(!settings.IncludeTemplates) yield break;

            var headeredItemsControl = Element as HeaderedItemsControl;

            if(headeredItemsControl != null)
            {
                checkedProperties.Add(HeaderedContentControl.HeaderTemplateProperty.Name);

                var template = headeredItemsControl.HeaderTemplate;

                if(template != null)
                {
                    var dataType = template.DataType as Type;

                    if(dataType == null)
                    {
                        var binding = BindingOperations.GetBinding(
                            headeredItemsControl,
                            HeaderedItemsControl.HeaderProperty
                            );

                        if(binding != null)
                            yield return Bound.DataTemplate(template, Type.GetAssociatedType(binding.Path.Path), Name);
                    }
                    else yield return Bound.DataTemplate(template, new BoundType(dataType), Name);
                }
            }
        }
    }
}