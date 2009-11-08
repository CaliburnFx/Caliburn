namespace Caliburn.Testability
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Specifies the details of how a <see cref="ElementEnumerator"/> will do its work.
    /// </summary>
    public class ElementEnumeratorSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to stop after the first error.
        /// </summary>
        /// <value><c>true</c> if should stop; otherwise, <c>false</c>.</value>
        /// <remarks>False by default.</remarks>
        public bool StopAfterFirstError { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include styles during validation.
        /// </summary>
        /// <value><c>true</c> to include styles; otherwise, <c>false</c>.</value>
        /// <remarks>True by default.</remarks>
        public bool IncludeStyles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include templates during validation.
        /// </summary>
        /// <value><c>true</c> to include templates; otherwise, <c>false</c>.</value>
        /// <remarks>True by default.</remarks>
        public bool IncludeTemplates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include all dependency object values during validation.
        /// </summary>
        /// <value>
        /// 	<c>true</c> to include all dependency object values; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>An example of</remarks>
        /// <remarks>False by default. When set to true, will attempt to validate all properties which have dependency
        /// object values.  For example, Brushes and ToolTips will be checked.</remarks>
        public bool IncludeAllDependencyObjects { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include child user controls during validation.
        /// </summary>
        /// <value><c>true</c> to include user controls; otherwise, <c>false</c>.</value>
        /// <remarks>False by default.</remarks>
        public bool TraverseUserControls { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementEnumeratorSettings"/> class.
        /// </summary>
        public ElementEnumeratorSettings()
        {
            StopAfterFirstError = false;

            IncludeStyles = true;
            IncludeTemplates = true;

            IncludeAllDependencyObjects = false;
            TraverseUserControls = false;
        }

        /// <summary>
        /// Gets the bound property.
        /// </summary>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="boundType">Type of the bound object.</param>
        /// <param name="baseName">A base name.</param>
        /// <returns>Returns an instance of <see cref="IElement"/> if the property should be checked, otherwise null.</returns>
        public IElement GetBoundProperty(object propertyValue, BoundType boundType, string baseName)
        {
            if(propertyValue is Style)
            {
                if(IncludeStyles)
                {
                    return Bound.Style((Style)propertyValue, boundType, baseName);
                }
            }

            else if(propertyValue is DataTemplate)
            {
                if(IncludeTemplates)
                {
                    var template = (DataTemplate)propertyValue;
                    var dataType = template.DataType as Type;

                    if(dataType == null) return null;
                    return Bound.DataTemplate(template, new BoundType(dataType), baseName);
                }
            }

            else if(propertyValue is ControlTemplate)
            {
                if(IncludeTemplates)
                {
                    var template = (ControlTemplate)propertyValue;
                    return Bound.ControlTemplate(template, boundType, baseName);
                }
            }

            else if(propertyValue is UserControl)
            {
                return Bound.DependencyObject((UserControl)propertyValue, boundType, baseName + ".",
                                              TraverseUserControls);
            }

            else if(propertyValue is IEnumerable)
            {
                if(IncludeAllDependencyObjects)
                    return Bound.Enumerable((IEnumerable)propertyValue, boundType, baseName + ".");
            }
            else if(propertyValue is DependencyObject && IncludeAllDependencyObjects)
                return Bound.DependencyObject((DependencyObject)propertyValue, boundType, baseName + ".");

            return null;
        }
    }
}