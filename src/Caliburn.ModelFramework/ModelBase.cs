namespace Caliburn.ModelFramework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Core.Metadata;
    using PresentationFramework;
    using PresentationFramework.ApplicationModel;

    /// <summary>
    /// An implementation of <see cref="IModel"/>.
    /// </summary>
    public class ModelBase : Presenter, IModel
    {
        private readonly Dictionary<string, IProperty> _propertyValues =
            new Dictionary<string, IProperty>();

        private readonly IModelDefinition _definition;
        private bool _isEditing;
        private readonly BindableCollection<IValidationResult> _errors = new BindableCollection<IValidationResult>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBase"/> class.
        /// </summary>
        public ModelBase()
        {
            _definition = ModelRepository.Current.GetModelFor(GetType());
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBase"/> class.
        /// </summary>
        /// <param name="definition">The definition.</param>
        public ModelBase(IModelDefinition definition)
        {
            _definition = definition;
            Initialize();
        }

        /// <summary>
        /// Gets the definition.
        /// </summary>
        /// <value>The definition.</value>
        public IModelDefinition Definition
        {
            get { return _definition; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
        public bool IsDirty
        {
            get
            {
                foreach(var value in _propertyValues.Values)
                {
                    if(value.IsDirty) return true;
                }

                return false;
            }
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

                foreach(var value in _propertyValues.Values)
                {
                    if(!value.IsValid) return false;
                }

                return true;
            }
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
        public IModelNode Parent { get; set; }

        /// <summary>
        /// Gets the <see cref="IProperty"/> with the specified property name.
        /// </summary>
        /// <value></value>
        public IProperty this[string propertyName]
        {
            get { return _propertyValues[propertyName]; }
        }

        /// <summary>
        /// Occurs when the model has changed.
        /// </summary>
        public event Action<IModelNode, IUndoRedoHandle> ModelChanged = delegate { };

        /// <summary>
        /// Bubbles a model change notification.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="handle">The handle.</param>
        public void BubbleModelChange(IModelNode source, IUndoRedoHandle handle)
        {
            ModelChanged(source, handle);
            if(Parent != null) Parent.BubbleModelChange(source, handle);
        }

        /// <summary>
        /// Uses the interrogators.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="borrower">The borrower.</param>
        public void UseInterrogators<T>(Action<IEnumerable<T>> borrower)
            where T : IMetadata
        {
            borrower(_definition.GetMatchingMetadata<T>());
        }

        /// <summary>
        /// Called when [initialize].
        /// </summary>
        protected override void OnInitialize()
        {
            foreach(var model in _definition)
            {
                _propertyValues[model.Name] = model.CreateInstance(this);
            }

            base.OnInitialize();
        }

        /// <summary>
        /// Determines whether this instance can shutdown.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can shutdown; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanShutdown()
        {
            return !IsDirty;
        }

        /// <summary>
        /// Called when [shutdown].
        /// </summary>
        protected override void OnShutdown()
        {
            CancelEdit();

            base.OnShutdown();
        }

        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public void Accept(IModelVisitor visitor)
        {
            foreach(var property in _propertyValues.Values)
            {
                property.Accept(visitor);
            }

            var method = visitor.GetType().GetMethod("VisitModel");
            var built = method.MakeGenericMethod(GetType());

            built.Invoke(visitor, new[] {this});
        }

        /// <summary>
        /// Begins an edit on an object.
        /// </summary>
        public void BeginEdit()
        {
            if(_isEditing) return;

            foreach(var value in _propertyValues.Values)
            {
                value.BeginEdit();
            }

            IsEditing = true;
        }

        /// <summary>
        /// Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> or <see cref="M:System.ComponentModel.IBindingList.AddNew"/> call into the underlying object.
        /// </summary>
        public void EndEdit()
        {
            if(!_isEditing) return;

            IsEditing = false;

            foreach(var value in _propertyValues.Values)
            {
                value.EndEdit();
            }

            BubblePropertyChange("IsDirty");
        }

        /// <summary>
        /// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> call.
        /// </summary>
        public void CancelEdit()
        {
            if(!_isEditing) return;

            IsEditing = false;

            foreach(var value in _propertyValues.Values)
            {
                value.CancelEdit();
            }

            BubblePropertyChange("IsDirty");
        }

        /// <summary>
        /// Bubbles a property change notification.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void BubblePropertyChange(string propertyName)
        {
            NotifyOfPropertyChange(propertyName);

            if(Parent != null)
                Parent.BubblePropertyChange(propertyName);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="definition">The definition.</param>
        /// <returns></returns>
        public K GetValue<K>(IPropertyDefinition<K> definition)
        {
            var state = GetState(definition);
            return state.Value;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="definition">The definition.</param>
        /// <param name="value">The value.</param>
        public void SetValue<K>(IPropertyDefinition<K> definition, K value)
        {
            var state = GetState(definition);
            state.Value = value;
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="definition">The definition.</param>
        /// <returns></returns>
        public IProperty<K> GetState<K>(IPropertyDefinition<K> definition)
        {
            IProperty state;

            if(!_propertyValues.TryGetValue(definition.Name, out state))
            {
                state = definition.CreateInstance(this);
                _propertyValues[definition.Name] = state;
            }

            return (IProperty<K>)state;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IProperty> GetEnumerator()
        {
            return _propertyValues.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static IModelDefinition GetModel<T>()
        {
            return ModelRepository.Current.GetModelFor(typeof(T));
        }

        /// <summary>
        /// Creates a property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public static IPropertyDefinition<K> Property<T, K>(Expression<Func<T, K>> property)
        {
            return Property<T, K>(GetPropertyNameFromExpression(property));
        }

        /// <summary>
        /// Creates a property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="property">The property.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static IPropertyDefinition<K> Property<T, K>(Expression<Func<T, K>> property, Func<K> defaultValue)
        {
            return Property<T, K>(GetPropertyNameFromExpression(property), defaultValue);
        }

        /// <summary>
        /// Creates a property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static IPropertyDefinition<K> Property<T, K>(string propertyName)
        {
            return GetModel<T>().AddProperty<K>(propertyName);
        }

        /// <summary>
        /// Creates a property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static IPropertyDefinition<K> Property<T, K>(string propertyName, Func<K> defaultValue)
        {
            return GetModel<T>().AddProperty(propertyName, defaultValue);
        }

        /// <summary>
        /// Creates an association.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public static IPropertyDefinition<K> Association<T, K>(Expression<Func<T, K>> property)
            where K : new()
        {
            return Association<T, K>(GetPropertyNameFromExpression(property));
        }

        /// <summary>
        /// Creates an association.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static IPropertyDefinition<K> Association<T, K>(string propertyName)
            where K : new()
        {
            return GetModel<T>().AddAssociation<K>(propertyName);
        }

        /// <summary>
        /// Creates a collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public static IPropertyDefinition<ICollectionNode<K>> Collection<T, K>(Expression<Func<T, IList<K>>> property)
        {
            return Collection<T, K>(GetPropertyNameFromExpression(property));
        }

        /// <summary>
        /// Creates a collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static IPropertyDefinition<ICollectionNode<K>> Collection<T, K>(string propertyName)
        {
            return GetModel<T>().AddCollection<K>(propertyName);
        }

        private static string GetPropertyNameFromExpression(LambdaExpression property)
        {
            var memberExpression = (MemberExpression)property.Body;
            return memberExpression.Member.Name;
        }
    }
}