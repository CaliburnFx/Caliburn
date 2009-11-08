namespace Caliburn.PresentationFramework.ApplicationModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Core;
    using Microsoft.Practices.ServiceLocation;
    using Core.Metadata;
    using Metadata;

    /// <summary>
    /// The default implementation of <see cref="IViewStrategy"/>.
    /// </summary>
    public class DefaultViewStrategy : IViewStrategy
    {
        private readonly IAssemblySource _assemblySource;
        private readonly IServiceLocator _serviceLocator;
        private readonly Dictionary<string, Type> _cache = new Dictionary<string, Type>();
        private readonly Dictionary<string, string> _namespaceAliases = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewStrategy"/> class.
        /// </summary>
        /// <param name="assemblySource">The assembly source.</param>
        /// <param name="serviceLocator">The service locator.</param>
        public DefaultViewStrategy(IAssemblySource assemblySource, IServiceLocator serviceLocator)
        {
            _assemblySource = assemblySource;
            _serviceLocator = serviceLocator;
        }

        /// <summary>
        /// Allows for the explicit mapping of a model's namespace to a view's namespace. This overrides the implicit mapping.
        /// </summary>
        /// <param name="modelNamespace">The namespace that the model resides in.</param>
        /// <param name="viewNamespace">the namespace that the view resides in.</param>
        public void AddNamespaceAlias(string modelNamespace, string viewNamespace)
        {
            _namespaceAliases[modelNamespace] = viewNamespace;
        }

        /// <summary>
        /// Gets the view for displaying the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="displayLocation">The control into which the view will be injected.</param>
        /// <param name="context">Some additional context used to select the proper view.</param>
        /// <returns></returns>
        public object GetView(object model, DependencyObject displayLocation, object context)
        {
            if (model == null)
                return null;

#if !SILVERLIGHT
            var metadataContainer = model as IMetadataContainer;
            if (metadataContainer != null)
            {
                var view = metadataContainer.GetView<object>(context);
                if (view != null) return view;
            }
#endif

            var modelType = GetModelType(model);

            var customStrategy = modelType.GetCustomAttributes(typeof(ViewStrategyAttribute), true)
                .OfType<ViewStrategyAttribute>().Where(x => x.Matches(context)).FirstOrDefault();

            if (customStrategy != null)
                return customStrategy.GetView(model, displayLocation, context);

            var stringContext = context.SafeToString();
            var cacheKey = DetermineCacheKey(modelType, stringContext);

            if (_cache.ContainsKey(cacheKey))
                return GetOrCreateViewFromType(_cache[cacheKey]);
            
            var namesToCheck = GetTypeNamesToCheck(modelType, stringContext);

            foreach(var name in namesToCheck)
            {
                foreach (var assembly in _assemblySource.Union(new[] { modelType.Assembly }))
                {
                    var type = assembly.GetType(name, false);
                    if(type == null) continue;

                    var view = GetOrCreateViewFromType(type);
                    if (view == null) continue;

                    _cache[cacheKey] = type;

                    return view;
                }
            }

            throw new CaliburnException("A default view was not found for " + modelType.FullName + ".");
        }

        /// <summary>
        /// Gets the type of the model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        protected virtual Type GetModelType(object model)
        {
            return model.GetType();
        }

        /// <summary>
        /// Determines the cache key.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="context">The context.</param>
        protected virtual string DetermineCacheKey(Type modelType, string context)
        {
            if (string.IsNullOrEmpty(context))
                return modelType.FullName;
            return modelType.FullName + "|" + context;
        }

        /// <summary>
        /// Queries the service locator for a view matching the type. If one is not found, it attempts to instantiate the type.
        /// If both options fail, it returns null.
        /// </summary>
        /// <param name="type">The candidate type for the view.</param>
        /// <returns>An instance of a view or null.</returns>
        protected object GetOrCreateViewFromType(Type type)
        {
            var view = _serviceLocator.GetAllInstances(type)
                .FirstOrDefault();

            if(view != null)
                return view;

            if (type.IsInterface || type.IsAbstract) 
                return null;

            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// Gets the type names to check for view implementations.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="context">Some additional context used to select the proper view.</param>
        /// <returns></returns>
        protected virtual IEnumerable<string> GetTypeNamesToCheck(Type modelType, string context)
        {
            //starts as: Namespace.ViewModels.SomethingViewModel

            var keywords = GetSingularKeywords();

            foreach(var word in keywords)
            {
                if(!modelType.FullName.Contains(MakeNamespacePart(word)) && !string.IsNullOrEmpty(word))
                    continue;

                var options = ReplaceWithView(
                    modelType,
                    word
                    );

                if(!string.IsNullOrEmpty(context))
                {
                    foreach(var option in options)
                    {
                        var name = option + "s." + context;
                        yield return MakeInterface(name); //Namespace.Views.SomethingViews.IEdit
                        yield return name; //Namespace.Views.SomethingViews.Edit
                    }
                }
                else
                {
                    foreach(var option in options)
                    {
                        yield return MakeInterface(option); //Namespace.Views.ISomethingView
                        yield return option; //Namespace.Views.SomethingView

                        var name = option + "s." + "Default";
                        yield return MakeInterface(name); //Namespace.Views.SomethingViews.IDefault
                        yield return name; //Namespace.Views.SomethingViews.Default
                    }
                }
            }
        }

        /// <summary>
        /// Gets the keywords used for namespace/type search and replace.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<string> GetSingularKeywords()
        {
            return new[]
            {
                "Presenter",
                "Model",
                "ViewModel",
                "PresentationModel",
                string.Empty
            };
        }

        /// <summary>
        /// Creates a set of possible type names based on the model type by replacing the toReplace text.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="toReplace">To replace.</param>
        /// <returns></returns>
        protected virtual IEnumerable<string> ReplaceWithView(Type modelType, string toReplace)
        {
            var part = modelType.FullName.Replace(MakeNamespacePart(toReplace), ".Views");

            foreach (var pair in _namespaceAliases)
            {
                if (part.StartsWith(pair.Key))
                {
                    part = part.Replace(pair.Key, pair.Value);
                    break;
                }
            }

            if (part.EndsWith(toReplace))
            {
                part = part.Substring(0, part.Length - toReplace.Length) + "View";
                yield return part;
            }
            else
            {
                yield return part;
            }
        }

        /// <summary>
        /// Makes a type name into an interface name.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <returns></returns>
        protected virtual string MakeInterface(string part)
        {
            var index = part.LastIndexOf(".");
            return part.Insert(index + 1, "I");
        }

        /// <summary>
        /// Makes a type name part into a namespace part.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <returns></returns>
        protected virtual string MakeNamespacePart(string part)
        {
            return "." + part + "s";
        }
    }
}