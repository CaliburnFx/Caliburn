namespace Caliburn.PresentationFramework.Filters
{
	using System;
	using System.ComponentModel;
	using System.Reflection;
	using Core;
	using Core.Invocation;

	/// <summary>
	/// A class used to monitor changes in a property path.
	/// </summary>
	public class PropertyPathMonitor : IDisposable
	{
		private const string ALL_PROPERTIES = "*";

		private readonly IMethodFactory _methodFactory;
		private WeakReference _notifierRef;
		private WeakReference _onPathChangedActionRef;

		private readonly string _propertyPath;
		private readonly string _observedPropertyName;
		private IMethod _propertyGetMethod;

		private readonly string _subPath;
		private PropertyPathMonitor _subPathMonitor;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyPathMonitor"/> class.
		/// </summary>
		/// <param name="methodFactory">The method factory.</param>
		/// <param name="notifier">The notifier.</param>
		/// <param name="propertyPath">The property path.</param>
		/// <param name="onPathChanged">The on path changed.</param>
		public PropertyPathMonitor(IMethodFactory methodFactory, INotifyPropertyChanged notifier, string propertyPath, Action onPathChanged)
		{
			_methodFactory = methodFactory;
			_notifierRef = new WeakReference(notifier);
			_onPathChangedActionRef = new WeakReference(onPathChanged);
			_propertyPath = propertyPath;

			_observedPropertyName = GetRootProperty(_propertyPath);
			_subPath = GetSubPath(_propertyPath);

			VerifyObservedPropertyExists();

			notifier.PropertyChanged += Notifier_PropertyChanged;
			HookSubpathMonitor();
		}

		private void VerifyObservedPropertyExists()
		{
			if (_observedPropertyName.Equals(ALL_PROPERTIES))
				return;

			var type = GetNotifier(true).GetType();

			var property = type.GetProperty(_observedPropertyName);

			if (property == null)
				throw new CaliburnException(
					string.Format("Cannot find property {0} of path {1} in class {2}.",
								  _observedPropertyName,
								  _propertyPath,
								  type.FullName
						)
					);
		}

		private void HookSubpathMonitor()
		{
			if (_subPathMonitor != null)
				_subPathMonitor.Dispose();

			if (string.IsNullOrEmpty(_subPath))
				return;

			var getter = GetPropertyGetMethod();

			var subTarget = getter.Invoke(GetNotifier(true)) as INotifyPropertyChanged;
			if (subTarget != null)
				_subPathMonitor = new PropertyPathMonitor(_methodFactory, subTarget, _subPath, Notify);
		}

		private void Notifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!_onPathChangedActionRef.IsAlive)
			{
				//if a notification is received but the notification delegate was already collected
				//then the current monitor is unnecessarily kept alive
				this.Dispose();
			}
			else
			{
				if (ShouldNotify(e.PropertyName))
					Notify();

				HookSubpathMonitor();
			}
		}

		private INotifyPropertyChanged GetNotifier(bool failIfAlredyCollected)
		{
			INotifyPropertyChanged notifier = null;
			if (!_notifierRef.IsAlive)
			{
				if (failIfAlredyCollected)
					throw new CaliburnException("Target is no longer available.");
			}
			else
			{
				notifier = _notifierRef.Target as INotifyPropertyChanged;
			}
			return notifier;
		}

		private bool ShouldNotify(string propertyName)
		{
			return _observedPropertyName.Equals(propertyName) || _observedPropertyName == ALL_PROPERTIES;
		}

		private void Notify()
		{
			if (_onPathChangedActionRef.IsAlive)
			{
				var notification = _onPathChangedActionRef.Target as Action;
				if (notification != null)
					notification.Invoke();
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			INotifyPropertyChanged notifier = GetNotifier(false);
			if (notifier != null)
				notifier.PropertyChanged -= Notifier_PropertyChanged;

			if (_subPathMonitor != null)
				_subPathMonitor.Dispose();
		}

		private IMethod GetPropertyGetMethod()
		{
			if (_propertyGetMethod == null)
			{
				if (_observedPropertyName.Equals(ALL_PROPERTIES))
					throw new CaliburnException(
						string.Format(
							"'{0}' marker in path {1} is invalid. '{0}' can only be used as leaf property in a path.", ALL_PROPERTIES, _propertyPath));

				var type = GetNotifier(true).GetType();
				var propInfo = type.GetProperty(_observedPropertyName, BindingFlags.Instance | BindingFlags.Public);

				if (propInfo == null)
				{
					throw new CaliburnException(
						string.Format("Cannot find property {0} of path {1} in class {2}.",
									  _observedPropertyName,
									  _propertyPath,
									  type.FullName
							)
						);
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
	}
}