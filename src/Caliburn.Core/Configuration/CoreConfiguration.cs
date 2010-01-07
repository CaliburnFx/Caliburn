namespace Caliburn.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using Invocation;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Configures Caliburn's core.
    /// </summary>
    public class CoreConfiguration : ConventionalModule<CoreConfiguration, ICoreServicesDescription>
    {
        private readonly List<Action> _afterStart = new List<Action>();

        /// <summary>
        /// Adds actions to execute immediately following the framework startup.
        /// </summary>
        /// <param name="doThis">The action to execute after framework startup.</param>
        /// <returns></returns>
        public CoreConfiguration AfterStart(Action doThis)
        {
            _afterStart.Add(doThis);
            return this;
        }

        /// <summary>
        /// Initializes the core.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        protected override void InitializeCore(IServiceLocator serviceLocator)
        {
            Execute.Initialize(serviceLocator.GetInstance<IDispatcher>());
        }

        internal void ExecuteAfterStart()
        {
            _afterStart.Apply(x => x());
            _afterStart.Clear();
        }
    }
}