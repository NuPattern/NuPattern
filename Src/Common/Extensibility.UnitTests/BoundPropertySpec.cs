using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Patterning.Extensibility.Binding;

namespace Microsoft.VisualStudio.Patterning.Extensibility.UnitTests
{
	[TestClass]
	public class BoundPropertySpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestMethod]
		public void WhenInitializingSettings_ThenHasPropertyName()
		{
			var property = new BoundProperty("Name", () => "", s => { });

			Assert.Equal("Name", property.Settings.Name);
		}

		[TestMethod]
		public void WhenValueIsEmpty_ThenSettingsInitializedEmpty()
		{
			var property = new BoundProperty("Name", () => "", s => { });

			Assert.Equal("", property.Settings.Value);
		}

		[TestMethod]
		public void WhenChangingSettingsValue_ThenAutomaticallySerializesValue()
		{
			var value = "";
			var property = new BoundProperty("Name", () => value, s => value = s);

			property.Settings.Value = "Hello";

			Assert.NotEqual("", value);
		}

		[TestMethod]
		public void WhenReassigningSettings_ThenSavesNewValue	()
		{
			var value = "";
			var property = new BoundProperty("Name", () => value, s => value = s);
			var settings = property.Settings;

			property.Settings = new PropertyBindingSettings { Name = "Name", Value = "Hello" };

			Assert.NotEqual("", value);
		}

		[TestMethod]
		public void WhenReassigningSettings_ThenDetachesChangesFromOlder()
		{
			var value = "";
			var property = new BoundProperty("Name", () => value, s => value = s);
			var settings = property.Settings;

			property.Settings = new PropertyBindingSettings { Name = "Name", Value = "Hello" };

			settings.Value = "Foo";

			Assert.False(value.Contains("Foo"));
		}

		[TestMethod]
		public void WhenExistingLegacyRawValueExists_ThenAutomaticallyUpgradesToBindingValue()
		{
			var value = "Foo";
			var property = new BoundProperty("Name", () => value, s => value = s);

			Assert.Equal("Foo", property.Settings.Value);
			Assert.True(value.Trim().StartsWith("{"));
		}
	}
}
