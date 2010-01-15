namespace Caliburn.DynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.Core.Interceptor;
    using Core;
    using PresentationFramework.Behaviors;
    using PresentationFramework.Filters;

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
        private readonly Dictionary<string, IList<string>> _dependencies = new Dictionary<string, IList<string>>();
        private readonly List<string> _getters = new List<string>();
        private readonly object _recordLock = new object();
        private readonly DependencyMode _dependencyMode;

        /// <summary>
        /// Gets the specified profile for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="behavior">The behavior.</param>
        /// <returns>The profile.</returns>
        public static NotificationProfile Get(Type type, NotifyPropertyChangedAttribute behavior)
        {
            NotificationProfile profile;

            if(!_profiles.TryGetValue(type, out profile))
            {
                lock(_creationLock)
                {
                    if(!_profiles.TryGetValue(type, out profile))
                    {
                        _profiles[type] = profile = new NotificationProfile(type, behavior);
                    }
                }
            }

            return profile;
        }

        private NotificationProfile(Type type, NotifyPropertyChangedAttribute behavior)
        {
            _dependencyMode = behavior.DependencyMode;

            var properties = type.GetProperties();

            _ignores = (from property in properties
                        where property.GetAttributes<DoNotNotifyAttribute>(true).Any()
                        select property.Name).ToList();

            var dependents = from property in properties
                             let dependencies = property.GetAttributes<DependenciesAttribute>(true)
                             where dependencies.Any()
                             from dependency in dependencies
                                 .SelectMany(x => x.Dependencies)
                                 .Distinct()
                             select new
                             {
                                 Property = property.Name,
                                 DependsOn = dependency
                             };

            foreach (var dependent in dependents)
            {
                GetOrCreateDependencies(dependent.DependsOn)
                    .Add(dependent.Property);

                if (!_recorded.Contains(dependent.Property))
                    _recorded.Add(dependent.Property);
            }
        }

        /// <summary>
        /// Handles the getter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="invocation">The invocation.</param>
        public void HandleGetter(string propertyName, IInvocation invocation)
        {
            switch (_dependencyMode)
            {
                case DependencyMode.AlwaysRecord:
                    _getters.Add(propertyName);
                    invocation.Proceed();
                    _getters.Remove(propertyName);
                    RecordDependencies(propertyName);
                    break;
                case DependencyMode.RecordOnce:
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
                    break;
                case DependencyMode.DoNotRecord:
                    invocation.Proceed();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
            IList<string> properties;

            if(!_dependencies.TryGetValue(propertyName, out properties))
                properties = new List<string>();

            foreach(var prop in properties)
            {
                yield return prop;

                foreach(var inner in GetDependencies(prop))
                {
                    yield return inner;
                }
            }
        }

        private void RecordDependencies(string propertyName)
        {
            if(_getters.Count < 1)
                return;

            var dependencies = GetOrCreateDependencies(propertyName);

            foreach(var getter in _getters)
            {
                if(!dependencies.Contains(getter))
                    dependencies.Add(getter);
            }

            _recorded.Add(propertyName);
        }

        private IList<string> GetOrCreateDependencies(string propertyName)
        {
            IList<string> dependencies;

            if(!_dependencies.TryGetValue(propertyName, out dependencies))
                _dependencies[propertyName] = dependencies = new List<string>();

            return dependencies;
        }
    }
}