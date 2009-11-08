namespace Caliburn.Testability
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Represents a type that an item is bound to.
    /// </summary>
    public class BoundType
    {
        private Type _type;
        private string _basePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundType"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public BoundType(Type type)
            : this(type, string.Empty) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundType"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="basePath">The base path.</param>
        public BoundType(Type type, string basePath)
        {
            _type = type;
            _basePath = basePath;
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
        /// Validates the information against this type.
        /// </summary>
        /// <param name="element">The data bound element.</param>
        /// <param name="boundProperty">The bound property.</param>
        /// <param name="binding">The binding.</param>
        /// <returns></returns>
        public ValidatedProperty ValidateAgainst(IElement element, DependencyProperty boundProperty, Binding binding)
        {
            var propertyPath = binding.Path.Path;

            if(PathIsBinding(propertyPath))
            {
                return new ValidatedProperty(
                    AreCompatibleTypes(element, boundProperty, _type, binding),
                    GetFullPath(propertyPath)
                    );
            }

            if(propertyPath == "/")
            {
                _type = DeriveTypeOfCollection(_type);
                _basePath += "/";

                return new ValidatedProperty(
                    null,
                    GetFullPath(propertyPath)
                    );
            }

            var propertyInfo = GetProperty(propertyPath);

            if(propertyInfo == null)
            {
                return new ValidatedProperty(
                    Error.BadProperty(element, this, boundProperty, binding),
                    GetFullPath(propertyPath)
                    );
            }

            return new ValidatedProperty(
                AreCompatibleTypes(element, boundProperty, propertyInfo.PropertyType, binding),
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
            if(PathIsBinding(propertyPath))
                return this;

            var association = GetProperty(propertyPath);
            return association != null ? new BoundType(association.PropertyType, propertyPath) : null;
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="propertyPath">The property path.</param>
        /// <returns></returns>
        public PropertyInfo GetProperty(string propertyPath)
        {
            var currentType = _type;
            PropertyInfo currentInfo = null;

            for(int i = 0; i < propertyPath.Length; i++)
            {
                if(propertyPath[i] == '[')
                {
                    while(i < propertyPath.Length && propertyPath[i] != ']')
                        i++;

                    currentInfo = currentType.GetProperty("Item")
                                  ?? GetInterfaceProperty("Item", currentType);

                    if(currentInfo == null || i >= propertyPath.Length)
                        return currentInfo;

                    currentType = currentInfo.PropertyType;
                }
                else if(propertyPath[i] == '/')
                    currentType = DeriveTypeOfCollection(currentType);
                else
                {
                    if(propertyPath[i] == '.') i++;

                    string propertyName = string.Empty;

                    while(i < propertyPath.Length && Char.IsLetterOrDigit(propertyPath[i]))
                    {
                        propertyName += propertyPath[i];
                        i++;
                    }

                    currentInfo = currentType.GetProperty(
                                      propertyName,
                                      BindingFlags.DeclaredOnly | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance)
                                  ?? GetInterfaceProperty(propertyName, currentType)
                                     ?? currentType.GetProperties().Where(x => x.Name == propertyName).FirstOrDefault();

                    if(currentInfo == null || i >= propertyPath.Length)
                        return currentInfo;

                    currentType = currentInfo.PropertyType;
                    i--;
                }
            }

            return currentInfo;
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
            foreach(var t in interfaces)
            {
                var propertyInfo = t.GetProperty(propertyName) ?? GetInterfaceProperty(propertyName, t);
                if(propertyInfo != null)
                    return propertyInfo;
            }
            return null;
        }

        private string GetFullPath(string propertyPath)
        {
            if(string.IsNullOrEmpty(_basePath))
                return propertyPath;

            if(_basePath.EndsWith("/") || propertyPath.StartsWith("/"))
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
            if(boundProperty == null) return null;

            if(typeof(IEnumerable).IsAssignableFrom(boundProperty.PropertyType) &&
               boundProperty.PropertyType != typeof(string) &&
               !typeof(IEnumerable).IsAssignableFrom(propertyType))
                return Error.NotEnumerable(element, this, boundProperty, binding);

            return null;
        }
    }
}