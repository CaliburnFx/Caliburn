namespace Caliburn.PresentationFramework.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Core;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The default implementation of <see cref="IViewLocator"/>.
    /// </summary>
    public class DefaultViewLocator : IViewLocator
    {
        private readonly IAssemblySource _assemblySource;
        private readonly IServiceLocator _serviceLocator;
        private readonly Dictionary<string, Type> _cache = new Dictionary<string, Type>();
        private readonly Dictionary<string, string> _namespaceAliases = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewLocator"/> class.
        /// </summary>
        /// <param name="assemblySource">The assembly source.</param>
        /// <param name="serviceLocator">The service locator.</param>
        public DefaultViewLocator(IAssemblySource assemblySource, IServiceLocator serviceLocator)
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
        /// Locates the View for the specified model type.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="displayLocation">The display location.</param>
        /// <param name="context">The context.</param>
        /// <returns>The view.</returns>
        public DependencyObject Locate(Type modelType, DependencyObject displayLocation, object context)
        {
            var customStrategy = modelType.GetAttributes<IViewStrategy>(true)
                .Where(x => x.Matches(context)).FirstOrDefault();

            if (customStrategy != null)
                return customStrategy.Locate(modelType, displayLocation, context);

            var stringContext = context.SafeToString();
            var cacheKey = DetermineCacheKey(modelType, stringContext);

            if (_cache.ContainsKey(cacheKey))
                return GetOrCreateViewFromType(_cache[cacheKey]);

            var namesToCheck = GetTypeNamesToCheck(modelType, stringContext).Distinct();

            foreach (var name in namesToCheck)
            {
                foreach (var assembly in new[] { modelType.Assembly }.Union(_assemblySource))
                {
                    var type = assembly.GetType(name, false);
                    if (type == null) continue;

                    var view = GetOrCreateViewFromType(type);
                    if (view == null) continue;

                    _cache[cacheKey] = type;

                    return view;
                }
            }

            var message = namesToCheck.Aggregate(
                "A default view was not found for " + modelType.FullName + ".  Views searched for include: ", 
                (a, c) => a + Environment.NewLine + c
                );

            var generated = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            ToolTipService.SetToolTip(generated, message);

            return generated;
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
        protected DependencyObject GetOrCreateViewFromType(Type type)
        {
            var view = _serviceLocator.GetAllInstances(type)
                .FirstOrDefault() as DependencyObject;

            if (view != null)
                return view;

            if (type.IsInterface || type.IsAbstract || !typeof(DependencyObject).IsAssignableFrom(type))
                return null;

            return (DependencyObject)Activator.CreateInstance(type);
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

            foreach (var word in keywords)
            {
                if (!modelType.FullName.Contains(MakeNamespacePart(word)) && !string.IsNullOrEmpty(word))
                {
                    foreach (var w in keywords)
                    {
                        var firstPass = ReplaceWithView(
                            modelType.FullName.Replace(MakeNamespacePart(w), ".Views"),
                            w
                            );

                        foreach(var pass in firstPass)
                        {
                            foreach (var w2 in keywords)
                            {
                                var secondPass = ReplaceWithView(pass, w2);

                                foreach(var pass2 in secondPass)
                                {
                                    foreach (var result in ProcessOption(context, secondPass))
                                    {
                                        yield return result;
                                    }
                                }
                            }
                        }
                    }

                    continue;
                }

                var options = ReplaceWithView(
                    modelType.FullName.Replace(MakeNamespacePart(word), ".Views"),
                    word
                    );

                foreach(var result in ProcessOption(context, options))
                {
                    yield return result;
                }
            }
        }

        private IEnumerable<string> ProcessOption(string context, IEnumerable<string> options)
        {
            if (!string.IsNullOrEmpty(context))
            {
                foreach (var option in options)
                {
                    var name = option + "Views." + context;
                    yield return MakeInterface(name);
                    yield return name;

                    name = !option.EndsWith("s") ? option + "s." + context : option + "." + context;
                    yield return MakeInterface(name); //Namespace.Views.SomethingViews.IEdit
                    yield return name; //Namespace.Views.SomethingViews.Edit
                }
            }
            else
            {
                foreach (var option in options)
                {
                    yield return MakeInterface(option); //Namespace.Views.ISomethingView
                    yield return option; //Namespace.Views.SomethingView

                    var name = option + "s." + "Default";
                    yield return MakeInterface(name); //Namespace.Views.SomethingViews.IDefault
                    yield return name; //Namespace.Views.SomethingViews.Default
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
                "ViewModel",
                "Screen",
                "Presenter",
                "PresentationModel",
                "Model",
                string.Empty
            };
        }

        /// <summary>
        /// Creates a set of possible type names based on the model type by replacing the toReplace text.
        /// </summary>
        /// <param name="part">The model name.</param>
        /// <param name="toReplace">To replace.</param>
        /// <returns></returns>
        protected virtual IEnumerable<string> ReplaceWithView(string part, string toReplace)
        {
            foreach (var pair in _namespaceAliases)
            {
                if (part.StartsWith(pair.Key))
                {
                    part = part.Replace(pair.Key, pair.Value);
                    break;
                }
            }

            if (string.IsNullOrEmpty(toReplace))
            {
                foreach (var keyword in GetSingularKeywords().Except(new[] { string.Empty }))
                {
                    if (part.EndsWith(keyword))
                        yield return part.Remove(part.LastIndexOf(keyword)) + "View";
                }
            }
            else if (part.EndsWith(toReplace))
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