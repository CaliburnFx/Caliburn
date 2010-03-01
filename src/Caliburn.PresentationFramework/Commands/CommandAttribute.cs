namespace Caliburn.PresentationFramework.Commands
{
    using System;
    using Core.IoC;

    /// <summary>
    /// Specifies a command which should be registered with the container and optionally allows customization of the executiion method.
    /// </summary>
    public class CommandAttribute : Attribute, IComponentMetadata
    {
        /// <summary>
        /// The suffix that is removed from the class name when registering by Key with the container.
        /// </summary>
        public static string CommandNameSuffix = "Command";

        private readonly string _name;
        private string _executeMethod = "Execute";

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAttribute"/> class.
        /// </summary>
        public CommandAttribute() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAttribute"/> class.
        /// </summary>
        /// <param name="name">The name to use when registering the command with the container.</param>
        public CommandAttribute(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Gets or sets the method to use as the execution method of the command.
        /// </summary>
        /// <value>The execute method.</value>
        public string ExecuteMethod
        {
            get { return _executeMethod; }
            set { _executeMethod = value; }
        }

        /// <summary>
        /// Registers the type with the specified container.
        /// </summary>
        /// <param name="decoratedType">The decorated type.</param>
        /// <returns></returns>
        public IComponentRegistration GetComponentInfo(Type decoratedType)
        {
            return new PerRequest(GetCommandName(decoratedType)) {Implementation = decoratedType};
        }

        /// <summary>
        /// Gets the name of the command, inferring it if no name was given.
        /// </summary>
        /// <param name="commandType">The command type.</param>
        /// <returns>The command name.</returns>
		public string GetCommandName(Type commandType) {
			return _name ?? InferCommandName(commandType);
		}

        /// <summary>
        /// Infers the name of the command.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <returns></returns>
        protected virtual string InferCommandName(Type commandType)
        {
            var typeName = commandType.Name;

            return typeName.EndsWith(CommandNameSuffix)
					   ? typeName.Substring(0, typeName.Length - CommandNameSuffix.Length)
                       : typeName;
        }
    }
}