﻿namespace Caliburn.ShellFramework.Configuration
{
    using System;
    using System.Reflection;
    using Core.Configuration;
    using Core.InversionOfControl;
    using Menus;
    using PresentationFramework.ApplicationModel;
    using PresentationFramework.ViewModels;
    using PresentationFramework.Views;
    using Questions;
    using Resources;
    using Results;

    /// <summary>
    /// The shell framework module.
    /// </summary>
    public class ShellFrameworkConfiguration :
        ConventionalModule<ShellFrameworkConfiguration, IShellFrameworkServicesDescription>
    {
        private string viewNamespace;

        /// <summary>
        /// Adds a namespace alias to the <see cref="IViewLocator"/> from Caliburn.ShellFramework.Questions to your custom namespace.
        /// </summary>
        /// <param name="viewNamespace">The view namespace.</param>
        /// <returns>The configruation.</returns>
        public ShellFrameworkConfiguration RedirectViewNamespace(string viewNamespace)
        {
            this.viewNamespace = viewNamespace;
            return this;
        }

        /// <summary>
        /// Localizes the question answers using the provided delegate.
        /// </summary>
        /// <param name="localizer">The localizer.</param>
        /// <returns>The configuration.</returns>
        public ShellFrameworkConfiguration LocalizeAnswersWith(Func<Answer, string> localizer)
        {
            Question.LocalizeAnswer = localizer;
            return this;
        }

        /// <summary>
        /// Sets up the default resource assembly.
        /// </summary>
        /// <param name="defaultResourceAssembly">The default resource assembly.</param>
        /// <returns></returns>
        public ShellFrameworkConfiguration LocateDefaultResourcesIn(Assembly defaultResourceAssembly)
        {
            ResourceExtensions.DefaultResourceAssembly = defaultResourceAssembly;
            return this;
        }

        /// <summary>
        /// Determines the icon path by using a display name.
        /// </summary>
        /// <param name="displayNameToResourcePath">The display name to resource path converter.</param>
        /// <returns>The configuration.</returns>
        public ShellFrameworkConfiguration DetermineIconPathWith(Func<string, string> displayNameToResourcePath)
        {
            ResourceExtensions.DetermineIconPath = displayNameToResourcePath;
            return this;
        }

        /// <summary>
        /// Determines the default implementation.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>The default implemenation.</returns>
        protected override Type DetermineDefaultImplementation(Type service)
        {
            return typeof(IQuestionDialog).IsAssignableFrom(service)
                ? typeof(QuestionDialogViewModel)
                : base.DetermineDefaultImplementation(service);
        }

        /// <summary>
        /// Initializes the module with the specified locator.
        /// </summary>
        /// <param name="locator">The locator.</param>
        public override void Initialize(IServiceLocator locator)
        {
            base.Initialize(locator);

            MenuModel.Initialize(locator.GetInstance<IInputManager>(), locator.GetInstance<IResourceManager>());

            if(!string.IsNullOrEmpty(viewNamespace))
            {
                var viewLocator = locator.GetInstance<IViewLocator>() as DefaultViewLocator;
                if(viewLocator != null)
                    viewLocator.AddNamespaceAlias("Caliburn.ShellFramework.Questions", viewNamespace);
            }
        }
    }
}
