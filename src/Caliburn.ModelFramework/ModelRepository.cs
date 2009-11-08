namespace Caliburn.ModelFramework
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of <see cref="IModelRepository"/>.
    /// </summary>
    public class ModelRepository : IModelRepository
    {
        private static IModelRepository _current = new ModelRepository();

        private readonly Dictionary<Type, IModelDefinition> _definitions =
            new Dictionary<Type, IModelDefinition>();

        /// <summary>
        /// Gets the current model repository.
        /// </summary>
        /// <value>The current.</value>
        public static IModelRepository Current
        {
            get { return _current; }
        }

        /// <summary>
        /// Initializes the specified model repository.
        /// </summary>
        /// <param name="modelRepository">The model repository.</param>
        public static void Initialize(IModelRepository modelRepository)
        {
            _current = modelRepository;
        }

        /// <summary>
        /// Clears the repository.
        /// </summary>
        public void Clear()
        {
            _definitions.Clear();
        }

        /// <summary>
        /// Adds the model to the repository.
        /// </summary>
        /// <param name="type">The instance type to which the definition applies.</param>
        /// <param name="modelDefinition">The model definition.</param>
        public void AddModel(Type type, IModelDefinition modelDefinition)
        {
            _definitions[type] = modelDefinition;
        }

        /// <summary>
        /// Gets the model for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IModelDefinition GetModelFor(Type type)
        {
            IModelDefinition model;

            if(!_definitions.TryGetValue(type, out model))
            {
                model = CreateModel(type);
                _definitions[type] = model;
            }

            return model;
        }

        private IModelDefinition CreateModel(Type type)
        {
            var model = new ModelDefinition();

            if(type.BaseType != typeof(ModelBase))
            {
                var baseModel = GetModelFor(type.BaseType);

                foreach(var propertyDefinition in baseModel)
                {
                    model.AddProperty(propertyDefinition);
                }
            }

            return model;
        }
    }
}