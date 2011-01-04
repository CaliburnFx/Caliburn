namespace Caliburn.MEF
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.ComponentModel.Composition.Primitives;
	using System.ComponentModel.Composition.ReflectionModel;
	using System.Linq;
	using System.Reflection;
	using Core.InversionOfControl;

	/// <summary>
	/// A <see cref="ComposablePart"/> used to configure MEF with Caliburn's required services.
	/// </summary>
	public class ComponentPart : ComposablePart
	{
		readonly ComponentRegistrationBase registration;
		readonly List<ImportDefinition> imports = new List<ImportDefinition>();
		ExportDefinition[] exports;

		readonly Dictionary<ImportDefinition, Export> satisfiedImports =
			new Dictionary<ImportDefinition, Export>();

		object cachedInstance;
		readonly ConstructorInfo greedyConstructor;

		/// <summary>
		/// Initializes a new instance of the <see cref="ComponentPart"/> class.
		/// </summary>
		/// <param name="registration">The registration.</param>
		public ComponentPart(ComponentRegistrationBase registration)
		{
			this.registration = registration;

			var implementation = GetImplementation(registration);
			
			var instanceReg = registration as Instance;
			if (instanceReg != null)
			{
				cachedInstance = instanceReg.Implementation;
			}
			else
			{
				greedyConstructor = implementation
					.SelectEligibleConstructor();
				ConfigureImportDefinitions();
			}

			ConfigureExportDefinitions(implementation, registration.Service);
		}

		private void ConfigureExportDefinitions(Type implementationType, Type contractType)
		{
			var lazyMember = new LazyMemberInfo(implementationType);
			var contractName = !registration.HasName()
								  ? AttributedModelServices.GetContractName(registration.Service)
								  : registration.Name;

			var metadata = new Lazy<IDictionary<string, object>>(() =>
			{
				var md = new Dictionary<string, object>();
				md.Add(CompositionConstants.ExportTypeIdentityMetadataName,
					   AttributedModelServices.GetTypeIdentity(contractType));
				return md;
			});

			exports = new[] { ReflectionModelServices.CreateExportDefinition(lazyMember, contractName, metadata, null) };
		}

		private void ConfigureImportDefinitions()
		{
			foreach (var param in greedyConstructor.GetParameters())
			{
				var cardinality = GetCardinality(param.ParameterType);
				var importType = cardinality == ImportCardinality.ZeroOrMore
									 ? GetCollectionContractType(param.ParameterType)
									 : param.ParameterType;

				imports.Add(
					ReflectionModelServices.CreateImportDefinition(
						new Lazy<ParameterInfo>(() => param),
						AttributedModelServices.GetContractName(importType),
						AttributedModelServices.GetTypeIdentity(importType),
						Enumerable.Empty<KeyValuePair<string, Type>>(),
						cardinality,
						CreationPolicy.Any,
						null
						)
					);
			}
		}

		private static Type GetCollectionContractType(Type collectionType)
		{
			return collectionType.GetElementType();
		}

		private static ImportCardinality GetCardinality(Type targetType)
		{
			if (targetType.IsArray)
				return ImportCardinality.ZeroOrMore;
			return ImportCardinality.ExactlyOne;
		}

		/// <summary>
		/// Gets the exported object described by the specified definition.
		/// </summary>
		/// <param name="definition">One of the <see cref="T:System.ComponentModel.Composition.Primitives.ExportDefinition"/> objects from the
		/// <see cref="P:System.ComponentModel.Composition.Primitives.ComposablePart.ExportDefinitions"/> property describing the exported object
		/// to return.</param>
		/// <returns>
		/// The exported <see cref="T:System.Object"/> described by <paramref name="definition"/>.
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
		/// An error occurred getting the exported object described by the <see cref="T:System.ComponentModel.Composition.Primitives.ExportDefinition"/>.
		/// </exception>
		public override object GetExportedValue(ExportDefinition definition)
		{
		    if (registration is PerRequest)
				return CreateInstance(definition);

		    return cachedInstance ?? (cachedInstance = CreateInstance(definition));
		}

	    /// <summary>
		/// Creates the instance.
		/// </summary>
		/// <param name="definition">The definition.</param>
		/// <returns></returns>
		private object CreateInstance(ExportDefinition definition)
		{
			var args = new List<object>();

			foreach (var parameterInfo in greedyConstructor.GetParameters())
			{
				var arg = (from export in satisfiedImports.Values
						   where export.Definition.ContractName ==
								 AttributedModelServices.GetContractName(parameterInfo.ParameterType)
						   select export).FirstOrDefault();

				args.Add(arg.Value);
			}

			var instance = args.Count > 0
					   ? Activator.CreateInstance(GetImplementation(registration), args.ToArray())
					   : Activator.CreateInstance(GetImplementation(registration));

			IoC.Get<CompositionContainer>().SatisfyImportsOnce(instance);

			return instance;
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
		/// 	<see cref="M:System.ComponentModel.Composition.Primitives.ComposablePart.OnComposed"/> has been previously called and
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
			if (definition == null) throw new ArgumentNullException("definition");
			if (exports == null) throw new ArgumentNullException("exports");

			satisfiedImports[definition] = exports.FirstOrDefault();
		}

		/// <summary>
		/// Gets the export definitions that describe the exported objects provided by the part.
		/// </summary>
		/// <value>
		/// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> of <see cref="T:System.ComponentModel.Composition.Primitives.ExportDefinition"/> objects describing
		/// the exported objects provided by the <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart"/>.
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
			get { return exports; }
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
			get { return imports; }
		}

		private static Type GetImplementation(IComponentRegistration registration)
		{
			var singleton = registration as Singleton;
			if (singleton != null) return singleton.Implementation;

			var perRequest = registration as PerRequest;
			if (perRequest != null) return perRequest.Implementation;

			var instance = registration as Instance;
			if (instance != null && instance.Implementation != null) return instance.Implementation.GetType();
			
			throw new NotSupportedException();
		}
	}
}