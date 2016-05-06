#if !SILVERLIGHT

namespace Caliburn.PresentationFramework
{
    using System;
    using System.Windows.Markup;
    using Configuration;
    using Core.InversionOfControl;

    /// <summary>
    /// A Markup Extension that enables type resolution through the DI container in XAML.
    /// </summary>
    public class ResolveExtension : MarkupExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveExtension"/> class.
        /// </summary>
        public ResolveExtension() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveExtension"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public ResolveExtension(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the design time value.
        /// </summary>
        /// <value>The design time value.</value>
        public object DesignTimeValue { get; set; }

        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (PresentationFrameworkConfiguration.IsInDesignMode) return DesignTimeValue;

            if (string.IsNullOrEmpty(Key)) return IoC.GetInstance(Type, null);
            return Type == null ? IoC.GetInstance(null, Key) : IoC.GetInstance(Type, Key);
        }
    }
}

#endif