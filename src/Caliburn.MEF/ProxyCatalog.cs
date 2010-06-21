namespace Caliburn.MEF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.ComponentModel.Composition.ReflectionModel;
    using System.Linq;
    using Core;
    using Core.Behaviors;

    /// <summary>
    /// A <see cref="ComposablePartCatalog"/> which adds proxy capabilities.
    /// </summary>
    public class ProxyCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private readonly ComposablePartCatalog _innerCatalog;
        private Dictionary<ComposablePartDefinition, ComposablePartDefinition> _parts;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyCatalog"/> class.
        /// </summary>
        /// <param name="innerCatalog">The inner catalog.</param>
        public ProxyCatalog(ComposablePartCatalog innerCatalog)
        {
            _innerCatalog = innerCatalog;
			_parts = CreateFrom(_innerCatalog);

            var notifyingCatalog = _innerCatalog as INotifyComposablePartCatalogChanged;
            if(notifyingCatalog != null)
                notifyingCatalog.Changing += NotifyingCatalog_Changing;
        }

        /// <summary>
        /// Occurs when [changed].
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed = delegate { };

        /// <summary>
        /// Occurs when [changing].
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing = delegate { };

        /// <summary>
        /// Handles the Changing event of the NotifyingCatalog control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.Composition.Hosting.ComposablePartCatalogChangeEventArgs"/> instance containing the event data.</param>
        private void NotifyingCatalog_Changing(object sender, ComposablePartCatalogChangeEventArgs e)
        {
            var removed = _parts.Where(x => e.RemovedDefinitions.Contains(x.Key)).Select(x => x.Key).ToList();
            removed.Apply(x => _parts.Remove(x));

            var added =
                e.AddedDefinitions.Select(
                    x => new KeyValuePair<ComposablePartDefinition, ComposablePartDefinition>(x, ConvertPart(x)));
            added.Apply(x => _parts[x.Key] = x.Value);

            Changing(
                this,
                new ComposablePartCatalogChangeEventArgs(
                    added.Select(x => x.Value),
                    removed,
                    e.AtomicComposition
                    )
                );
        }

        /// <summary>
        /// Gets the part definitions of the catalog.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.Linq.IQueryable`1"/> of <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition"/> objects of the
        /// <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartCatalog"/>.
        /// </value>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartCatalog"/> has been disposed of.
        /// </exception>
        /// <remarks>
        /// 	<note type="inheritinfo">
        /// Overriders of this property should never return <see langword="null"/>.
        /// </note>
        /// </remarks>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
				return _parts.Values.AsQueryable();
            }
        }

        private static Dictionary<ComposablePartDefinition, ComposablePartDefinition> CreateFrom(
            ComposablePartCatalog catalog)
        {
            var dictionary = new Dictionary<ComposablePartDefinition, ComposablePartDefinition>();
            catalog.Parts.Apply(x => dictionary[x] = ConvertPart(x));
            return dictionary;
        }

        private static ComposablePartDefinition ConvertPart(ComposablePartDefinition original)
        {
            var type = ReflectionModelServices.GetPartType(original).Value;

            return type.ShouldCreateProxy()
                       ? new ProxyPartDefinition(type, original)
                       : original;
        }
    }
}