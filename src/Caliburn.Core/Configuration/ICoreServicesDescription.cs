namespace Caliburn.Core.Configuration
{
    using Invocation;
    using InversionOfControl;
    using Validation;

    /// <summary>
    /// Describes the services required for the core framework to function.
    /// </summary>
    public interface ICoreServicesDescription
    {
        /// <summary>
        /// Customizes the method factory used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The method factory type.</typeparam>
        IConfiguredRegistration<Singleton, T> MethodFactory<T>() where T : IMethodFactory;

        /// <summary>
        /// Usings the assembly source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        IConfiguredRegistration<Singleton, T> AssemblySource<T>() where T : IAssemblySource;

        /// <summary>
        /// Customizes the validator used by Caliburn.
        /// </summary>
        /// <typeparam name="T">The validator type.</typeparam>
        IConfiguredRegistration<Singleton, T> Validator<T>() where T : IValidator;
    }
}