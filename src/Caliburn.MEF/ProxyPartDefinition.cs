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
        readonly ComposablePartDefinition innerDefinition;
        ProxyPart part;
        readonly Type implementation;
		
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyPartDefinition"/> class.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="innerDefinition">The inner definition.</param>
        public ProxyPartDefinition(Type implementation, ComposablePartDefinition innerDefinition)
        {
        	if (implementation == null) throw new ArgumentNullException("implementation");
        	if (innerDefinition == null) throw new ArgumentNullException("innerDefinition");

        	this.innerDefinition = innerDefinition;
            this.implementation = implementation;
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
			// TODO:
			// return ReflectionModelServices.IsDisposalRequired(innerDefinition)
			//           ? new DisposableProxyPart(implementation, innerDefinition.CreatePart())
			//           : new ProxyPart(implementation, innerDefinition.CreatePart());
            if(part == null)
                part = new ProxyPart(implementation, innerDefinition.CreatePart());
            return part;
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
            get { return innerDefinition.ExportDefinitions; }
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
            get { return innerDefinition.ImportDefinitions; }
        }

        /// <summary>
        /// Gets the metadata of the definition.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Collections.Generic.IDictionary`2"/> containing the metadata of the
        /// <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition"/>. The default is an empty, read-only
        /// <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </value>
        /// <remarks>
        /// 	<note type="inheritinfo">
        /// Overriders of this property should return a read-only
        /// <see cref="T:System.Collections.Generic.IDictionary`2"/> object with a case-sensitive,
        /// non-linguistic comparer, such as <see cref="P:System.StringComparer.Ordinal"/>,
        /// and should never return <see langword="null"/>. If the
        /// <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition"/> does contain metadata,
        /// return an empty <see cref="T:System.Collections.Generic.IDictionary`2"/> instead.
        /// </note>
        /// </remarks>
		public override IDictionary<string, object> Metadata
		{
			get { return innerDefinition.Metadata; }
		}

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
		public override string ToString()
		{
			return innerDefinition.ToString();
		}
	}
}