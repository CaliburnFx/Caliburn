using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.PresentationFramework.Views;
using Caliburn.Core;
using NUnit.Framework;
using Microsoft.Practices.ServiceLocation;
using Caliburn.PresentationFramework.Screens;
using NUnit.Framework.SyntaxHelpers;
namespace Tests.Caliburn.MVP
{
	[TestFixture]
	public class The_DefaultViewLocator : TestBase
	{
		TestFriendlyDefaultViewLocator defaultViewLocator;

		protected override void given_the_context_of()
		{
			base.given_the_context_of();
			defaultViewLocator = new TestFriendlyDefaultViewLocator(
				Stub<IAssemblySource>(),
				Stub<IServiceLocator>()
				);
		}


		[Test]

		public void should_make_an_interface_name_from_a_name_part()
		{
			AssertMakeInterface("A.Simple.Full.Name", "A.Simple.Full.IName");
			AssertMakeInterface<Tests.Caliburn.Fakes.ViewModel.Address>("Tests.Caliburn.Fakes.ViewModel.IAddress");

			//http://caliburn.codeplex.com/workitem/6275
			
			var stringAQN = typeof(string).AssemblyQualifiedName;
			var intAQN = typeof(int).AssemblyQualifiedName;

			AssertMakeInterface<System.Collections.Generic.List<string>>(
				string.Format("System.Collections.Generic.IList`1[[{0}]]", stringAQN)
				);

			AssertMakeInterface<System.Collections.Generic.KeyValuePair<int, string>>(
				string.Format("System.Collections.Generic.IKeyValuePair`2[[{0}],[{1}]]", intAQN, stringAQN)
				);

		}

		private void AssertMakeInterface(string part, string expected)
		{
			var interfaceName = defaultViewLocator.MakeInterface(part);
			Assert.That(interfaceName, Is.EqualTo(expected));
		}
		private void AssertMakeInterface<T>(string expected)
		{
			AssertMakeInterface(typeof(T).FullName, expected);
		}
	}



	public class TestFriendlyDefaultViewLocator : DefaultViewLocator
	{
		public TestFriendlyDefaultViewLocator(IAssemblySource assemblySource, IServiceLocator serviceLocator)
			: base(assemblySource, serviceLocator) { }

		public new string MakeInterface(string part)
		{
			return base.MakeInterface(part);
		}

		public new IEnumerable<string> GetTypeNamesToCheck(Type modelType, string context)
		{
			return base.GetTypeNamesToCheck(modelType, context);
		}
	}




}
