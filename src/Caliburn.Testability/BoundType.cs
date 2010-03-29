namespace Caliburn.Testability
{
	using System;
	using System.Collections;
	using System.Linq;
	using System.Reflection;
	using System.Windows;
	using System.Windows.Data;
	using System.Collections.Generic;

	/// <summary>
	/// Represents a type that an item is bound to.
	/// </summary>
	public class BoundType
	{
		private Type _type;
		private string _basePath;
		private IDictionary<string, Type> _hints;

		/// <summary>
		/// Initializes a new instance of the <see cref="BoundType"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		public BoundType(Type type)
			: this(type, string.Empty) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="BoundType"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="basePath">The base path.</param>
		public BoundType(Type type, string basePath) 
			: this (type, basePath, new Dictionary<string, Type>()) {}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="BoundType"/> class.
		/// Used internally to create an associated BoundType from a parent one.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="basePath">The base path.</param>
		/// /// <param name="hints">Hints propagated from parent.</param>
		public BoundType(Type type, string basePath, IDictionary<string, Type> hints)
		{
			_type = type;
			_basePath = basePath;
			_hints = hints;
		}

		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value>The type.</value>
		public Type Type
		{
			get { return _type; }
		}

        /// <summary>
        /// Adds a hint for polymorphic type checking.
        /// </summary>
        /// <param name="propertyPath">The property path.</param>
        /// <param name="hint">The hint.</param>
		public void AddHint(string propertyPath, Type hint) {
			if (_hints.ContainsKey(propertyPath))
				throw new Caliburn.Core.CaliburnException(string.Format("Hint for path '{0}' was already added", propertyPath));

			//TODO: validate path part names
			//TODO: validate hint congruency with reflected property type 

			_hints.Add(propertyPath, hint);
		}

		/// <summary>
		/// Validates the information against this type.
		/// </summary>
		/// <param name="element">The data bound element.</param>
		/// <param name="boundProperty">The bound property.</param>
		/// <param name="binding">The binding.</param>
		/// <returns></returns>
		public ValidatedProperty ValidateAgainst(IElement element, DependencyProperty boundProperty, Binding binding)
		{
			var propertyPath = binding.Path.Path;

			if (PathIsBinding(propertyPath))
			{
				return new ValidatedProperty(
					AreCompatibleTypes(element, boundProperty, _type, binding),
					GetFullPath(propertyPath)
					);
			}

			if (propertyPath == "/")
			{
				_type = DeriveTypeOfCollection(_type);
				_basePath += "/";

				return new ValidatedProperty(
					null,
					GetFullPath(propertyPath)
					);
			}

			var propertyType = GetPropertyType(propertyPath);

			if (propertyType == null)
			{
				return new ValidatedProperty(
					Error.BadProperty(element, this, boundProperty, binding),
					GetFullPath(propertyPath)
					);
			}

			return new ValidatedProperty(
				AreCompatibleTypes(element, boundProperty, propertyType, binding),
				GetFullPath(propertyPath)
				);
		}

		/// <summary>
		/// Gets a type by association.
		/// </summary>
		/// <param name="propertyPath">The property path.</param>
		/// <returns></returns>
		public BoundType GetAssociatedType(string propertyPath)
		{
			if (PathIsBinding(propertyPath))
				return this;

			var associationType = GetPropertyType(propertyPath);
			return associationType != null ? new BoundType(associationType, propertyPath, GetHintsToPropagate()) : null;
		}

		private IDictionary<string, Type> GetHintsToPropagate() {
			return _hints
				.Select(pair => new KeyValuePair<string, Type>(
						StripLeadingPathPart(pair.Key),
						pair.Value
					))
				.Where(pair => !string.IsNullOrEmpty(pair.Key))
				.ToDictionary(pair => pair.Key, pair => pair.Value); 
		}
		private string StripLeadingPathPart(string propertyPath) {
			var dotIndex = propertyPath.IndexOf('.');
			if (dotIndex < 0) return null;
			return propertyPath.Substring(dotIndex + 1);
		}

		/// <summary>
		/// Gets the type of property.
		/// </summary>
		/// <param name="propertyPath">The property path.</param>
		/// <returns></returns>
		public Type GetPropertyType(string propertyPath) 
		{
			var currentType = _type;
			var currentPrefixForHintLookup = string.Empty;
		 
			for (int i = 0; i < propertyPath.Length; i++)
			{
				if (propertyPath[i] == '[')
				{
					while (i < propertyPath.Length && propertyPath[i] != ']')
						i++;

					var info = currentType.GetProperty("Item")
								  ?? GetInterfaceProperty("Item", currentType);

					if (info == null) return null;

					currentType = info.PropertyType;
					currentPrefixForHintLookup += "Item.";
					if (i >= propertyPath.Length) return currentType;
				}
				else if (propertyPath[i] == '/')
					currentType = DeriveTypeOfCollection(currentType);
				else
				{
					if (propertyPath[i] == '.') i++;

					string propertyName = string.Empty;

					while (i < propertyPath.Length && IsCharValidInPropertyName(propertyPath[i]))
					{
						propertyName += propertyPath[i];
						i++; 
					}

					Type hint;
					if (_hints.TryGetValue(currentPrefixForHintLookup + propertyName, out hint))
					{
						currentType = hint;
					}
					else
					{
						var info = currentType.GetProperty(
										  propertyName,
										  BindingFlags.DeclaredOnly | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance)
									  ?? GetInterfaceProperty(propertyName, currentType)
										 ?? currentType.GetProperties().Where(x => x.Name == propertyName).FirstOrDefault();

						if (info == null) return null;

						currentType = info.PropertyType;
					}

					currentPrefixForHintLookup += propertyName + ".";
					if (i >= propertyPath.Length) return currentType;
					i--;
				}
			}

			return currentType;
		}

		private static bool IsCharValidInPropertyName(char c)
		{
			return Char.IsLetterOrDigit(c) || c == '_';
		}

		/// <summary>
		/// Derives the type of the collection.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <returns></returns>
		private static Type DeriveTypeOfCollection(Type collection)
		{
			return (from i in collection.GetInterfaces()
					where typeof(IEnumerable).IsAssignableFrom(i)
						  && i.IsGenericType
					select i.GetGenericArguments()[0]).FirstOrDefault();
		}

		/// <summary>
		/// Gets the interface property.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		private static PropertyInfo GetInterfaceProperty(string propertyName, Type type)
		{
			var interfaces = type.GetInterfaces();
			foreach (var t in interfaces)
			{
				var propertyInfo = t.GetProperty(propertyName) ?? GetInterfaceProperty(propertyName, t);
				if (propertyInfo != null)
					return propertyInfo;
			}
			return null;
		}

		private string GetFullPath(string propertyPath)
		{
			if (string.IsNullOrEmpty(_basePath))
				return propertyPath;

			if (_basePath.EndsWith("/") || propertyPath.StartsWith("/"))
				return (_basePath + propertyPath).Replace("//", "/");

			return _basePath + "." + propertyPath;
		}

		private bool PathIsBinding(string propertyPath)
		{
			return string.IsNullOrEmpty(propertyPath) || propertyPath == ".";
		}

		private IError AreCompatibleTypes(IElement element, DependencyProperty boundProperty, Type propertyType,
										  Binding binding)
		{
			if (boundProperty == null) return null;

			if (typeof(IEnumerable).IsAssignableFrom(boundProperty.PropertyType) &&
			   boundProperty.PropertyType != typeof(string) &&
			   !typeof(IEnumerable).IsAssignableFrom(propertyType))
				return Error.NotEnumerable(element, this, boundProperty, binding);

			return null;
		}
	}


 
}