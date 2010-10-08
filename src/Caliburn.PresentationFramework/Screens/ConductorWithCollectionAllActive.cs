namespace Caliburn.PresentationFramework.Screens
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Linq;
    using Core;

    public partial class Conductor<T>
    {
        /// <summary>
        /// An implementation of <see cref="IConductor"/> that holds on many items.
        /// </summary>
        public partial class Collection
        {
            /// <summary>
            /// An implementation of <see cref="IConductor"/> that holds on to many items wich are all activated.
            /// </summary>
            public class AllActive : Screen, IConductor
            {
                readonly BindableCollection<T> items = new BindableCollection<T>();
                readonly bool openPublicItems;
                ICloseStrategy<T> closeStrategy;

                /// <summary>
                /// Initializes a new instance of the <see cref="Conductor&lt;T&gt;.Collection.AllActive"/> class.
                /// </summary>
                /// <param name="openPublicItems">if set to <c>true</c> opens public items that are properties of this class.</param>
                public AllActive(bool openPublicItems)
                    : this()
                {
                    this.openPublicItems = openPublicItems;
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="Conductor&lt;T&gt;.Collection.AllActive"/> class.
                /// </summary>
                public AllActive()
                {
                    items.CollectionChanged += (s, e) =>{
                        switch(e.Action)
                        {
                            case NotifyCollectionChangedAction.Add:
                            case NotifyCollectionChangedAction.Replace:
                                e.NewItems.OfType<IChild<IConductor>>().Apply(x => x.Parent = this);
                                break;
                            case NotifyCollectionChangedAction.Reset:
                                items.OfType<IChild<IConductor>>().Apply(x => x.Parent = this);
                                break;
                        }
                    };
                }

                /// <summary>
                /// Gets or sets the close strategy.
                /// </summary>
                /// <value>The close strategy.</value>
                public ICloseStrategy<T> CloseStrategy
                {
                    get { return closeStrategy ?? (closeStrategy = new DefaultCloseStrategy<T>(true)); }
                    set { closeStrategy = value; }
                }

                /// <summary>
                /// Gets the items that are currently being conducted.
                /// </summary>
                public IObservableCollection<T> Items
                {
                    get { return items; }
                }

                /// <summary>
                /// The currently active item.
                /// </summary>
                /// <value></value>
                object IConductor.ActiveItem
                {
                    get { return null; }
                    set { ActivateItem((T)value); }
                }

                /// <summary>
                /// Gets all the items that are being conducted.
                /// </summary>
                /// <returns></returns>
                IEnumerable IConductor.GetConductedItems()
                {
                    return items;
                }

                /// <summary>
                /// Activates the specified item.
                /// </summary>
                /// <param name="item">The item to activate.</param>
                void IConductor.ActivateItem(object item)
                {
                    ActivateItem((T)item);
                }

                /// <summary>
                /// Closes the specified item.
                /// </summary>
                /// <param name="item">The item to close.</param>
                void IConductor.CloseItem(object item)
                {
                    CloseItem((T)item);
                }

                /// <summary>
                /// Occurs when an activation request is processed.
                /// </summary>
                public event EventHandler<ActivationProcessedEventArgs> ActivationProcessed = delegate { };

                /// <summary>
                /// Called when activating.
                /// </summary>
                protected override void OnActivate()
                {
                    items.OfType<IActivate>().Apply(x => x.Activate());
                }

                /// <summary>
                /// Called when deactivating.
                /// </summary>
                /// <param name="close">Inidicates whether this instance will be closed.</param>
                protected override void OnDeactivate(bool close)
                {
                    items.OfType<IDeactivate>().Apply(x => x.Deactivate(close));
                }

                /// <summary>
                /// Called to check whether or not this instance can close.
                /// </summary>
                /// <param name="callback">The implementor calls this action with the result of the close check.</param>
                public override void CanClose(Action<bool> callback)
                {
                    CloseStrategy.Execute(items, (canClose, closable) =>{
                        closable.Apply(CloseItemCore);
                        callback(canClose);
                    });
                }

                /// <summary>
                /// Called when initializing.
                /// </summary>
                protected override void OnInitialize()
                {
                    if(openPublicItems)
                    {
                        GetType().GetProperties()
                            .Where(x => x.Name != "Parent" && typeof(T).IsAssignableFrom(x.PropertyType))
                            .Select(x => x.GetValue(this, null))
                            .Cast<T>()
                            .Apply(ActivateItem);
                    }
                }

                /// <summary>
                /// Activates the specified item.
                /// </summary>
                /// <param name="item">The item to activate.</param>
                public void ActivateItem(T item)
                {
                    if(item == null)
                        return;

                    item = EnsureItem(item);

                    if(IsActive)
                    {
                        var activator = item as IActivate;
                        if(activator != null)
                            activator.Activate();
                    }

                    OnActivationProcessed(item, true);
                }

                /// <summary>
                /// Closes the specified item.
                /// </summary>
                /// <param name="item">The item to close.</param>
                public void CloseItem(T item)
                {
                    if(item == null)
                        return;

                    CloseStrategy.Execute(new[] { item }, (canClose, closable) =>{
                        if(canClose)
                            CloseItemCore(item);
                    });
                }

                /// <summary>
                /// Called by a subclass when an activation needs processing.
                /// </summary>
                /// <param name="item">The item on which activation was attempted.</param>
                /// <param name="success">if set to <c>true</c> activation was successful.</param>
                protected virtual void OnActivationProcessed(T item, bool success)
                {
                    if(item == null)
                        return;

                    ActivationProcessed(this, new ActivationProcessedEventArgs {
                        Item = item,
                        Success = success
                    });
                }

                void CloseItemCore(T item)
                {
                    var deactivator = item as IDeactivate;
                    if(deactivator != null)
                        deactivator.Deactivate(true);

                    items.Remove(item);
                }

                /// <summary>
                /// Ensures that an item is ready to be activated.
                /// </summary>
                /// <param name="newItem"></param>
                /// <returns>The item to be activated.</returns>
                protected T EnsureItem(T newItem)
                {
                    var index = items.IndexOf(newItem);

                    if(index == -1)
                        items.Add(newItem);
                    else newItem = items[index];

                    return newItem;
                }
            }
        }
    }
}