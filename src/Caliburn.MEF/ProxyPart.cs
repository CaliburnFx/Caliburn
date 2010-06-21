namespace Caliburn.MEF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using Core;
    using Core.Behaviors;
    using Core.IoC;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// A <see cref="ComposablePart"/> which adds proxy capabilities.
    /// </summary>
    public class ProxyPart : ComposablePart
    {
        private readonly Type _implementation;
    	readonly IComponentRegistration _registration;
    	private readonly ComposablePart _innerPart;
        private object _instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyPart"/> class.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <param name="innerPart">The inner part.</param>
        public ProxyPart(Type implementation, ComposablePart innerPart)
        {
            _implementation = implementation;
            _innerPart = innerPart;
        }

    	/// <summary>
    	/// Initializes a new instance of the <see cref="ProxyPart"/> class.
    	/// </summary>
    	/// <param name="registration">The registration</param>
    	/// <param name="innerPart">The inner part.</param>
    	public ProxyPart(ComponentRegistrationBase registration, ComposablePart innerPart)
		{
			_implementation = GetImplementation(registration);
			_registration = registration;
			_innerPart = innerPart;
		}

		private static Type GetImplementation(IComponentRegistration registration)
		{
			var singleton = registration as Singleton;
			if (singleton != null) return singleton.Implementation;

			var perRequest = registration as PerRequest;
			if (perRequest != null) return perRequest.Implementation;

			throw new NotSupportedException();
		}

		/// <summary>
        /// Gets the export definitions that describe the exported values provided by the part.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> of <see cref="T:System.ComponentModel.Composition.Primitives.ExportDefinition"/> objects describing
        /// the exported values provided by the <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart"/>.
        /// </value>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart"/> has been disposed of.
        /// </exception>
        /// <remarks>
        /// 	<para>
        /// 		<note type="inheritinfo">
        /// If the <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart"/> was created from a
        /// <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition"/>, this property should return the result of
        /// <see cref="P:System.ComponentModel.Composition.Primitives.ComposablePartDefinition.ExportDefinitions"/>.
        /// </note>
        /// 	</para>
        /// 	<para>
        /// 		<note type="inheritinfo">
        /// Overriders of this property should never return <see langword="null"/>.
        /// If the <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart"/> does not have exports, return an empty
        /// <see cref="T:System.Collections.Generic.IEnumerable`1"/> instead.
        /// </note>
        /// 	</para>
        /// </remarks>
        public override IEnumerable<ExportDefinition> ExportDefinitions
        {
            get { return _innerPart.ExportDefinitions; }
        }

        /// <summary>
        /// Gets the import definitions that describe the imports required by the part.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> of <see cref="T:System.ComponentModel.Composition.Primitives.ImportDefinition"/> objects describing
        /// the imports required by the <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart"/>.
        /// </value>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart"/> has been disposed of.
        /// </exception>
        /// <remarks>
        /// 	<para>
        /// 		<note type="inheritinfo">
        /// If the <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart"/> was created from a
        /// <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition"/>, this property should return the result of
        /// <see cref="P:System.ComponentModel.Composition.Primitives.ComposablePartDefinition.ImportDefinitions"/>.
        /// </note>
        /// 	</para>
        /// 	<para>
        /// 		<note type="inheritinfo">
        /// Overrides of this property should never return <see langword="null"/>.
        /// If the <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart"/> does not have imports, return an empty
        /// <see cref="T:System.Collections.Generic.IEnumerable`1"/> instead.
        /// </note>
        /// 	</para>
        /// </remarks>
        public override IEnumerable<ImportDefinition> ImportDefinitions
        {
            get { return _innerPart.ImportDefinitions; }
        }

        /// <summary>
        /// Gets the exported value described by the specified definition.
        /// </summary>
        /// <param name="definition">One of the <see cref="T:System.ComponentModel.Composition.Primitives.ExportDefinition"/> objects from the
        /// <see cref="P:System.ComponentModel.Composition.Primitives.ComposablePart.ExportDefinitions"/> property describing the exported value
        /// to return.</param>
        /// <returns>
        /// The exported value described by <paramref name="definition"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="definition"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="definition"/> did not originate from the <see cref="P:System.ComponentModel.Composition.Primitives.ComposablePart.ExportDefinitions"/>
        /// property on the <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart"/>.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// One or more pre-requisite imports, indicated by <see cref="P:System.ComponentModel.Composition.Primitives.ImportDefinition.IsPrerequisite"/>,
        /// have not been set.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart"/> has been disposed of.
        /// </exception>
        /// <exception cref="T:System.ComponentModel.Composition.Primitives.ComposablePartException">
        /// An error occurred getting the exported value described by the <see cref="T:System.ComponentModel.Composition.Primitives.ExportDefinition"/>.
        /// </exception>
        public override object GetExportedValue(ExportDefinition definition)
        {
            if(_instance != null)
                return _instance;

            object creationPolicy;
            if(definition.Metadata.TryGetValue("System.ComponentModel.Composition.CreationPolicy", out creationPolicy))
            {
                var actual = (CreationPolicy)creationPolicy;

                if(actual == CreationPolicy.Shared)
                {
                    _instance = CreateInstance();
                    return _instance;
                }
            }
			
			var isSingleton = (_registration as Singleton) != null;
			if (isSingleton)
			{
				_instance = CreateInstance();
				return _instance;				
			}
            return CreateInstance();
        }

        /// <summary>
        /// Sets the import described by the specified definition with the specified exports.
        /// </summary>
        /// <param name="definition">One of the <see cref="T:System.ComponentModel.Composition.Primitives.ImportDefinition"/> objects from the
        /// <see cref="P:System.ComponentModel.Composition.Primitives.ComposablePart.ImportDefinitions"/> property describing the import to be set.</param>
        /// <param name="exports">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> of <see cref="T:System.ComponentModel.Composition.Primitives.Export"/> objects of which
        /// to set the import described by <paramref name="definition"/>.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="definition"/> is <see langword="null"/>.
        /// <para>
        /// -or-
        /// </para>
        /// 	<paramref name="exports"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// 	<paramref name="definition"/> did not originate from the <see cref="P:System.ComponentModel.Composition.Primitives.ComposablePart.ImportDefinitions"/>
        /// property on the <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart"/>.
        /// <para>
        /// -or-
        /// </para>
        /// 	<paramref name="exports"/> contains an element that is <see langword="null"/>.
        /// <para>
        /// -or-
        /// </para>
        /// 	<paramref name="exports"/> is empty and <see cref="P:System.ComponentModel.Composition.Primitives.ImportDefinition.Cardinality"/> is
        /// <see cref="F:System.ComponentModel.Composition.Primitives.ImportCardinality.ExactlyOne"/>.
        /// <para>
        /// -or-
        /// </para>
        /// 	<paramref name="exports"/> contains more than one element and
        /// <see cref="P:System.ComponentModel.Composition.Primitives.ImportDefinition.Cardinality"/> is <see cref="F:System.ComponentModel.Composition.Primitives.ImportCardinality.ZeroOrOne"/> or
        /// <see cref="F:System.ComponentModel.Composition.Primitives.ImportCardinality.ExactlyOne"/>.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// 	<see cref="M:System.ComponentModel.Composition.Primitives.ComposablePart.Activate"/> has been previously called and
        /// <see cref="P:System.ComponentModel.Composition.Primitives.ImportDefinition.IsRecomposable"/> is <see langword="false"/>.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// The <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart"/> has been disposed of.
        /// </exception>
        /// <exception cref="T:System.ComponentModel.Composition.Primitives.ComposablePartException">
        /// An error occurred setting the import described by the <see cref="T:System.ComponentModel.Composition.Primitives.ImportDefinition"/>.
        /// </exception>
        public override void SetImport(ImportDefinition definition, IEnumerable<Export> exports)
        {
            _innerPart.SetImport(definition, exports);
        }

        private object CreateInstance()
        {
            var factory = ServiceLocator.Current.GetInstance<IProxyFactory>();

            return factory.CreateProxy(
                _implementation,
                _implementation.GetAttributes<IBehavior>(true).ToArray(),
                DetermineConstructorArgs()
                );
        }

        private object[] DetermineConstructorArgs()
        {
            var args = new List<object>();
            var constructorInfo = _implementation.SelectEligibleConstructor();

            if(constructorInfo != null)
            {
                foreach(var info in constructorInfo.GetParameters())
                {
                    var arg = ServiceLocator.Current.GetInstance(info.ParameterType);
                    args.Add(arg);
                }
            }

            return args.ToArray();
        }
    }
}