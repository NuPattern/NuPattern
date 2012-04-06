using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility.Binding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Extensibility.UnitTests
{
	[TestClass]
	public class FixedBindingSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestMethod]
		public void WhenCreatingBinding_ThenValuesAreSame()
		{
			var value = new object();
			var binding = FixedBinding.Create(value);

			Assert.Same(value, binding.Value);
		}

		[TestMethod]
		public void WhenCreated_ThenCreateDynamicContextSucceeds()
		{
			var value = new object();
			var binding = FixedBinding.Create(value);

			Assert.NotNull(binding.CreateDynamicContext());
		}

		[TestMethod]
		public void WhenCreated_ThenEvaluatesToTrue()
		{
			var value = new object();
			var binding = FixedBinding.Create(value);

			Assert.True(binding.Evaluate());
		}

		[TestMethod]
		public void WhenCreated_ThenEvaluatesToTrueWithContext()
		{
			var value = new object();
			var binding = FixedBinding.Create(value);
			var context = binding.CreateDynamicContext();

			Assert.True(binding.Evaluate(context));
		}

		[TestMethod]
		public void WhenCreated_ThenEvaluationResultsAreEmpty()
		{
			var value = new object();
			var binding = FixedBinding.Create(value);

			Assert.False(binding.EvaluationResults.Any());
		}

		[TestMethod]
		public void WhenCreated_ThenHasErrorsIsFalse()
		{
			var value = new object();
			var binding = FixedBinding.Create(value);

			Assert.False(binding.HasErrors);
		}

		[TestMethod]
		public void WhenCreated_ThenUserMessageCanBeSet()
		{
			var value = new object();
			var binding = FixedBinding.Create(value);

			binding.UserMessage = "foo";

			Assert.Equal("foo", binding.UserMessage);
		}
	}
}
