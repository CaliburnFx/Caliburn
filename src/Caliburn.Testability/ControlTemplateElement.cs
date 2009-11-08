namespace Caliburn.Testability
{
    using System.Windows.Controls;

    /// <summary>
    /// Represents a bound <see cref="ControlTemplate"/>.
    /// </summary>
    public class ControlTemplateElement : DependencyObjectElement
    {
        private readonly ControlTemplate _controlTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTemplateElement"/> class.
        /// </summary>
        /// <param name="controlTemplate">The data template.</param>
        /// <param name="boundType">Type of the bound.</param>
        internal ControlTemplateElement(ControlTemplate controlTemplate, BoundType boundType)
            : this(controlTemplate, boundType, string.Empty) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTemplateElement"/> class.
        /// </summary>
        /// <param name="controlTemplate">The data template.</param>
        /// <param name="boundType">Type of the bound.</param>
        /// <param name="baseName">Name of the base.</param>
        internal ControlTemplateElement(ControlTemplate controlTemplate, BoundType boundType, string baseName)
            : base(controlTemplate.LoadContent(), boundType)
        {
            _controlTemplate = controlTemplate;

            if(_controlTemplate.TargetType != null)
                BaseName = baseName + " [ControlTemplate " + _controlTemplate.TargetType.Name + "] ";

            else BaseName = baseName + " [ControlTemplate] ";
        }

        /// <summary>
        /// Gets the control template.
        /// </summary>
        /// <value>The control template.</value>
        public ControlTemplate ControlTemplate
        {
            get { return _controlTemplate; }
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