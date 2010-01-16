namespace Caliburn.PresentationFramework.Behaviors
{
    using System;
    using System.Collections.Generic;
    using Core.Behaviors;
    using Screens;

    /// <summary>
    /// Applies a behavior which implements <see cref="IScreen"/>.
    /// </summary>
    public class ScreenAttribute : Attribute, IBehavior 
    {
        /// <summary>
        /// Gets or sets the default display name property name.
        /// </summary>
        /// <value>The display name.</value>
        public static string DefaultDisplayName = "Name";

        /// <summary>
        /// Gets or sets the default initialize method name.
        /// </summary>
        /// <value>The initialize.</value>
        public static string DefaultInitialize = "OnInitialize";

        /// <summary>
        /// Gets or sets the default can shutdown method name.
        /// </summary>
        /// <value>The can shutdown.</value>
        public static string DefaultCanShutdown = "OnCanShutdown";

        /// <summary>
        /// Gets or sets the default shutdown method name.
        /// </summary>
        /// <value>The can shutdown.</value>
        public static string DefaultShutdown = "OnShutdown";

        /// <summary>
        /// Gets or sets the default activate method name.
        /// </summary>
        /// <value>The activate.</value>
        public static string DefaultActivate = "OnActivate";

        /// <summary>
        /// Gets or sets the default deactivate method name.
        /// </summary>
        /// <value>The deactivate.</value>
        public static string DefaultDeactivate = "OnDeactivate";

        /// <summary>
        /// Gets or sets the display name property name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the initialize method name.
        /// </summary>
        /// <value>The initialize.</value>
        public string Initialize { get; set; }

        /// <summary>
        /// Gets or sets the can shutdown method name.
        /// </summary>
        /// <value>The can shutdown.</value>
        public string CanShutdown { get; set; }

        /// <summary>
        /// Gets or sets the shutdown method name.
        /// </summary>
        /// <value>The shutdown.</value>
        public string Shutdown { get; set; }

        /// <summary>
        /// Gets or sets the activate method name.
        /// </summary>
        /// <value>The activate.</value>
        public string Activate { get; set; }

        /// <summary>
        /// Gets or sets the deactivate method name.
        /// </summary>
        /// <value>The deactivate.</value>
        public string Deactivate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenAttribute"/> class.
        /// </summary>
        public ScreenAttribute()
        {
            DisplayName = DefaultDisplayName;
            Initialize = DefaultInitialize;
            CanShutdown = DefaultCanShutdown;
            Shutdown = DefaultShutdown;
            Activate = DefaultActivate;
            Deactivate = DefaultDeactivate;
        }

        /// <summary>
        /// Gets the interfaces which represent this behavior.
        /// </summary>
        /// <param name="implementation">The implementation.</param>
        /// <returns>The representative interfaces.</returns>
        public IEnumerable<Type> GetInterfaces(Type implementation)
        {
            yield return typeof(IScreen);
        }
    }
}