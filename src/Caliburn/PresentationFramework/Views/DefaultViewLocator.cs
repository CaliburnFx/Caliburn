namespace Caliburn.PresentationFramework.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Interop;
    using Core;
    using Core.InversionOfControl;
    using Core.Logging;

    /// <summary>
    /// The default implementation of <see cref="IViewLocator"/>.
    /// </summary>
    public class DefaultViewLocator : IViewLocator
    {
        static readonly ILog Log = LogManager.GetLog(typeof(DefaultViewLocator));

        readonly IAssemblySource assemblySource;
        readonly IServiceLocator serviceLocator;
        readonly Dictionary<string, Type> cache = new Dictionary<string, Type>();
        readonly Dictionary<string, string> namespaceAliases = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewLocator"/> class.
        /// </summary>
        /// <param name="assemblySource">The assembly source.</param>
        /// <param name="serviceLocator">The service locator.</param>
        public DefaultViewLocator(IAssemblySource assemblySource, IServiceLocator serviceLocator)
        {
            this.assemblySource = assemblySource;
            this.serviceLocator = serviceLocator;
        }

        /// <summary>
        /// Allows for the explicit mapping of a model's namespace to a view's namespace. This overrides the implicit mapping.
        /// </summary>
        /// <param name="modelNamespace">The namespace that the model resides in.</param>
        /// <param name="viewNamespace">the namespace that the view resides in.</param>
        public void AddNamespaceAlias(string modelNamespace, string viewNamespace)
        {
            namespaceAliases[modelNamespace] = viewNamespace;
        }

        /// <summary>
        /// Locates the view for the specified model instance.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="displayLocation">The display location.</param>
        /// <param name="context">The context.</param>
        /// <returns>The view.</returns>
        public virtual DependencyObject LocateForModel(object model, DependencyObject displayLocation, object context)
        {
            if (model == null)
                return null;

            var viewAware = model as IViewAware;
            if (viewAware != null)
            {
                var view = viewAware.GetView(context) as DependencyObject;
                if (view != null)
                {
#if !SILVERLIGHT
                    var windowCheck = view as Window;
                    if (windowCheck == null || (!windowCheck.IsLoaded && !(new WindowInteropHelper(windowCheck).Handle == IntPtr.Zero)))
                    {
                        Log.Info("Cached view returned for {0}.", model);
                        return view;
                    }
#else
                    Log.Info("Cached view returned for {0}.", model);
                    return view;
#endif
                }
            }

            var createdView = LocateForModelType(model.GetModelType(), displayLocation, context);
            InitializeComponent(createdView);
            return createdView;
        }

        /// <summary>
        /// Locates the View for the specified model type.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="displayLocation">The display location.</param>
        /// <param name="context">The context.</param>
        /// <returns>The view.</returns>
        public virtual DependencyObject LocateForModelType(Type modelType, DependencyObject displayLocation, object context)
        {
            var customStrategy = modelType.GetAttributes<IViewStrategy>(true)
                .Where(x => x.Matches(context)).FirstOrDefault();

            if (customStrategy != null)
                return customStrategy.Locate(modelType, displayLocation, context);

            var stringContext = context.SafeToString();
            var cacheKey = DetermineCacheKey(modelType, stringContext);

            if (cache.ContainsKey(cacheKey))
                return GetOrCreateViewFromType(cache[cacheKey]);

            var namesToCheck = GetTypeNamesToCheck(modelType, stringContext).Distinct();

            foreach (var name in namesToCheck)
            {
                foreach (var assembly in new[] { modelType.Assembly }.Union(assemblySource))
                {
                    var type = assembly.GetType(name, false);
                    if (type == null) continue;

                    var view = GetOrCreateViewFromType(type);
                    if (view == null) continue;

                    cache[cacheKey] = type;

                    Log.Info("Located view {0} for {1}.", view, modelType);
                    return view;
                }
            }

            var message = namesToCheck.Aggregate(
                "A default view was not found for " + modelType.FullName + ".  Views searched for include: ",
                (a, c) => a + Environment.NewLine + c
                );

            Log.Warn(message);
            return new NotFoundView(message);
        }

        /// <summary>
        /// When a view does not contain a code-behind file, we need to automatically call InitializeCompoent.
        /// </summary>
        /// <param name="element">The element to initialize</param>
        public static void InitializeComponent(object element)
        {
            var method = element.GetType()
                .GetMethod("InitializeComponent", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if(method == null)
                return;

            method.Invoke(element, null);
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
            var view = serviceLocator.GetAllInstances(type)
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
            var modelTypeName = modelType.FullName;

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

                                foreach (var result in ProcessOption(context, secondPass))
                                {
                                    if (!result.Equals(modelTypeName))
                                        yield return result;
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
                    if (!result.Equals(modelTypeName))
                        yield return result;
                }
            }
        }

        IEnumerable<string> ProcessOption(string context, IEnumerable<string> options)
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
            foreach (var pair in namespaceAliases)
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
			string suffix = string.Empty;
			if (part.Contains("[[")) { 
				//generic type
				var genericParStart= part.IndexOf("[[");
				suffix = part.Substring(genericParStart);
				part = part.Remove(genericParStart);
				
			}

            var index = part.LastIndexOf(".");
            return part.Insert(index + 1, "I") + suffix;
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