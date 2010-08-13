using System.Linq;
using Caliburn.Testability;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Tests.Caliburn.Fakes.Model;
using Tests.Caliburn.Fakes.UI;
using System;
using System.Threading;
using System.Security.Permissions;
using System.Reflection;

namespace Tests.Caliburn.Testability
{
	[TestFixture]
	public class When_testing_Xaml_element : TestBase
	{
		[Test]
		public void Simple_Xaml_is_loaded_correctly()
		{
			var validator = Validator.For<SimpleUIBuiltWithXaml, Customer>();
			var result = validator.Validate();

			Assert.That(result.Errors.Count(), Is.EqualTo(0));

		}

		[Test]
		public void Xaml_with_markup_extensions_is_loaded_correctly()
		{
			var validator = Validator.For<UIWithMarkupExtensionsBuiltWithXaml, Customer>();
			var result = validator.Validate();

			Assert.That(result.Errors.Count(), Is.EqualTo(0));

		} 

		 


	}
}