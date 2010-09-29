namespace Caliburn.Windsor
{
	using System.Collections.Generic;
	using Castle.MicroKernel.Registration;
	using Castle.MicroKernel.SubSystems.Configuration;
	using Castle.Windsor;

	/// <summary>
	/// An <see cref="IWindsorInstaller"/> that collects all component registrations found by Caliburn to be performed during installation.
	/// </summary>
	public class ComponentInstaller : IWindsorInstaller
	{
		private readonly List<IRegistration> registrations = new List<IRegistration>();

		/// <summary>
		/// Adds a component registration to the installer.
		/// </summary>
		/// <param name="registration">The component registration to be added.</param>
		public void AddRegistration(IRegistration registration)
		{
			registrations.Add(registration);
		}

		/// <summary>
		/// Performs the installation in the <see cref="T:Castle.Windsor.IWindsorContainer"/>.
		/// </summary>
		/// <param name="container">The container.</param><param name="store">The configuration store.</param>
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(registrations.ToArray());
			registrations.Clear();
		}
	}
}