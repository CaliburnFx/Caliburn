#if !SILVERLIGHT

namespace Caliburn.PresentationFramework.ApplicationModel
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Controls.Primitives;
	using System.Windows.Data;
	using System.Windows.Navigation;
	using Conventions;
	using Invocation;
	using Screens;
	using ViewModels;
	using Views;
	using System.Linq;

	/// <summary>
	/// An implementation of <see cref="IWindowManager"/>.
	/// </summary>
	public class DefaultWindowManager : IWindowManager
	{
		protected readonly IViewLocator ViewLocator;
		protected readonly IViewModelBinder ViewModelBinder;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultWindowManager"/> class.
		/// </summary>
		/// <param name="viewLocator">The view locator.</param>
		/// <param name="viewModelBinder">The view model binder.</param>
		public DefaultWindowManager(IViewLocator viewLocator, IViewModelBinder viewModelBinder)
		{
			ViewLocator = viewLocator;
			ViewModelBinder = viewModelBinder;
		}

		/// <summary>
		/// Shows the dialog.
		/// </summary>
		/// <param name="rootModel">The root model.</param>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public virtual bool? ShowDialog(object rootModel, object context)
		{
			var window = CreateWindow(rootModel, true, context);
			return window.ShowDialog();
		}

		/// <summary>
		/// Shows a window for the specified model.
		/// </summary>
		/// <param name="rootModel">The root model.</param>
		/// <param name="context">The context.</param>
		public virtual void ShowWindow(object rootModel, object context)
		{
			var navWindow = Application.Current.MainWindow as NavigationWindow;

			if (navWindow != null)
			{
				var window = CreatePage(rootModel, context);
				navWindow.Navigate(window);
			}
			else
			{
				var window = CreateWindow(rootModel, false, context);
				window.Show();
			}
		}

        /// <summary>
        /// Shows a popup at the current mouse position.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="context">The view context.</param>
        /// <param name="settings">The optional popup settings.</param>
        public virtual void ShowPopup(object rootModel, object context, IDictionary<string, object> settings) {
            var popup = CreatePopup(rootModel, settings);
            var view = ViewLocator.LocateForModel(rootModel, popup, context);

            popup.Child = (UIElement)view;
            popup.SetValue(View.IsGeneratedProperty, true);

            ViewModelBinder.Bind(rootModel, popup, null);

            var activatable = rootModel as IActivate;
            if (activatable != null)
                activatable.Activate();

            var deactivator = rootModel as IDeactivate;
            if (deactivator != null)
                popup.Closed += delegate { deactivator.Deactivate(true); };

            popup.IsOpen = true;
            popup.CaptureMouse();
        }

        /// <summary>
        /// Creates a popup for hosting a popup window.
        /// </summary>
        /// <param name="rootModel">The model.</param>
        /// <param name="settings">The optional popup settings.</param>
        /// <returns>The popup.</returns>
        protected virtual Popup CreatePopup(object rootModel, IDictionary<string, object> settings)
        {
            var popup = new Popup();

            if (settings != null)
            {
                var type = popup.GetType();

                foreach (var pair in settings)
                {
                    var propertyInfo = type.GetProperty(pair.Key);

                    if (propertyInfo != null)
                        propertyInfo.SetValue(popup, pair.Value, null);
                }

                if (!settings.ContainsKey("PlacementTarget") && !settings.ContainsKey("Placement"))
                    popup.Placement = PlacementMode.MousePoint;
                if (!settings.ContainsKey("AllowsTransparency"))
                    popup.AllowsTransparency = true;
            }
            else
            {
                popup.AllowsTransparency = true;
                popup.Placement = PlacementMode.MousePoint;
            }

            return popup;
        }

		/// <summary>
		/// Creates the window.
		/// </summary>
		/// <param name="rootModel">The root model.</param>
		/// <param name="isDialog">Indicates it is a dialog window.</param>
		/// <param name="context">The context.</param>
		/// <returns>The window.</returns>
		protected virtual Window CreateWindow(object rootModel, bool isDialog, object context)
		{
			var view = EnsureWindow(rootModel, ViewLocator.LocateForModel(rootModel, null, context), isDialog);
			ViewModelBinder.Bind(rootModel, view, context);

			var haveDisplayName = rootModel as IHaveDisplayName;
			if (haveDisplayName != null && !view.HasBinding(Window.TitleProperty))
			{
				var binding = new Binding("DisplayName") { Mode = BindingMode.TwoWay };
				view.SetBinding(Window.TitleProperty, binding);
			}

			new WindowConductor(rootModel, view);

			return view;
		}

		/// <summary>
		/// Ensures the that the view is a window or provides one.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="view">The view.</param>
		/// <param name="isDialog">Indicates we are insuring a dialog window.</param>
		/// <returns></returns>
		protected virtual Window EnsureWindow(object model, object view, bool isDialog)
		{
			var window = view as Window;

			if (window == null)
			{
				window = new Window
				{
					Content = view,
					SizeToContent = SizeToContent.WidthAndHeight
				};

				window.SetValue(View.IsGeneratedProperty, true);

				var owner = InferOwnerOf(window);
				if (owner != null)
				{
					window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
					window.Owner = owner;
				}
				else
				{
					window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
				}
			}
			else
			{
				var owner = InferOwnerOf(window);
				if (owner != null && isDialog)
					window.Owner = owner;
			}

			return window;
		}

		/// <summary>
		/// Infers the owner of a new window being opened
		/// </summary>
		/// <param name="window">The window being opened</param>
		/// <returns>The inferred owner</returns>
        protected virtual Window InferOwnerOf(Window window)
		{
			if (Application.Current == null) return null;

			var active = Application.Current.Windows.OfType<Window>()
				.Where(x => x.IsActive)
				.FirstOrDefault();
			active = active ?? Application.Current.MainWindow;
			return active == window ? null : active;
		}


		/// <summary>
		/// Creates the page.
		/// </summary>
		/// <param name="rootModel">The root model.</param>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public virtual Page CreatePage(object rootModel, object context)
		{
			var view = EnsurePage(rootModel, ViewLocator.LocateForModel(rootModel, null, context));
			ViewModelBinder.Bind(rootModel, view, context);

			var haveDisplayName = rootModel as IHaveDisplayName;
            if (haveDisplayName != null && !view.HasBinding(Page.TitleProperty))
			{
				var binding = new Binding("DisplayName") { Mode = BindingMode.TwoWay };
				view.SetBinding(Page.TitleProperty, binding);
			}

			var activatable = rootModel as IActivate;
			if (activatable != null)
				activatable.Activate();

			var deactivatable = rootModel as IDeactivate;
			if (deactivatable != null)
				view.Unloaded += (s, e) => deactivatable.Deactivate(true);

			return view;
		}

		/// <summary>
		/// Ensures the view is a page or provides one.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="view">The view.</param>
		/// <returns></returns>
		protected virtual Page EnsurePage(object model, object view)
		{
			var page = view as Page;

			if (page == null)
			{
				page = new Page
				{
					Content = view
				};

				page.SetValue(View.IsGeneratedProperty, true);
			}

			return page;
		}

		class WindowConductor
		{
			bool deactivatingFromView;
			bool deactivateFromViewModel;
			bool actuallyClosing;
			readonly Window view;
			readonly object model;

			public WindowConductor(object model, Window view)
			{
				this.model = model;
				this.view = view;

				var activatable = model as IActivate;
				if (activatable != null)
					activatable.Activate();

				var deactivatable = model as IDeactivate;
				if (deactivatable != null)
				{
					view.Closed += Closed;
					deactivatable.Deactivated += Deactivated;
				}

				var guard = model as IGuardClose;
				if (guard != null)
					view.Closing += Closing;
			}

			void Closed(object sender, EventArgs e)
			{
				view.Closed -= Closed;
				view.Closing -= Closing;

				if (deactivateFromViewModel)
					return;

				var deactivatable = (IDeactivate)model;

				deactivatingFromView = true;
				deactivatable.Deactivate(true);
				deactivatingFromView = false;
			}

			void Deactivated(object sender, DeactivationEventArgs e)
			{
                if (!e.WasClosed)
                    return;

                ((IDeactivate)model).Deactivated -= Deactivated;

                if (deactivatingFromView)
                    return;

				deactivateFromViewModel = true;
				actuallyClosing = true;
				view.Close();
				actuallyClosing = false;
				deactivateFromViewModel = false;
			}

			void Closing(object sender, CancelEventArgs e)
			{
                if(e.Cancel)
                    return;

			    var guard = (IGuardClose)model;

				if (actuallyClosing)
				{
					actuallyClosing = false;
					return;
				}

				bool runningAsync = false, shouldEnd = false;

				guard.CanClose(canClose =>
				{
					Execute.OnUIThread(() =>
					{
						if (runningAsync && canClose)
						{
							actuallyClosing = true;
							view.Close();
						}
						else e.Cancel = !canClose;

						shouldEnd = true;
					});
				});

				if (shouldEnd)
					return;

				runningAsync = e.Cancel = true;
			}
		}
	}
}

#endif