namespace Caliburn.Core.Configuration
{
    using IoC;

    /// <summary>
    /// Represents a modules configuration for a service.
    /// </summary>
    /// <typeparam name="TRegistration">The type of the registration.</typeparam>
    /// <typeparam name="TInstance">The type of the instance.</typeparam>
    public interface IConfiguredRegistration<TRegistration, TInstance>
        where TRegistration : ComponentRegistrationBase, new()
    {

    }
}