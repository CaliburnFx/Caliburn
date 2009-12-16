namespace Caliburn.PresentationFramework.Filters
{
    using System;
    using System.Reflection;
    using Core.Invocation;
    using Core.Metadata;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// A base class for filters that make method calls.
    /// </summary>
    public class MethodCallFilterBase : Attribute, IInitializable
    {
        /// <summary>
        /// The method.
        /// </summary>
        protected IMethod _method;
        private readonly string _methodName;
        private IMetadataContainer _target;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCallFilterBase"/> class.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        public MethodCallFilterBase(string methodName)
        {
            _methodName = methodName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCallFilterBase"/> class.
        /// </summary>
        public MethodCallFilterBase(IMethod method)
        {
            _method = method;
            _methodName = IsGetter ? method.Info.Name.Remove(0, 4) : method.Info.Name;
        }

        /// <summary>
        /// Gets the order the filter will be evaluated in.
        /// </summary>
        /// <value>The order.</value>
        public int Priority { get; set; }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        /// <value>The name of the method.</value>
        public string MethodName
        {
            get { return _methodName; }
        }

        /// <summary>
        /// Indicates whether the preview method is actually a property getter.
        /// </summary>
        /// <value><c>true</c> if this instance points to a getter; otherwise, <c>false</c>.</value>
        protected bool IsGetter
        {
            get { return _method.Info.IsSpecialName && _method.Info.Name.StartsWith("get_"); }
        }

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        protected IMetadataContainer Target
        {
            get { return _target; }
        }

        /// <summary>
        /// Initializes the filter.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="metadataContainer">The metadata container.</param>
        /// <param name="serviceLocator">The serviceLocator.</param>
        public virtual void Initialize(Type targetType, IMetadataContainer metadataContainer, IServiceLocator serviceLocator)
        {
            _target = metadataContainer;

            if (_method != null)
                return;

            var methodInfo = targetType.GetMethod(_methodName,
                                                  BindingFlags.Public | BindingFlags.Instance |
                                                  BindingFlags.Static)
                             ?? targetType.GetMethod("get_" + _methodName,
                                                     BindingFlags.Public | BindingFlags.Instance |
                                                     BindingFlags.Static);

            if (methodInfo == null)
                throw new Exception(
                    string.Format("Method or property '{0}' could not be found on '{1}'.", _methodName, targetType.FullName)
                    );

            _method = serviceLocator.GetInstance<IMethodFactory>().CreateFrom(methodInfo);
        }
    }
}