namespace Caliburn.PresentationFramework.Commands
{
    using System;
    using Core.InversionOfControl;

    /// <summary>
    /// Specifies a command which should be registered with the container and optionally allows customization of the executiion method.
    /// </summary>
    public class CommandAttribute : Attribute, IComponentMetadata
    {
        internal static string CommandNameSuffix = "Command";

        readonly string name;
        string executeMethod = "Execute";

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
            this.name = name;
        }

        /// <summary>
        /// Gets or sets the method to use as the execution method of the command.
        /// </summary>
        /// <value>The execute method.</value>
        public string ExecuteMethod
        {
            get { return executeMethod; }
            set { executeMethod = value; }
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
			return name ?? InferCommandName(commandType);
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