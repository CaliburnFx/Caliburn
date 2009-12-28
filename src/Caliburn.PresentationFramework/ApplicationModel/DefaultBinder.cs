namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Markup;
    using Actions;
    using Core.Metadata;
    using Metadata;
    using ViewModels;
    using Action=Actions.Action;

    /// <summary>
    /// The default implementation of <see cref="IBinder"/>.
    /// </summary>
    public class DefaultBinder : IBinder
    {
        private static readonly Type _presenterType = typeof(IPresenter);

        private bool _useMessageConventions;
        private bool _useBindingConventions;
        private readonly IMessageBinder _messageBinder;
        private readonly IViewModelDescriptionBuilder _viewModelDescriptionBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBinder"/> class.
        /// </summary>
        /// <param name="viewModelDescriptionBuilder"></param>
        /// <param name="messageBinder">The message binder.</param>
        public DefaultBinder(IViewModelDescriptionBuilder viewModelDescriptionBuilder, IMessageBinder messageBinder)
        {
            _viewModelDescriptionBuilder = viewModelDescriptionBuilder;
            _messageBinder = messageBinder;
        }

        /// <summary>
        /// Enables convention-based actions.
        /// </summary>
        public void EnableMessageConventions()
        {
            _useMessageConventions = true;
        }

        /// <summary>
        /// Enables convention-based bindings.
        /// </summary>
        public void EnableBindingConventions()
        {
            _useBindingConventions = true;
        }

        /// <summary>
        /// Binds the specified model to the view.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="view">The view.</param>
        /// <param name="context">The context.</param>
        public void Bind(object model, object view, object context)
        {
            TryApplyConventions(model, view);
            AttachTo(model, view, context);
        }

        /// <summary>
        /// Attaches the model and the view.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="view">The view.</param>
        /// <param name="context">The context.</param>
        protected virtual void AttachTo(object model, object view, object context)
        {
            Action.SetTarget(view as DependencyObject, model);

            var metadataContainer = model as IMetadataContainer;
            if (metadataContainer != null) metadataContainer.SetView(view, context, false);

            var viewAware = model as IViewAware;
            if (viewAware != null)
            {
                var element = view as FrameworkElement;
                if (element != null)
                {
                    element.Loaded += delegate
                    {
                        viewAware.ViewLoaded(element, context);
                    };
                }
#if !SILVERLIGHT
                else
                {
                    var contentElement = view as FrameworkContentElement;
                    if (contentElement != null)
                    {
                        contentElement.Loaded += delegate
                        {
                            viewAware.ViewLoaded(contentElement, context);
                        };
                    }
                }
#endif
            }
        }

        /// <summary>
        /// Applies the conventions.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="view">The view.</param>
        protected virtual void TryApplyConventions(object model, object view)
        {
            var element = view as DependencyObject;
            if(element == null) return;

            if(_useMessageConventions)
                ApplyMessageConventions(element, model);

            if(_useBindingConventions)
                ApplyBindingConventions(element, model);
        }

        /// <summary>
        /// Applies the action conventions.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="model">The model.</param>
        protected virtual void ApplyMessageConventions(DependencyObject element, object model)
        {
            var modelType = GetModelType(model);
            var host = _viewModelDescriptionBuilder.Build(modelType);

            foreach(var action in host)
            {
                var found = FindControl(element, action.Name);
                if(found == null) continue;

                var triggers = Message.GetTriggers(found);
                if(triggers != null && triggers.Count > 0) continue;

                AttachMessage(action, found);
            }
        }

        /// <summary>
        /// Gets the type of the model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        protected virtual Type GetModelType(object model)
        {
            return model.GetType();
        }

        /// <summary>
        /// Attaches a message to the control.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="control">The control.</param>
        protected virtual void AttachMessage(IAction action, DependencyObject control) 
        {
            var message = action.Name;

            if(action.Requirements.Count > 0)
            {
                message += "(";

                foreach (var requirement in action.Requirements)
                {
                    var paramName = requirement.Name;
                    var specialValue = "$" + paramName;

                    if (_messageBinder.IsSpecialValue(specialValue))
                        paramName = specialValue;

                    message += paramName + ",";
                }

                message = message.Remove(message.Length - 1, 1);

                message += ")";
            }

            Message.SetAttach(control, message);
        }

        /// <summary>
        /// Applies the binding conventions.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="model">The model.</param>
        protected virtual void ApplyBindingConventions(DependencyObject element, object model)
        {
            var host = model as IPresenterHost;
            if (host != null)
                TryBindHost(element);

            var props = GetModelType(model).GetProperties()
                .Where(x => _presenterType.IsAssignableFrom(x.PropertyType));

            foreach(var info in props)
            {
                var mode = info.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay;
                TryBindPresenter(element, info.Name, mode);
            }
        }

        /// <summary>
        /// Finds the control.
        /// </summary>
        /// <param name="parent">The parent to whose children will be searched.</param>
        /// <param name="name">The name of the element to locate.</param>
        /// <returns></returns>
        protected virtual DependencyObject FindControl(DependencyObject parent, string name)
        {
            var fe = parent as FrameworkElement;
            if (fe != null)
                return fe.FindName(name) as DependencyObject;
#if !SILVERLIGHT
            else
            {
                var fce = parent as FrameworkContentElement;
                if (fce != null)
                    return fce.FindName(name) as DependencyObject;
            }
#endif

            return null;
        }

        /// <summary>
        /// Tries the bind presenter hosts using conventions.
        /// </summary>
        /// <param name="element">The element.</param>
        protected virtual void TryBindHost(DependencyObject element)
        {
            var currentPresenter = FindControl(element, "CurrentPresenter");
            if (currentPresenter != null)
                TryBindContent(currentPresenter, "CurrentPresenter", BindingMode.TwoWay);

            var presenterHost = FindControl(element, "PresenterHost") as ItemsControl;
            if (presenterHost != null)
            {
                if (!TryAddBinding(presenterHost, ItemsControl.ItemsSourceProperty, "Presenters", BindingMode.OneWay))
                    return;

                if (presenterHost.ItemTemplate == null && string.IsNullOrEmpty(presenterHost.DisplayMemberPath))
                    presenterHost.ItemTemplate = CreateItemTemplate(presenterHost);

                var selector = presenterHost as Selector;
                if (selector != null)
                    TryAddBinding(selector, Selector.SelectedItemProperty, "CurrentPresenter", BindingMode.TwoWay);

                return;
            }
        }

        /// <summary>
        /// Tries to bind presenters by convention.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="name">The name.</param>
        /// <param name="mode">The mode.</param>
        protected virtual void TryBindPresenter(DependencyObject element, string name, BindingMode mode) 
        {
            var control = FindControl(element, name);

            if (control != null)
                TryBindContent(control, name, mode);
        }

        /// <summary>
        /// Tries to bind the content to a view model.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="path">The path.</param>
        /// <param name="mode">The mode.</param>
        protected virtual void TryBindContent(DependencyObject element, string path, BindingMode mode) 
        {
            var type = element.GetType();
            var contentProperty = type.GetCustomAttributes(typeof(ContentPropertyAttribute), true)
                                      .OfType<ContentPropertyAttribute>().FirstOrDefault()
                                  ?? new ContentPropertyAttribute("Content");

            var property = type.GetProperty(contentProperty.Name);
            if (property == null) return;

            TryAddBinding(element as FrameworkElement, View.ModelProperty, path, mode);
        }

        /// <summary>
        /// Tries to add a binding.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="property">The property.</param>
        /// <param name="path">The path.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        protected virtual bool TryAddBinding(FrameworkElement element, DependencyProperty property, string path, BindingMode mode) 
        {
            if (element == null)
                return false;

#if !SILVERLIGHT_20
            if (element.GetBindingExpression(property) != null)
                return false;
#endif

            var binding = new Binding(path) { Mode = mode };
            element.SetBinding(property, binding);

            return true;
        }

        private const string _templateCore =
        "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                      "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
                      "xmlns:am='clr-namespace:Caliburn.PresentationFramework.ApplicationModel;assembly=Caliburn.PresentationFramework'> " +
            "<ContentControl am:View.Model=\"{Binding}\" ";

        /// <summary>
        /// Creates an item template which binds view models.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns></returns>
        protected virtual DataTemplate CreateItemTemplate(ItemsControl itemsControl)
        {
            var context = View.GetContext(itemsControl);
            var template = _templateCore;

            if(context != null)
                template += "am:View.Context=\"" + context + "\"";
            
            template += " /></DataTemplate>";

#if SILVERLIGHT
            return (DataTemplate)XamlReader.Load(template);
#else
            return (DataTemplate)XamlReader.Parse(template);
#endif
        }
    }
}