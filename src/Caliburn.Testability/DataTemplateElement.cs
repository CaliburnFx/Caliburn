namespace Caliburn.Testability
{
    using System.Windows;

    /// <summary>
    /// Represents a bound <see cref="DataTemplate"/>.
    /// </summary>
    public class DataTemplateElement : DependencyObjectElement
    {
        readonly DataTemplate dataTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTemplateElement"/> class.
        /// </summary>
        /// <param name="dataTemplate">The data template.</param>
        /// <param name="boundType">Type of the bound.</param>
        internal DataTemplateElement(DataTemplate dataTemplate, BoundType boundType)
            : this(dataTemplate, boundType, string.Empty) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTemplateElement"/> class.
        /// </summary>
        /// <param name="dataTemplate">The data template.</param>
        /// <param name="boundType">Type of the bound.</param>
        /// <param name="baseName">Name of the base.</param>
        internal DataTemplateElement(DataTemplate dataTemplate, BoundType boundType, string baseName)
            : base(dataTemplate.LoadContent(), boundType)
        {
            this.dataTemplate = dataTemplate;

            if(this.dataTemplate.DataTemplateKey != null)
                BaseName = baseName + " [DataTemplate " + this.dataTemplate.DataTemplateKey + "] ";

            else BaseName = baseName + " [DataTemplate] ";
        }

        /// <summary>
        /// Gets the data template.
        /// </summary>
        /// <value>The data template.</value>
        public DataTemplate DataTemplate
        {
            get { return dataTemplate; }
        }

        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public override void Accept(IElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}