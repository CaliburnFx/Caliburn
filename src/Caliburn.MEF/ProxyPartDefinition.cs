namespace Caliburn.MEF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;

    /// <summary>
    /// A <see cref="ComposablePartDefinition"/>  which adds proxy capabilities.
    /// </summary>
    public class ProxyPartDefinition : ComposablePartDefinition
    {
        private readonly ComposablePartDefinition _innerDefinition;
        private ProxyPart _part;
        private readonly Type _implementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyPartDefinition"/> class.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="innerDefinition">The inner definition.</param>
        public ProxyPartDefinition(Type implementation, ComposablePartDefinition innerDefinition)
        {
            _innerDefinition = innerDefinition;
            _implementation = implementation;
        }

        /// <summary>
        /// Creates a new instance of a part that the definition describes.
        /// </summary>
        /// <returns>
        /// The created <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart"/>.
        /// </returns>
        /// <remarks>
        /// 	<note type="inheritinfo">
        /// Derived types overriding this method should return a new instance of a
        /// <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart"/> on every invoke and should never return
        /// <see langword="null"/>.
        /// </note>
        /// </remarks>
        public override ComposablePart CreatePart()
        {
            if(_part == null)
                _part = new ProxyPart(_implementation, _innerDefinition.CreatePart());
            return _part;
        }

        /// <summary>
        /// Gets the export definitions that describe the exported values provided by parts
        /// created by the definition.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> of <see cref="T:System.ComponentModel.Composition.Primitives.ExportDefinition"/> objects describing
        /// the exported values provided by <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart"/> objects created by the
        /// <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition"/>.
        /// </value>
        /// <remarks>
        /// 	<note type="inheritinfo">
        /// Overrides of this property should never return <see langword="null"/>.
        /// If the <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart"/> objects created by the
        /// <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition"/> do not provide exported values, return
        /// an empty <see cref="T:System.Collections.Generic.IEnumerable`1"/> instead.
        /// </note>
        /// </remarks>
        public override IEnumerable<ExportDefinition> ExportDefinitions
        {
            get { return _innerDefinition.ExportDefinitions; }
        }

        /// <summary>
        /// Gets the import definitions that describe the imports required by parts created
        /// by the definition.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> of <see cref="T:System.ComponentModel.Composition.Primitives.ImportDefinition"/> objects describing
        /// the imports required by <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart"/> objects created by the
        /// <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition"/>.
        /// </value>
        /// <remarks>
        /// 	<note type="inheritinfo">
        /// Overriders of this property should never return <see langword="null"/>.
        /// If the <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart"/> objects created by the
        /// <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition"/> do not have imports, return an empty
        /// <see cref="T:System.Collections.Generic.IEnumerable`1"/> instead.
        /// </note>
        /// </remarks>
        public override IEnumerable<ImportDefinition> ImportDefinitions
        {
            get { return _innerDefinition.ImportDefinitions; }
        }
    }
}