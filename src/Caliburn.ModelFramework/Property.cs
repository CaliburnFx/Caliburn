namespace Caliburn.ModelFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using Core.Metadata;
    using PresentationFramework;
    using PresentationFramework.Views;

    /// <summary>
    /// An implementation of <see cref="IProperty{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Property<T> : MetadataContainer, IProperty<T>
    {
        private readonly Dictionary<object, DependencyObject> _views = new Dictionary<object, DependencyObject>();
        private readonly IPropertyDefinition<T> _definition;
        private IModelNode _parent;

        private T _value;
        private T _editedValue;
        private bool _isEditing;
        private readonly BindableCollection<IValidationResult> _errors = new BindableCollection<IValidationResult>();
        private bool _isInterrogating;

        /// <summary>
        /// Initializes a new instance of the <see cref="Property&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="value">The value.</param>
        public Property(IPropertyDefinition<T> model, IModelNode parent, T value)
        {
            _definition = model;
            _parent = parent;
            _value = _editedValue = value;

            var pm = _value as IModelNode;
            if(pm != null) pm.Parent = this;
        }

        /// <summary>
        /// Gets the validation results.
        /// </summary>
        /// <value>The validation results.</value>
        public IList<IValidationResult> ValidationResults
        {
            get { return _errors; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid
        {
            get
            {
                if(_errors.Count > 0) return false;

                if(_value is IModelNode)
                    return ((IModelNode)_value).IsValid;
                return true;
            }
        }

        /// <summary>
        /// Gets the definition.
        /// </summary>
        /// <value>The definition.</value>
        IPropertyDefinition IProperty.Definition
        {
            get { return _definition; }
        }

        /// <summary>
        /// Gets the definition.
        /// </summary>
        /// <value>The definition.</value>
        public IPropertyDefinition<T> Definition
        {
            get { return _definition; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is in edit mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in edit mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsEditing
        {
            get { return _isEditing; }
            private set
            {
                _isEditing = value;
                NotifyOfPropertyChange("IsEditing");
            }
        }

        /// <summary>
        /// Gets or sets the parent model.
        /// </summary>
        /// <value>The parent.</value>
        public IModelNode Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
        public bool IsDirty
        {
            get
            {
                if(_value is ValueType)
                    return !_value.Equals(_editedValue);

                var notSameReference = !ReferenceEquals(_value, _editedValue);

                if(notSameReference)
                {
                    if(_value is IEquatable<T>)
                    {
                        var value = _value as IEquatable<T>;

                        if(value != null)
                            return !value.Equals(_editedValue);

                        var edited = _editedValue as IEquatable<T>;

                        if(edited != null) return true;
                        return false;
                    }

                    return true;
                }

                if(_value is IModelNode)
                    return ((IModelNode)_value).IsDirty;
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the untyped value.
        /// </summary>
        /// <value>The untyped value.</value>
        public object UntypedValue
        {
            get
            {
                var converter = _definition.GetMetadata<IPropertyValueConverter<T>>();

                if(converter != null)
                    return converter.ConvertBack(this, Value);
                return Value;
            }
            set
            {
                _errors.Clear();
                var converter = _definition.GetMetadata<IPropertyValueConverter<T>>();

                if(converter != null)
                    SetValue(converter.Convert(this, value), false, true);
                else SetValue((T)value, false, true);
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get { return _editedValue; }
            set { SetValue(value, true, true); }
        }

        /// <summary>
        /// Occurs when the model has changed.
        /// </summary>
        public event Action<IModelNode, IUndoRedoHandle> ModelChanged = delegate { };

        /// <summary>
        /// Called when the screen's view is loaded.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="context">The context.</param>
        public void AttachView(DependencyObject view, object context)
        {
            _views[context ?? DefaultViewLocator.DefaultContext] = view;
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The view</returns>
        public DependencyObject GetView(object context)
        {
            DependencyObject view;
            _views.TryGetValue(context ?? DefaultViewLocator.DefaultContext, out view);
            return view;
        }

        private void SetValue(T value, bool clearErrors, bool raiseUndoRedo)
        {
            var pm = value as IModelNode;
            if(pm != null) pm.Parent = this;

            if(IsEditing)
            {
                var oldValue = _editedValue;
                var newValue = value;

                _editedValue = newValue;

                var editable = newValue as IEditableObject;
                if(editable != null) editable.BeginEdit();

                if(_isInterrogating) return;

                if(clearErrors) _errors.Clear();

                UseInterrogators<IPropertyChangeAware<T>>(
                    interrogators =>{
                        foreach(var validator in interrogators)
                        {
                            if(!validator.Interrogate(this)) return;
                        }
                    });

                NotifyOfPropertyChange("Value");
                _parent.NotifyOfPropertyChange(_definition.Name);
                BubblePropertyChange("IsDirty");
                BubblePropertyChange("IsValid");

                if(raiseUndoRedo)
                {
                    var undoRedoHandle = new UndoRedoHandle(
                        () => SetValue(oldValue, true, false),
                        () => SetValue(newValue, true, false)
                        );

                    BubbleModelChange(this, undoRedoHandle);
                }
            }
            else
            {
                _value = _editedValue = value;
                NotifyOfPropertyChange("Value");
                _parent.NotifyOfPropertyChange(_definition.Name);
            }
        }

        /// <summary>
        /// Bubbles a model change notification.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="handle">The handle.</param>
        public void BubbleModelChange(IModelNode source, IUndoRedoHandle handle)
        {
            ModelChanged(source, handle);
            _parent.BubbleModelChange(source, handle);
        }

        /// <summary>
        /// Uses the interrogators.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="borrower">The borrower.</param>
        public void UseInterrogators<K>(Action<IEnumerable<K>> borrower)
            where K : IMetadata
        {
            _isInterrogating = true;

            borrower(_definition.FindMetadata<K>());

            _isInterrogating = false;
        }

        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public void Accept(IModelVisitor visitor)
        {
            var valueModel = Value as IModelNode;
            if(valueModel != null) valueModel.Accept(visitor);

            visitor.VisitProperty(this);
        }

        /// <summary>
        /// Begins an edit on an object.
        /// </summary>
        public void BeginEdit()
        {
            if(_isEditing) return;

            IsEditing = true;

            if(_value is IEditableObject)
                ((IEditableObject)_value).BeginEdit();
        }

        /// <summary>
        /// Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> or <see cref="M:System.ComponentModel.IBindingList.AddNew"/> call into the underlying object.
        /// </summary>
        public void EndEdit()
        {
            if(!_isEditing) return;

            IsEditing = false;

            _value = _editedValue;

            if(_value is IEditableObject)
                ((IEditableObject)_value).EndEdit();
        }

        /// <summary>
        /// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> call.
        /// </summary>
        public void CancelEdit()
        {
            if(!_isEditing) return;

            IsEditing = false;

            _editedValue = _value;

            if(_value is IEditableObject)
                ((IEditableObject)_value).CancelEdit();

            NotifyOfPropertyChange("Value");
            _parent.NotifyOfPropertyChange(_definition.Name);
            NotifyOfPropertyChange("IsDirty");
        }

        /// <summary>
        /// Bubbles a property change notification.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void BubblePropertyChange(string propertyName)
        {
            NotifyOfPropertyChange(propertyName);
            _parent.BubblePropertyChange(propertyName);
        }
    }
}