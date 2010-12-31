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
	public class PropertyPathMonitor : IChangeMonitorNode
	{
		const string AllProperties = "*";

		readonly IMethodFactory methodFactory;
		readonly WeakReference notifierRef;
		readonly WeakReference parentMonitorRef;

		readonly string propertyPath;
		readonly string observedPropertyName;
		IMethod propertyGetMethod;

		readonly string subPath;
		PropertyPathMonitor subPathMonitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyPathMonitor"/> class.
        /// </summary>
        /// <param name="methodFactory">The method factory.</param>
        /// <param name="notifier">The notifier.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <param name="parentMonitor">The parent monitor.</param>
        public PropertyPathMonitor(IMethodFactory methodFactory, INotifyPropertyChanged notifier, string propertyPath, IChangeMonitorNode parentMonitor)
		{
			this.methodFactory = methodFactory;
			notifierRef = new WeakReference(notifier);
			parentMonitorRef = new WeakReference(parentMonitor);
			this.propertyPath = propertyPath;

			observedPropertyName = GetRootProperty(this.propertyPath);
			subPath = GetSubPath(this.propertyPath);

			VerifyObservedPropertyExists();

			notifier.PropertyChanged += Notifier_PropertyChanged;
			HookSubpathMonitor();
		}

		void VerifyObservedPropertyExists()
		{
			if (observedPropertyName.Equals(AllProperties))
				return;

			var type = GetNotifier(true).GetType();

			var property = type.GetProperty(observedPropertyName);

			if (property == null)
				throw new CaliburnException(
					string.Format("Cannot find property {0} of path {1} in class {2}.",
								  observedPropertyName,
								  propertyPath,
								  type.FullName
						)
					);
		}

		void HookSubpathMonitor()
		{
			if (subPathMonitor != null)
				subPathMonitor.Dispose();

			if (string.IsNullOrEmpty(subPath))
				return;

			var getter = GetPropertyGetMethod();

			var subTarget = getter.Invoke(GetNotifier(true)) as INotifyPropertyChanged;
			if (subTarget != null)
				subPathMonitor = new PropertyPathMonitor(methodFactory, subTarget, subPath, this);
		}

		void Notifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (ShouldStopMonitoring()) {
				Dispose();

			} else {

				if (ShouldNotify(e.PropertyName))
					NotifyChange();

				HookSubpathMonitor();
			}
		}

		INotifyPropertyChanged GetNotifier(bool failIfAlredyCollected)
		{
			INotifyPropertyChanged notifier = null;
			if (!notifierRef.IsAlive)
			{
				if (failIfAlredyCollected)
					throw new CaliburnException("Target is no longer available.");
			}
			else
			{
				notifier = notifierRef.Target as INotifyPropertyChanged;
			}
			return notifier;
		}

		bool ShouldNotify(string propertyName)
		{
			return observedPropertyName.Equals(propertyName) || observedPropertyName == AllProperties;
		}

        /// <summary>
        /// The parent node.
        /// </summary>
        /// <value></value>
		public IChangeMonitorNode Parent {
			get {
				if (parentMonitorRef != null && parentMonitorRef.IsAlive)
					return parentMonitorRef.Target as IChangeMonitorNode;
				return null;
			}
		}

        /// <summary>
        /// Indicates whether to stop monitoring changes.
        /// </summary>
        /// <returns></returns>
		public bool ShouldStopMonitoring() {
			return Parent == null || Parent.ShouldStopMonitoring();
		}

        /// <summary>
        /// Raises change notification.
        /// </summary>
		public void NotifyChange()
		{	
			var parent = Parent;
			if (parent!= null)
				parent.NotifyChange();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			var notifier = GetNotifier(false);
			if (notifier != null)
				notifier.PropertyChanged -= Notifier_PropertyChanged;

			if (subPathMonitor != null)
				subPathMonitor.Dispose();
		}

		IMethod GetPropertyGetMethod()
		{
			if (propertyGetMethod == null)
			{
				if (observedPropertyName.Equals(AllProperties))
					throw new CaliburnException(
						string.Format(
							"'{0}' marker in path {1} is invalid. '{0}' can only be used as leaf property in a path.", AllProperties, propertyPath));

				var type = GetNotifier(true).GetType();
				var propInfo = type.GetProperty(observedPropertyName, BindingFlags.Instance | BindingFlags.Public);

				if (propInfo == null)
				{
					throw new CaliburnException(
						string.Format("Cannot find property {0} of path {1} in class {2}.",
									  observedPropertyName,
									  propertyPath,
									  type.FullName
							)
						);
				}

				propertyGetMethod = methodFactory.CreateFrom(propInfo.GetGetMethod());
			}

			return propertyGetMethod;
		}

		static string GetRootProperty(string propertyPath)
		{
			var index = propertyPath.IndexOf(".");
			if (index < 0) index = propertyPath.Length;

			return propertyPath.Substring(0, index);
		}

		static string GetSubPath(string propertyPath)
		{
			var index = propertyPath.IndexOf(".");
			if (index < 0 || index >= propertyPath.Length) return null;

			return propertyPath.Substring(index + 1);
		}
	}
}