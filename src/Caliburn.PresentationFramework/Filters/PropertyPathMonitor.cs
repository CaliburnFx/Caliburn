namespace Caliburn.PresentationFramework.Filters
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using Core;
    using Core.Invocation;
    using Core.MemoryManagement;

    public class PropertyPathMonitor : IDisposable
    {
        private const string ALL_PROPERTIES = "*";

        private readonly IMethodFactory _methodFactory;
        private WeakReference<object> _targetReference;
        private WeakReference<Action> _callbackReference;

        private readonly string _propertyPath;
        private readonly string _observedPropertyName;
        private IMethod _propertyGetMethod;

        private readonly string _subPath;
        private PropertyPathMonitor _subPathMonitor;

        public PropertyPathMonitor(IMethodFactory methodFactory, object target, string propertyPath,
                                   Action onPathChanged)
        {
            _methodFactory = methodFactory;

            if (target == null) throw new ArgumentNullException("target");
            _targetReference = new WeakReference<object>(target);

            if (onPathChanged == null) throw new ArgumentNullException("onPathChanged");
            _callbackReference = new WeakReference<Action>(onPathChanged);

            _propertyPath = propertyPath;
            _observedPropertyName = GetRootProperty(_propertyPath);
            _subPath = GetSubPath(_propertyPath);

            StartMonitoring();
        }

        private void StartMonitoring()
        {
            var notifier = GetTarget() as INotifyPropertyChanged;
            if (notifier != null)
                notifier.PropertyChanged += Notifier_PropertyChanged;

            HookSubpathMonitor();
        }

        private void HookSubpathMonitor()
        {
            if (_subPathMonitor != null)
                _subPathMonitor.Dispose();

            if (string.IsNullOrEmpty(_subPath)) return;

            var getter = GetPropertyGetMethod();

            var subTarget = getter.Invoke(GetTargetOrFail());
            if (subTarget != null)
            {
                _subPathMonitor = new PropertyPathMonitor(_methodFactory, subTarget, _subPath, OnSubPathChanged);
            }
        }

        private void OnSubPathChanged()
        {
            NotifyChange();
        }

        private void Notifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ShouldNotify(e.PropertyName))
            {
                NotifyChange();
            }

            HookSubpathMonitor();
        }

        private void NotifyChange()
        {
            if (_callbackReference != null && _callbackReference.IsAlive)
                _callbackReference.Target.Invoke();
        }

        private object GetTarget()
        {
            if (!_targetReference.IsAlive) return null;
            return _targetReference.Target;
        }

        private object GetTargetOrFail()
        {
            if (!_targetReference.IsAlive) throw new CaliburnException("Target is no longer available");
            return _targetReference.Target;
        }

        private bool ShouldNotify(string propertyName)
        {
            return _observedPropertyName == ALL_PROPERTIES || _observedPropertyName.Equals(propertyName);
        }

        private IMethod GetPropertyGetMethod()
        {
            if (_propertyGetMethod == null)
            {
                if (_observedPropertyName.Equals(ALL_PROPERTIES))
                    throw new CaliburnException(
                        string.Format(
                            "'{0}' marker in path {1} is invalid. '{0}' can only be used as leaf property in a path",
                            ALL_PROPERTIES, _propertyPath));

                var type = GetTargetOrFail().GetType();
                var propInfo = type.GetProperty(_observedPropertyName, BindingFlags.Instance | BindingFlags.Public);

                if (propInfo == null)
                {
                    throw new CaliburnException(string.Format("Cannot find property {0} of path {1} in class {2}",
                                                              _observedPropertyName, _propertyPath, type.FullName
                                                    ));
                }

                _propertyGetMethod = _methodFactory.CreateFrom(propInfo.GetGetMethod());
            }

            return _propertyGetMethod;
        }

        private static string GetRootProperty(string propertyPath)
        {
            var index = propertyPath.IndexOf(".");
            if (index < 0) index = propertyPath.Length;

            return propertyPath.Substring(0, index);
        }

        private static string GetSubPath(string propertyPath)
        {
            var index = propertyPath.IndexOf(".");
            if (index < 0 || index >= propertyPath.Length) return null;

            return propertyPath.Substring(index + 1);
        }

        public void Dispose()
        {
            _callbackReference = null;
            _targetReference = null;

            if (_subPathMonitor != null)
                _subPathMonitor.Dispose();
        }
    }
}