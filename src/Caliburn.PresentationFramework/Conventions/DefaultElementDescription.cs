namespace Caliburn.PresentationFramework.Conventions
{
    using System;

    public class DefaultElementDescription : IElementDescription
    {
        private readonly Type _type;
        private readonly string _name;
        private readonly IElementConvention _convention;

        public DefaultElementDescription(Type type, string name, IElementConvention convention)
        {
            _type = type;
            _name = name;
            _convention = convention;
        }

        public Type Type
        {
            get { return _type; }
        }

        public string Name
        {
            get { return _name; }
        }

        public IElementConvention Convention
        {
            get { return _convention; }
        }
    }
}