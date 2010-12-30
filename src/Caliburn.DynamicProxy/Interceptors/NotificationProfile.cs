namespace Caliburn.DynamicProxy.Interceptors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
	using Castle.DynamicProxy;
	using Core;
    using PresentationFramework.Behaviors;
    using PresentationFramework.Filters;

    /// <summary>
    /// Stores information about how property change notification should work for a particular type.
	/// </summary>
#if NET
	[System.Serializable]
#endif
	public class NotificationProfile
    {
        static readonly Dictionary<Type, NotificationProfile> Profiles =
            new Dictionary<Type, NotificationProfile>();

        static readonly object CreationLock = new object();

        readonly List<string> recorded = new List<string>();
        readonly List<string> ignores = new List<string>();
        readonly Dictionary<string, IList<string>> dependencies = new Dictionary<string, IList<string>>();
        readonly List<string> getters = new List<string>();
        readonly object recordLock = new object();
        readonly DependencyMode dependencyMode;

        /// <summary>
        /// Gets the specified profile for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="behavior">The behavior.</param>
        /// <returns>The profile.</returns>
        public static NotificationProfile Get(Type type, NotifyPropertyChangedAttribute behavior)
        {
            NotificationProfile profile;

            if(!Profiles.TryGetValue(type, out profile))
            {
                lock(CreationLock)
                {
                    if(!Profiles.TryGetValue(type, out profile))
                    {
                        Profiles[type] = profile = new NotificationProfile(type, behavior);
                    }
                }
            }

            return profile;
        }

        private NotificationProfile(Type type, NotifyPropertyChangedAttribute behavior)
        {
            dependencyMode = behavior.DependencyMode;

            var properties = type.GetProperties();

            ignores = (from property in properties
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

                if (!recorded.Contains(dependent.Property))
                    recorded.Add(dependent.Property);
            }
        }

        /// <summary>
        /// Handles the getter.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="invocation">The invocation.</param>
        public void HandleGetter(string propertyName, IInvocation invocation)
        {
            switch (dependencyMode)
            {
                case DependencyMode.AlwaysRecord:
                    lock (recordLock)
                    {
                        try
                        {
                            getters.Add(propertyName);
                            invocation.Proceed();                            
                        }
                        finally
                        {
                            getters.Remove(propertyName);
                            RecordDependencies(propertyName);
                        }
                    }
                    break;
                case DependencyMode.RecordOnce:
                    if(!recorded.Contains(propertyName))
                    {
                        lock(recordLock)
                        {
                            if(!recorded.Contains(propertyName))
                            {
                                try
                                {
                                    getters.Add(propertyName);
                                    invocation.Proceed();
                                }
                                finally
                                {
                                    getters.Remove(propertyName);
                                    RecordDependencies(propertyName);
                                }
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
            return !ignores.Contains(propertyName);
        }

        /// <summary>
        /// Gets the properties which depend on the specified property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The dependent property names.</returns>
        public IEnumerable<string> GetDependencies(string propertyName)
        {
            IList<string> properties;

            if(!dependencies.TryGetValue(propertyName, out properties))
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
            if(getters.Count < 1)
                return;

            var dependencies = GetOrCreateDependencies(propertyName);

            foreach(var getter in getters)
            {
                if(!dependencies.Contains(getter))
                    dependencies.Add(getter);
            }

            if(!recorded.Contains(propertyName))
                recorded.Add(propertyName);
        }

        private IList<string> GetOrCreateDependencies(string propertyName)
        {
            IList<string> dependencies;

            if(!this.dependencies.TryGetValue(propertyName, out dependencies))
                this.dependencies[propertyName] = dependencies = new List<string>();

            return dependencies;
        }
    }
}