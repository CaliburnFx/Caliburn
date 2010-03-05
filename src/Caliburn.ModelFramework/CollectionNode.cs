namespace Caliburn.ModelFramework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using Core;
    using PresentationFramework;

    /// <summary>
    /// An implementation of <see cref="ICollectionNode{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionNode<T> : BindableCollection<T>, ICollectionNode<T>
    {
        private bool _isDirty;
        private readonly List<T> _values = new List<T>();
        private bool _isEditing;
        private bool _isPerformingUndoRedo;
        private List<T> _clearedItems;

        /// <summary>
        /// Gets or sets the parent model.
        /// </summary>
        /// <value>The parent.</value>
        public IModelNode Parent { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
        public bool IsDirty
        {
            get
            {
                if(_isDirty) return true;

                foreach(var model in this)
                {
                    var pm = model as IModelNode;
                    if(pm != null && pm.IsDirty) return true;
                }

                return false;
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
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid
        {
            get
            {
                foreach(var model in this)
                {
                    var pm = model as IModelNode;
                    if(pm != null && !pm.IsValid) return false;
                }

                return true;
            }
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
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public void Accept(IModelVisitor visitor)
        {
            foreach(var model in this)
            {
                var pm = model as IModelNode;
                if(pm != null) pm.Accept(visitor);
            }

            visitor.VisitCollection(this);
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            var pm = item as IModelNode;
            if(pm != null) pm.Parent = this;

            if(_isEditing)
            {
                var editable = item as IEditableObject;
                if(editable != null) editable.BeginEdit();
            }

            base.InsertItem(index, item);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            if(!_isPerformingUndoRedo)
                _clearedItems = this.ToList();

            base.ClearItems();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged"/> event with the provided arguments.
        /// </summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if(_isEditing)
            {
                HandleUndoRedo(e);

                _isDirty = true;

                if(Parent != null) Parent.Validate();
                else this.Validate();

                BubblePropertyChange("IsDirty");
                BubblePropertyChange("IsValid");
            }
        }

        /// <summary>
        /// Handles the undo redo.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void HandleUndoRedo(NotifyCollectionChangedEventArgs e)
        {
            if(_isPerformingUndoRedo) return;

            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    BubbleModelChange(
                        this,
                        new UndoRedoHandle(
                            () =>{
                                _isPerformingUndoRedo = true;
                                e.NewItems.OfType<T>().Apply(x => Remove(x));
                                _isPerformingUndoRedo = false;
                            },
                            () =>{
                                _isPerformingUndoRedo = true;
                                ReInsert(e.NewItems, e.NewStartingIndex);
                                _isPerformingUndoRedo = false;
                            })
                        );
                    break;
                case NotifyCollectionChangedAction.Remove:
                    BubbleModelChange(
                        this,
                        new UndoRedoHandle(
                            () =>{
                                _isPerformingUndoRedo = true;
                                ReInsert(e.OldItems, e.OldStartingIndex);
                                _isPerformingUndoRedo = false;
                            },
                            () =>{
                                _isPerformingUndoRedo = true;
                                e.OldItems.OfType<T>().Apply(x => Remove(x));
                                _isPerformingUndoRedo = false;
                            })
                        );
                    break;
#if !SILVERLIGHT
                case NotifyCollectionChangedAction.Move:
                    BubbleModelChange(
                        this,
                        new UndoRedoHandle(
                            () =>{
                                _isPerformingUndoRedo = true;
                                e.OldItems.OfType<T>().Apply(x => Remove(x));
                                ReInsert(e.OldItems, e.OldStartingIndex);
                                _isPerformingUndoRedo = false;
                            },
                            () =>{
                                _isPerformingUndoRedo = true;
                                e.OldItems.OfType<T>().Apply(x => Remove(x));
                                ReInsert(e.OldItems, e.NewStartingIndex);
                                _isPerformingUndoRedo = false;
                            })
                        );
                    break;
#endif
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    var localClearedItems = _clearedItems;
                    _clearedItems = null;

                    BubbleModelChange(
                        this,
                        new UndoRedoHandle(
                            () =>{
                                _isPerformingUndoRedo = true;
                                if(localClearedItems != null)
                                    localClearedItems.Apply(Add);
                                else if(e.NewItems != null)
                                {
                                    e.NewItems.OfType<T>().Apply(x => Remove(x));
                                    ReInsert(e.OldItems, e.OldStartingIndex);
                                }
                                _isPerformingUndoRedo = false;
                            },
                            () =>{
                                _isPerformingUndoRedo = true;
                                if(localClearedItems != null)
                                    Clear();
                                else if(e.OldItems != null)
                                {
                                    e.OldItems.OfType<T>().Apply(x => Remove(x));
                                    ReInsert(e.NewItems, e.NewStartingIndex);
                                }
                                _isPerformingUndoRedo = false;
                            })
                        );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ReInsert(IEnumerable enumerable, int index)
        {
            if(enumerable != null)
            {
                foreach(var oldItem in enumerable.OfType<T>())
                {
                    Insert(index, oldItem);
                    index++;
                }
            }
        }

        /// <summary>
        /// Begins an edit on an object.
        /// </summary>
        public void BeginEdit()
        {
            if(_isEditing) return;

            _isEditing = true;

            foreach(var item in this)
            {
                _values.Add(item);

                if(item is IEditableObject)
                    ((IEditableObject)item).BeginEdit();
            }
        }

        /// <summary>
        /// Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> or <see cref="M:System.ComponentModel.IBindingList.AddNew"/> call into the underlying object.
        /// </summary>
        public void EndEdit()
        {
            if(!_isEditing) return;

            _isEditing = false;
            _isDirty = false;

            _values.Clear();

            foreach(var item in this)
            {
                if(item is IEditableObject)
                    ((IEditableObject)item).EndEdit();
            }
        }

        /// <summary>
        /// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> call.
        /// </summary>
        public void CancelEdit()
        {
            if(!_isEditing) return;

            _isEditing = false;
            _isDirty = false;

            Clear();

            foreach(var item in _values)
            {
                Add(item);

                if(item is IEditableObject)
                    ((IEditableObject)item).CancelEdit();
            }

            _values.Clear();
        }

        /// <summary>
        /// Fires a property change notification.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void NotifyOfPropertyChange(string propertyName)
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Bubbles a property change notification.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void BubblePropertyChange(string propertyName)
        {
            NotifyOfPropertyChange(propertyName);
            if(Parent != null) Parent.BubblePropertyChange(propertyName);
        }
    }
}