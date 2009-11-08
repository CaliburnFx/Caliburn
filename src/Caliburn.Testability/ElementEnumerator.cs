namespace Caliburn.Testability
{
    using System.Collections.Generic;

    /// <summary>
    /// A class capable of enumerating all aspects of a UI.  A user defined visitor visits the enumerated UI elements.
    /// </summary>
    public class ElementEnumerator
    {
        private readonly IElement _element;
        private readonly ElementEnumeratorSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementEnumerator"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        public ElementEnumerator(IElement element)
        {
            _element = element;
            _settings = new ElementEnumeratorSettings();
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public ElementEnumeratorSettings Settings
        {
            get { return _settings; }
        }

        /// <summary>
        /// Enumerates the UI with the specified visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public void Enumerate(IElementVisitor visitor)
        {
            visitor.Prepare(_settings);

            var queue = new Queue<IElement>();
            queue.Enqueue(_element);

            while(queue.Count > 0)
            {
                var current = queue.Dequeue();

                current.Accept(visitor);

                if(visitor.ShouldStopVisiting) break;

                foreach(var dataBound in current.GetChildren(_settings))
                {
                    queue.Enqueue(dataBound);
                }
            }
        }
    }
}