using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility.Binding;

namespace NuPattern.Extensibility.UnitTests
{
	[TestClass]
	public class BoundPropertySpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestMethod, TestCategory("Unit")]
		public void WhenInitializingSettings_ThenHasPropertyName()
		{
			var property = new BoundProperty("Name", () => "", s => { });

			Assert.Equal("Name", property.Settings.Name);
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenValueIsEmpty_ThenSettingsInitializedEmpty()
		{
			var property = new BoundProperty("Name", () => "", s => { });

			Assert.Equal("", property.Settings.Value);
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenChangingSettingsValue_ThenAutomaticallySerializesValue()
		{
			var value = "";
			var property = new BoundProperty("Name", () => value, s => value = s);

			property.Settings.Value = "Hello";

			Assert.NotEqual("", value);
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenReassigningSettings_ThenSavesNewValue	()
		{
			var value = "";
			var property = new BoundProperty("Name", () => value, s => value = s);
			var settings = property.Settings;

			property.Settings = new PropertyBindingSettings { Name = "Name", Value = "Hello" };

			Assert.NotEqual("", value);
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenReassigningSettings_ThenDetachesChangesFromOlder()
		{
			var value = "";
			var property = new BoundProperty("Name", () => value, s => value = s);
			var settings = property.Settings;

			property.Settings = new PropertyBindingSettings { Name = "Name", Value = "Hello" };

			settings.Value = "Foo";

			Assert.False(value.Contains("Foo"));
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenExistingLegacyRawValueExists_ThenAutomaticallyUpgradesToBindingValue()
		{
			var value = "Foo";
			var property = new BoundProperty("Name", () => value, s => value = s);

			Assert.Equal("Foo", property.Settings.Value);
			Assert.True(value.Trim().StartsWith("{"));
		}
	}
}
