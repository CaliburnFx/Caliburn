namespace Caliburn.PresentationFramework.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;

    /// <summary>
    /// A default view provided by Caliburn when an <see cref="IViewLocator"/> is unable to locate one.
    /// </summary>
    public class NotFoundView : TextBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundView"/> class.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="searchedViewNames">The searched view names.</param>
        public NotFoundView(Type modelType, IEnumerable<string> searchedViewNames)
        {
            var message = searchedViewNames.Aggregate(
                "A default view was not found for " + modelType.FullName + ".  Views searched for include: ",
                (a, c) => a + Environment.NewLine + c
                );

            Text = message;
            TextWrapping = System.Windows.TextWrapping.Wrap;
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            IsReadOnly = true;
        }
    }
}