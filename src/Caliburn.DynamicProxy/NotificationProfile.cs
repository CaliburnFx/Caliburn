namespace Caliburn.DynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Interceptor;
    using Core;
    using PresentationFramework.Behaviors;

    /// <summary>
    /// Stores information about how property change notification should work for a particular type.
    /// </summary>
    public class NotificationProfile
    {
        private static readonly Dictionary<Type, NotificationProfile> _profiles =
            new Dictionary<Type, NotificationProfile>();

        private static readonly object _creationLock = new object();

        private readonly List<string> _recorded = new List<string>();
        private readonly List<string> _ignores = new List<string>();
        private readonly Dictionary<string, List<string>> _dependencies = new Dictionary<string, List<string>>();
        private readonly List<string> _getters = new List<string>();
        private readonly object _recordLock = new object();

        /// <summary>
        /// Gets the specified profile for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The profile.</returns>
        public static NotificationProfile Get(Type type)
        {
            NotificationProfile profile;

            if(!_profiles.TryGetValue(type, out profile))
            {
                lock(_creationLock)
                {
                    if(!_profiles.TryGetValue(type, out profile))
                    {
                        _profiles[type] = profile = new NotificationProfile(type);
                    }
                }
            }

            return profile;
        }

        private NotificationProfile(Type type)
        {
            _ignores = (from property in type.GetProperties()
                        where property.GetAttributes<DoNotNotifyAttribute>(true).Any()
                        select property.Name).ToList();
        }

        /// <summary>
        /// Handles the getter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="invocation">The invocation.</param>
        public void HandleGetter(string propertyName, IInvocation invocation)
        {
            if(!_recorded.Contains(propertyName))
            {
                lock(_recordLock)
                {
                    if(!_recorded.Contains(propertyName))
                    {
                        _getters.Add(propertyName);
                        invocation.Proceed();
                        _getters.Remove(propertyName);

                        RecordDependencies(propertyName);
                    }
                    else invocation.Proceed();
                }
            }
            else invocation.Proceed();
        }

        /// <summary>
        /// Determines whether the interceptor should notify when this property is changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public bool ShouldNotify(string propertyName)
        {
            return !_ignores.Contains(propertyName);
        }

        /// <summary>
        /// Gets the properties which depend on the specified property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The dependent property names.</returns>
        public IEnumerable<string> GetDependencies(string propertyName)
        {
            List<string> properties;

            if(!_dependencies.TryGetValue(propertyName, out properties))
                properties = new List<string>();

            return properties;
        }

        private void RecordDependencies(string propertyName)
        {
            if(_getters.Count < 1)
                return;

            List<string> dependencies;

            if(!_dependencies.TryGetValue(propertyName, out dependencies))
                _dependencies[propertyName] = dependencies = new List<string>();

            foreach(var getter in _getters)
            {
                if(!dependencies.Contains(getter))
                    dependencies.Add(getter);
            }

            _recorded.Add(propertyName);
        }
    }
}